# BO Wrapper Generator Patterns

This document catalogs all the code patterns found in existing Kore Commerce generated business object (`_gen.cs`) files. These patterns must be supported by the Kraken `BOWrapperFileGenerator` to achieve API compatibility with existing consuming code.

## Overview

The existing Kore Commerce code generator produces several types of generated files per domain:

| File Type | Example | Purpose |
|-----------|---------|---------|
| `{Entity}_gen.cs` | `PaymentTerms_gen.cs` | Main BO class with properties and relationships |
| `{Entity}List_gen.cs` | `PaymentTermsList_gen.cs` | Typed list class |
| `BO{Domain}_gen.cs` | `BOPaymentTerm_gen.cs` | Domain object (simple domains) |
| `BO{Domain}_gen.cs` | `BOBusinessEntity_gen.cs` | Super-domain object (partitioned domains) |
| `BO{Entity}_gen.cs` | `BOCustomer_gen.cs` | Sub-domain object (partition members) |

---

## Pattern Categories

### 1. Class Declaration Patterns

#### 1.1 Simple Entity Class
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
[Serializable]
public class PaymentTerms_gen : BOEntity<PaymentTerms, Data.payment_terms, kssf_scope_allornone, PaymentTermsList>, 
    IBORepositoryProvider<PaymentTerms>
```

**Elements:**
- `TypeConverter` attribute for data binding
- `Serializable` attribute
- Inherits from `BOEntity<TObject, TData, TScope, TList>`
- Implements `IBORepositoryProvider<TObject>`

#### 1.2 Entity with Extended Properties
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
[Serializable]
[ExtendedPropertiesSupported(typeof(Customer))]
public class Customer_gen : BOEntity<Customer, Data.business_entity, Data.kssf_scope_Customer, CustomerList>, 
    IBORepositoryProvider<Customer>, IExtendable
```

**Additional Elements:**
- `ExtendedPropertiesSupported` attribute
- Implements `IExtendable` interface

#### 1.3 Super-Domain Class (Partitioned)
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
[Serializable]
public class BOBusinessEntity_gen : BODomainSuper<BusinessEntity>, IDomainObject
```

**Elements:**
- Inherits from `BODomainSuper<TEntity>` instead of `BOEntity`
- Implements `IDomainObject`

#### 1.4 Sub-Domain Class (Partition Member)
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
[Serializable]
public class BOCustomer_gen : BODomainSub<Customer, BOBusinessEntity>, IDomainObject
```

**Elements:**
- Inherits from `BODomainSub<TEntity, TSuperDomain>`
- Implements `IDomainObject`

---

### 2. Interface Patterns

#### 2.1 Entity Interface
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
public interface IPaymentTerms
{
    System.Int32 PaymentTermSeq { get; set; }
    System.String TermType { get; set; }
    // ... all properties
}
```

#### 2.2 Domain Interface
```csharp
[TypeConverter(typeof(GenericTypeConverter))]
public interface IBOPaymentTerm { }
```

---

### 3. Property Patterns

#### 3.1 Simple Bound Property (Single Table)
```csharp
[Bindable(true)]
[DatabaseFieldAttribute("payment_terms", "term_type", false, false, false, true, 8, false)]
[ValidateStringLength(8)]
public virtual System.String TermType {
    get { return Row.term_type; }
    set { 
        errors.Remove("TermType");
        Row.term_type = value;
        AddContainer();
    }
}
```

**Elements:**
- `Bindable(true)` attribute
- `DatabaseFieldAttribute(table, column, isKey, isComputed, isIdentity, isNullable, length, isFixedLength)`
- Optional validation attributes (`ValidateStringLength`, `ValidateRequired`, etc.)
- Getter: `Row.column_name` (snake_case)
- Setter: Error removal, assignment, `AddContainer()` call

#### 3.2 Required Property
```csharp
[Bindable(true)]
[DatabaseFieldAttribute("payment_terms", "payment_term_seq", true, false, false, false, 10, false)]
[ValidateRequired]
public virtual System.Int32 PaymentTermSeq {
    get { return Row.payment_term_seq; }
    set { 
        errors.Remove("PaymentTermSeq");
        Row.payment_term_seq = value;
        AddContainer();
    }
}
```

#### 3.3 Nullable Property
```csharp
virtual public bool IsNullDiscountPct()
{
    return Row.discount_pct == null;
}

[Bindable(true)]
[DatabaseFieldAttribute("payment_terms", "discount_pct", false, false, false, true, 19, false)]
public virtual System.Double? DiscountPct {
    get { return Row.discount_pct; }
    set { 
        errors.Remove("DiscountPct");
        Row.discount_pct = value;
        AddContainer();
    }
}
```

**Additional Element:** `IsNull{Property}()` method for nullable properties

#### 3.4 Boolean Flag Property (Y/N to bool)
```csharp
[Bindable(true)]
[DatabaseFieldAttribute("order_header", "approved_flg", false, false, false, false, 1, false)]
public virtual System.String ApprovedFlg {
    get { return Row.approved_flg; }
    set { ... }
}

public virtual System.Boolean IsApprovedFlg {
    get { return "Y".Equals(ApprovedFlg, StringComparison.OrdinalIgnoreCase); }
    set { ApprovedFlg = value ? "Y" : "N"; }
}
```

**Elements:**
- Original `{Prop}Flg` property with string type
- Alias `Is{Prop}Flg` property with bool type that converts Y/N

#### 3.5 Composite Table Property (Row1)
```csharp
// Properties from a second table (e.g., entity_address joined to business_entity)
[Bindable(true)]
[DatabaseFieldAttribute("entity_address", "addr_no", false, false, false, false, 10, false)]
[ValidateRequired]
public virtual System.Int32 AddrNo {
    get { return Row1.addr_no; }  // Note: Row1 instead of Row
    set { 
        errors.Remove("AddrNo");
        Row1.addr_no = value;
        AddContainer();
    }
}
```

**Elements:**
- Uses `Row1` for second table data
- Initialization creates `Rowset1` and `Row1` for second table

---

### 4. Relationship Patterns

#### 4.1 Child List (One-to-Many, Owner)
```csharp
protected OrderItemList orderitems;

[ChildList]
[Bindable(true)]
[BORelation("OrderHeader_OrderItem_REF")]
public virtual OrderItemList OrderItems 
{
    get 
    {
        if (this.orderitems == null) 
        {
            if (this.IsNew)
                return this.orderitems = OrderItemList.NewInstance();

            try 
            {
                if (UtilityData.IsEmpty(this.OrderNo)) 
                {
                    throw new BOException("Foreign key is not initialized.");
                }
            }
            catch (System.Exception) 
            {
                return OrderItemList.NewInstance();
            }
            
            OrderItem bo = OrderItem.NewInstance();
            bo.ShareContext(this);
            bo.OrderNo = this.OrderNo;

            this.orderitems = new OrderItemList();
            this.orderitems.AddRange(bo.Query(bo.GetCriteria()));
        }
        return this.orderitems;
    }
    set { this.orderitems = value; }
}
```

**Elements:**
- `[ChildList]` attribute
- `[BORelation("RelationName")]` attribute
- Lazy loading with null check
- Context sharing via `ShareContext(this)`
- Query via foreign key

#### 4.2 Parent Object (Many-to-One, FK Reference)
```csharp
protected Customer customer;

[ParentObject]
[Bindable(true)]
[BORelation("CustomerList")]
public virtual Customer Customer
{
    get 
    {
        if (this.customer == null) 
        {
            if (UtilityData.IsEmpty(this.BusinessNo)) 
            {
                return Customer.NewInstance();
            }
            Customer bo = Customer.NewInstance();
            bo.CustomerNo = this.BusinessNo;
            bo.ScopeFilter = nameof(kssf_scope_all);
            this.customer = (Customer)bo.Fetch();
        }
        return this.customer;
    }
    set { this.customer = value; }
}
```

**Elements:**
- `[ParentObject]` attribute
- `[BORelation("RelationName")]` attribute
- Lazy loading via `Fetch()`
- Scope filter setting

#### 4.3 Cross-Domain Parent
```csharp
protected PaymentTerms paymentterms;

[ParentObject]
[Bindable(true)]
[BORelation("PaymentTermsList")]
public virtual Koretech.KommerceServer.BusinessObjects.PaymentTerm.PaymentTerms PaymentTerms
{
    get 
    {
        if (this.paymentterms == null) 
        {
            if (UtilityData.IsEmpty(this.PaymentTermSeq)) 
            {
                return PaymentTerms.NewInstance();
            }
            PaymentTerms bo = PaymentTerms.NewInstance();
            bo.PaymentTermSeq = this.PaymentTermSeq.Value;
            bo.ScopeFilter = nameof(kssf_scope_all);
            this.paymentterms = (PaymentTerms)bo.Fetch();
        }
        return this.paymentterms;
    }
    set { this.paymentterms = value; }
}
```

---

### 5. Composite Item Patterns

#### 5.1 Save All Composite Items
```csharp
protected override void SaveAllCompositeItems()
{
    if (this.orderitems != null)
    {
        this.orderitems.ForEach(i => i.Save());
    }
    if (this.orderspecialcharges != null)
    {
        this.orderspecialcharges.ForEach(i => i.Save());
    }
}
```

#### 5.2 Update Composite Item Keys
```csharp
protected override void UpdateAllCompositeItemKeys()
{
    if (this.orderitems != null)
    {
        this.orderitems.ForEach(i => { i.OrderNo = this.OrderNo; });
    }
    if (this.orderspecialcharges != null)
    {
        this.orderspecialcharges.ForEach(i => { i.OrderNo = this.OrderNo; });
    }
}
```

---

### 6. Super-Domain Patterns (Partitioning)

#### 6.1 Partition Property
```csharp
public override string PartitionProperty { get { return "BusinessType"; } }
```

#### 6.2 Sub-Domain Objects List
```csharp
private IList<IDomainSub> _subDomainObjects;
public override IList<IDomainSub> SubDomainObjects 
{ 
    get
    {
        if (this._subDomainObjects == null)
        {
            this._subDomainObjects = new List<IDomainSub>();
            this._subDomainObjects.Add(BOCustomer.NewInstance());
            this._subDomainObjects.Add(BOProspect.NewInstance());
            this._subDomainObjects.Add(BOEndUser.NewInstance());
            this._subDomainObjects.Add(BOVendor.NewInstance());
        }      
        return this._subDomainObjects;
    }    
}
```

#### 6.3 Union Query Across Partitions
```csharp
public override IQueryable<BusinessEntity> Query()
{
    return 
        ((from o1 in Customer.NewQuery
            select new BusinessEntity
            {
                BusinessNo = o1.CustomerNo,
                BusinessName = o1.CustomerName,
                // ... property mappings
                BusinessType = "C"
            }
        ).AsEnumerable())
    .Union 
        ((from o2 in Prospect.NewQuery
            select new BusinessEntity
            {
                BusinessNo = o2.BusinessNo,
                // ... property mappings
                BusinessType = "P"
            }
        ).AsEnumerable())
    // ... more unions
}
```

---

### 7. Sub-Domain Patterns

#### 7.1 Super Domain Reference
```csharp
public new BODomainSuper<BusinessEntity> SuperDomain
{
    get { return BOBusinessEntity.NewInstance(); }
}
```

#### 7.2 Partition Value
```csharp
public override object PartitionValue 
{ 
    get { return "C"; }  // "C" for Customer, "P" for Prospect, etc.
}
```

---

### 8. Factory and Query Patterns

#### 8.1 NewInstance Factory
```csharp
public static PaymentTerms NewInstance() 
{
    return (PaymentTerms)new PaymentTerms();
}
```

#### 8.2 NewQuery Static Property
```csharp
public static IQueryable<PaymentTerms> NewQuery
{
    get
    {
        return PaymentTerms.NewInstance().Query();
    }
}
```

#### 8.3 GetDomain Method
```csharp
public override IDomainObject GetDomain()
{
    return BOPaymentTerm.NewInstance();
}
```

#### 8.4 GetDomainName Method
```csharp
public override string GetDomainName()
{
    return "PaymentTerm";
}
```

---

### 9. Initialization Patterns

#### 9.1 Simple Init (Single Table)
```csharp
protected virtual void init(BODataAccessEvent e) 
{
    Rowset = new ScopedRepository<Data.payment_terms, kssf_scope_allornone>();
    Rowset.ScopeContext = this;
    Row = new Data.payment_terms();
    
    if (ObjDbMap == null)
    {
        lock (_lockObject)
        {
            if (ObjDbMap == null)
            {
                AlternateDbToPropertyNameMap = new Dictionary<string, string>();
                Rowset.AlternateDbToPropertyNameMap = AlternateDbToPropertyNameMap;
                ObjPropMap = Rowset.PropMap;
                ObjKeyMap = Rowset.KeyMap;
                ObjDbMap = Rowset.DbMap;
            }
        } 
    }
    Rowset.AlternateDbToPropertyNameMap = AlternateDbToPropertyNameMap;
    this.SetPropMap(ObjPropMap);
    this.SetKeyMap(ObjKeyMap);
    this.SetDbMap(ObjDbMap);

    this.CreateInstance();
}
```

#### 9.2 Composite Init (Multiple Tables)
```csharp
protected virtual void init(BODataAccessEvent e) 
{
    // Primary table
    Rowset = new ScopedRepository<Data.business_entity, Data.kssf_scope_Customer>();
    Row = new Data.business_entity();
    
    // Secondary table (composite)
    Rowset1 = new ScopedRepository<Data.entity_address, kssf_scope_all>();
    Row1 = new Data.entity_address();
    
    // ... mapping setup
}
```

---

### 10. Extended Properties Patterns

#### 10.1 Extended Properties Support
```csharp
public override bool SupportsExtendedProperties => true;

protected override ExtendedPropertyList ReadExtendedProperties()
{
    return ReadExtendedProperties<Customer>();
}

protected override PropertyDescriptor[] GetPropertyDescriptors()
{
    return base.GetPropertyDescriptors<Customer>();
}

public override PropertyInfo[] GetProperties(bool includeChildListOrParentObjectProps)
{
    return GetProperties<Customer>(includeChildListOrParentObjectProps);
}

public override PropertyInfo[] GetPropertiesBindable()
{
    return GetPropertiesBindable<Customer>();
}

public override bool IsObjectProperty(string propertyName)
{
    return base.IsObjectProperty<Customer>(propertyName);
}
```

---

### 11. Constants Patterns

#### 11.1 Property Name Constants
```csharp
public static string PAYMENTTERMSEQ = "PaymentTermSeq";
public static string TERMTYPE = "TermType";
public static string TERMSXREFID = "TermsXrefId";
```

#### 11.2 Column Name Constants
```csharp
public static string COLPAYMENTTERMSEQ = "payment_term_seq";
public static string COLTERMTYPE = "term_type";
public static string COLTERMSXREFID = "terms_xref_id";
```

#### 11.3 Label Constants
```csharp
public static string NAMEPAYMENTTERMSEQ = "Payment Term Seq";
public static string NAMETERMTYPE = "Term Type";
```

---

### 12. List Class Patterns

#### 12.1 BOList Implementation
```csharp
[Serializable]
public partial class OrderHeaderList : BOList<OrderHeader>
{
    public new static OrderHeaderList NewInstance()
    {
        return new OrderHeaderList();
    }            

    public new OrderHeaderList Retrieve(object criteria)
    {
        return base.Retrieve(criteria) as OrderHeaderList;
    }

    public new OrderHeader Fetch(object criteria)
    {
        return base.Fetch(criteria) as OrderHeader;
    }

    override public BOBase NewItemInstance() 
    {
        return OrderHeader.NewInstance();
    } 
}
```

---

### 13. Serialization Patterns

#### 13.1 ToXML Method
```csharp
public override string ToXML(Dictionary<string, string> attributes, XmlTextWriter xmlWriter, 
    IList<BOBase> ancestorList, string tagName)
{
    // ... full XML serialization of all properties
    xmlWriter.WriteElementString(PAYMENTTERMSEQ, string.Format("{0}", this.PaymentTermSeq));
    xmlWriter.WriteElementString(TERMTYPE, string.Format("{0}", this.TermType));
    // ...
}
```

---

### 14. Application and Scope Patterns

#### 14.1 GetApplications
```csharp
override public List<int> GetApplications()
{
    List<int> appList = new List<int>();
    appList.Add(6);  // webTeam
    appList.Add(3);  // webSales
    appList.Add(2);  // webPortal
    appList.Add(5);  // webServiceCenter
    appList.Add(1);  // webStore
    return appList;
}
```

#### 14.2 GetScopes
```csharp
override public List<string> GetScopes()
{
    List<string> scopeList = new List<string>();
    scopeList.Add("REPCOMPANY");
    scopeList.Add("MASTERCOMPANY");
    scopeList.Add("COMPANY");
    scopeList.Add("ALL");
    scopeList.Add("NONE");
    return scopeList;
}
```

---

## Summary: Kraken Generator Requirements

To achieve full compatibility, the Kraken `BOWrapperFileGenerator` must support:

| Category | Patterns | Complexity |
|----------|----------|------------|
| **Class Declarations** | Simple, Extended, SuperDomain, SubDomain | Medium |
| **Interfaces** | Entity, Domain | Low |
| **Properties** | Simple, Required, Nullable, Boolean, Composite | Medium |
| **Relationships** | ChildList, ParentObject, CrossDomain | High |
| **Composites** | SaveAll, UpdateKeys | Medium |
| **Partitioning** | SuperDomain Query, SubDomain Value | High |
| **Factories** | NewInstance, NewQuery, GetDomain | Low |
| **Initialization** | Single Table, Composite | Medium |
| **Extended Props** | Full support | Medium |
| **Constants** | Property, Column, Label | Low |
| **Lists** | BOList implementation | Low |
| **Serialization** | ToXML | Medium |
| **App/Scope** | Applications, Scopes | Low |

### Priority Order for Implementation

1. **Phase 1 (Core)**: Simple properties, factories, initialization, lists
2. **Phase 2 (Relationships)**: Child lists, parent objects, cross-domain
3. **Phase 3 (Advanced)**: Composite tables, partitioning, extended properties
4. **Phase 4 (Compatibility)**: Constants, serialization, app/scope metadata

---

## Implementation Effort Estimates

### Phase 1: Core (Simple Domains)
**Scope**: Simple properties, factories, initialization, lists, interfaces, constants

| Task | Effort | Notes |
|------|--------|-------|
| Create `BOEntityEF<>` base class | 3-4 days | New framework class replacing `BOEntity<>` for EF |
| Create `BOWrapperFileGenerator` skeleton | 2 days | Scaffold based on existing `EntityFileGenerator` |
| Implement simple property generation | 3-4 days | Properties wrapping EF entity, validation attributes |
| Implement `IsNull{Prop}()` methods | 1 day | For nullable properties |
| Implement boolean flag aliases (`Is{Prop}Flg`) | 1 day | Y/N to bool conversion |
| Implement factory methods (`NewInstance`, `NewQuery`) | 1-2 days | Static factories |
| Implement `GetDomain`, `GetDomainName` | 0.5 days | Simple methods |
| Create `BOListFileGenerator` | 2 days | `BOList<T>` typed lists |
| Create `BOInterfaceFileGenerator` | 1-2 days | `I{Entity}` interfaces |
| Implement constants generation | 1 day | Property, column, label constants |
| Implement initialization (`Init`) | 2 days | EF repository setup |
| Unit tests for Phase 1 | 3-4 days | Test simple domain generation |
| Integration test with PaymentTerm | 2-3 days | End-to-end validation |
| **Phase 1 Total** | **~22-28 days (4-5 weeks)** | |

**Domains Supported**: ~40-50% (simple single-table domains like PaymentTerm, TaxCode, Currency, etc.)

---

### Phase 2: Relationships
**Scope**: Child lists, parent objects, cross-domain relationships, composite item handling

| Task | Effort | Notes |
|------|--------|-------|
| Implement child list properties | 3-4 days | Lazy loading, context sharing, `[ChildList]` attribute |
| Implement parent object properties | 2-3 days | Lazy loading via `Fetch()`, `[ParentObject]` attribute |
| Implement cross-domain parent references | 2 days | Different namespaces, scope handling |
| Implement `SaveAllCompositeItems()` | 1-2 days | Cascade saves |
| Implement `UpdateAllCompositeItemKeys()` | 1 day | FK propagation |
| Implement `ResetChildren()` | 0.5 days | Clear lazy-loaded lists |
| Update `BOEntityEF<>` for relationships | 2 days | Support relationship patterns |
| Unit tests for relationships | 3-4 days | |
| Integration test with Order domain | 3-4 days | Order → OrderItems, OrderSpecialCharges |
| **Phase 2 Total** | **~18-22 days (3-4 weeks)** | |

**Domains Supported**: ~75-80% (Order, ItemMaster, most domains with parent-child relationships)

---

### Phase 3: Advanced (Composite Tables, Partitioning)
**Scope**: Multi-table composites (Row/Row1), super/sub domain partitioning

| Task | Effort | Notes |
|------|--------|-------|
| Implement composite table support (`Row1`) | 4-5 days | Multiple EF entities per BO, complex |
| Implement composite initialization | 2 days | Multiple repositories/entities |
| Create `BODomainFileGenerator` | 2-3 days | For `BO{Domain}_gen.cs` files |
| Implement super-domain (`BODomainSuper<>`) | 3-4 days | `PartitionProperty`, `SubDomainObjects` |
| Implement union query across partitions | 4-5 days | Complex LINQ union with projections |
| Implement sub-domain (`BODomainSub<>`) | 2 days | `SuperDomain`, `PartitionValue` |
| Update kamlbo model for partition metadata | 2 days | Need to capture super/sub relationships |
| Unit tests for composites | 3-4 days | |
| Unit tests for partitioning | 2-3 days | |
| Integration test with Customer/BusinessEntity | 4-5 days | Most complex domain |
| **Phase 3 Total** | **~28-35 days (5-7 weeks)** | |

**Domains Supported**: ~95% (BusinessEntity, Customer, Prospect, EndUser, Vendor)

---

### Phase 4: Compatibility (Full Feature Parity)
**Scope**: Extended properties, serialization, app/scope metadata

| Task | Effort | Notes |
|------|--------|-------|
| Implement extended properties support | 3-4 days | `IExtendable`, property descriptors |
| Implement `ToXML` serialization | 2-3 days | XML serialization methods |
| Implement `GetApplications()` | 0.5 days | App list metadata |
| Implement `GetScopes()` | 0.5 days | Scope list metadata |
| Implement `GetKeyValue()` | 0.5 days | Key access method |
| Final cleanup and edge cases | 2-3 days | |
| Full regression testing | 4-5 days | All domains |
| Documentation | 1-2 days | |
| **Phase 4 Total** | **~14-19 days (3-4 weeks)** | |

**Domains Supported**: 100%

---

## Effort Summary

| Phase | Scope | Effort | Cumulative | Coverage |
|-------|-------|--------|------------|----------|
| **Phase 1** | Core | 4-5 weeks | 4-5 weeks | ~40-50% |
| **Phase 2** | Relationships | 3-4 weeks | 7-9 weeks | ~75-80% |
| **Phase 3** | Composites/Partitioning | 5-7 weeks | 12-16 weeks | ~95% |
| **Phase 4** | Full Compatibility | 3-4 weeks | 15-20 weeks | 100% |

---

## Recommendations

### Minimum Viable Implementation
If you want to migrate **most** domains with reasonable effort:

**Phases 1 + 2 = 7-9 weeks** → Covers ~75-80% of domains

This would leave BusinessEntity/Customer/Prospect/EndUser/Vendor for later but allow migration of Order, ItemMaster, PaymentTerm, and most other domains.

### Risk Mitigation
- **Start with Phase 1** on a single simple domain (PaymentTerm) to validate the approach
- **Phase 2** adds significant value - most domains have relationships
- **Phase 3** is the most complex - consider deferring composite/partitioned domains if timeline is tight
- **Phase 4** can be deferred if extended properties and XML serialization aren't heavily used

### Parallel Work Opportunity
While building the generator, you could:
- Convert kamlbo files to Kraken format in parallel (~1 week for all 69 domains)
- Create the `BOEntityEF<>` base class independently (~1 week)
