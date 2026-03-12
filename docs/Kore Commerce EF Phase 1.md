# Kore Commerce EF Migration (Phase 1)

## Background
Kore Commerce (previously known as KommerceServer) is a web-based storefront that is used by manufacturers and distributors to sell products to their customers.  Its current implementation is C# on the latest version .NET Framework.  As part of a project called Kraken, we wish to migrate Kore Commerce to the latest version of .NET Core.  Part of this migration is replacing the existing persistence mechanism which is based on SubSonic with a new Entity Framework based approach.

## Architecture
The existing Kore Commerce architecture uses a set of generated business objects to manage data, some business logic, and persistence.  These business objects support CRUD operations, i.e. they can "save themselves".  Long term, the desire is to move away from this to a model in which there is a persistence service layer which sits outside of the business objects and performs CRUD operations on them.  A significant challenge is refactoring the business objects and persistence logic to separate them without having to essentially rewrite a large part of the application.

## Analysis Needed
By examining the existing Kore Commerce services, business objects and database access mechanisms, we wish to develop one or more possible approaches to the refactoring described above.  Of particular interest would be a "Phase 1" approach that would be part of a multi-phase solution, which would replace SubSonic with EF while disrupting the existing code base as little as possible.  The goal of this first phase would not be to implement the long term persistence service layer unless that could be done in a minimally disruptive way.  

### Project Kraken
The code-generated persistence approach of project Kraken has already been described in 'Kraken-Importer Gap Analysis.md'.  This should be reviewed before proceeding.

It is expected that the approach to Phase 1 will involve the use of Kraken kamlbo and the code generation capabilities being developed for Kraken.  It could be, however, that not all of the code products currently being generated are used in Phase 1.

### Code Locations
Examine the code in these locations in order to analyze it and posit possible approaches for Phase 1.

 - - Base path to repositories: 'C:\Dev\Source\repos\' (All paths below are relative to this base path)
- Kraken code generator: `Kraken\Koretech.Tools.KrakenGenerator`
- Kraken kamlbo samples: 'Kraken\Koretech.Kraken.Data'
- Kraken persistence layer: `KommerceServer-Kraken\Koretech.Domains`
- Existing Kore Commerce business objects: `KommerceServer-Kraken\Koretech.KommerceServer\BusinessObjects`

---

# Phase 1 Analysis Results

## Executive Summary

After examining the existing Kore Commerce architecture, I've identified **four possible approaches** for Phase 1, ranging from minimal disruption to full architectural modernization. Each approach offers different tradeoffs between risk, effort, and long-term benefit. Option B has two sub-variants (B1 and B2) that differ in which code generator is modified.

**Recommended Approach**: **Option B2 (Extend Kraken Generator for BO Wrappers)** - It consolidates all code generation in Kraken, reduces long-term maintenance burden, and provides the best balance of minimal disruption while establishing a clean foundation for future modernization.

---

## Current Architecture Analysis

### Business Object Structure

The existing architecture follows a **generated Active Record pattern** with these key components:

```
BusinessObject (e.g., PaymentTerms)
    ↓ inherits from
BusinessObject_gen (e.g., PaymentTerms_gen)  [Generated]
    ↓ inherits from
BOEntity<TObject, TData, TScope, TList>      [Framework]
    ↓ inherits from
BOBase                                        [Framework]
```

### Key Characteristics

| Aspect | Current Implementation |
|--------|------------------------|
| **Persistence Pattern** | Active Record ("save yourself") |
| **Data Access** | SubSonic via `IScopedRepository<T, S>` |
| **Data Objects** | SubSonic-generated `Data.table_name` classes |
| **Query Mechanism** | `NewQuery` static property → LINQ-style queries |
| **Property Access** | BO properties wrap `Row.column_name` (SubSonic row) |
| **Transactions** | `ITransactionManager` wrapping SubSonic transactions |
| **Scoping** | Custom scope filters (`kssf_scope_all`, etc.) |
| **Business Logic** | Mixed - some in generated classes, some in partial classes |

### SubSonic Integration Points

The SubSonic integration is concentrated in these areas:

1. **`IScopedRepository<T, S>`** - Repository interface extending SubSonic's `IRepository<T>`
2. **`ScopedRepository<T, S>`** - Implementation using SubSonic `SqlQuery`, `Select`, etc.
3. **`Data.table_name`** - SubSonic-generated data classes (e.g., `Data.payment_terms`)
4. **`BOEntity.Row`** - Each BO holds a SubSonic data row
5. **`BOBase.Save()`** - Calls `Create()`, `Update()`, or `Delete()` which use `Rowset.Add()`, `Rowset.Update()`, `Rowset.Delete()`

### Scale Assessment

| Metric | Count |
|--------|-------|
| Business Object Domains | 69 |
| Total Business Objects | ~150-200 (estimated) |
| Simple BOs (little/no logic) | ~60-70% |
| Complex BOs (significant logic) | ~30-40% |

---

## Migration Approaches

### Option A: Minimal Touch - EF as SubSonic Replacement

**Concept**: Replace SubSonic internals with EF while keeping the `IScopedRepository` interface intact.

#### What Changes
- Create EF versions of `Data.table_name` classes (EF entities)
- Implement `ScopedRepository<T,S>` using EF `DbContext` instead of SubSonic
- No changes to generated BO classes or consuming code

#### Architecture
```
Existing BO Code (unchanged)
    ↓
BOEntity<TObject, TData, TScope, TList> (unchanged)
    ↓
IScopedRepository<T, S> (unchanged interface)
    ↓
ScopedRepositoryEF<T, S> (NEW - EF implementation)
    ↓
DbContext + EF Entities (NEW)
```

#### Pros
- **Minimal disruption** - No changes to BO classes or consuming code
- **Quick win** - Can migrate incrementally, one domain at a time
- **Low risk** - Existing tests continue to work
- **Parallel operation** - SubSonic and EF can coexist

#### Cons
- **Technical debt** - Preserves the Active Record anti-pattern
- **Complex adapter** - `IScopedRepository` has SubSonic-specific methods that are hard to map to EF
- **Dual data classes** - Need both `Data.table_name` (SubSonic) and EF entities
- **No .NET Core EF** - SubSonic dependency prevents moving to .NET Core EF

#### Effort Estimate
| Task | Effort |
|------|--------|
| Create `ScopedRepositoryEF<T,S>` adapter | 2-3 weeks |
| Generate EF entities for all domains | 2 weeks |
| Testing and bug fixing | 2-3 weeks |
| **Total** | **6-8 weeks** |

---

### Option B: EF Repository Adapter (Recommended)

**Concept**: Create a new EF-based repository layer and adapt the existing `BOEntity` base class to use it, while keeping BO API unchanged.

There are **two sub-approaches** for implementing this option, differing in which code generator is modified:

---

### Option B1: Modify Existing Kore Commerce Generator

**Concept**: Modify the existing proprietary Kore Commerce code generator to produce BO classes that wrap EF entities instead of SubSonic rows.

#### What Changes
- Generate EF entities and `DbContext` using Kraken
- Create a new `IEFRepository<TEntity>` interface
- Modify `BOEntity` to use `IEFRepository` instead of `IScopedRepository`
- **Modify the existing Kore Commerce generator** to produce EF-compatible property wrappers

#### Architecture
```
Existing BO Code (unchanged)
    ↓
BusinessObject_gen (REGENERATED - wraps EF entity)
    ↓
BOEntity<TObject, TEntity, TScope, TList> (MODIFIED - uses EF repo)
    ↓
IEFRepository<TEntity> (NEW)
    ↓
EFRepository<TEntity> (NEW - Kraken-generated)
    ↓
DbContext + EF Entities (NEW - Kraken-generated)
```

#### Key Insight: Property Mapping

Currently (SubSonic):
```csharp
public virtual string TermType {
    get { return Row.term_type; }    // Row = SubSonic data object (snake_case)
    set { Row.term_type = value; }
}
```

With EF (after modification):
```csharp
public virtual string TermType {
    get { return Entity.TermType; }  // Entity = EF entity (PascalCase)
    set { Entity.TermType = value; }
}
```

#### Pros
- **Moderate disruption** - Only framework and generated code changes
- **No BO API changes** - Consuming code unchanged
- **Clean EF integration** - Uses EF properly, not through adapter
- **.NET Core ready** - Removes SubSonic dependency
- **Incremental** - Can migrate domain by domain
- **Familiar tooling** - Uses existing generator that team knows

#### Cons
- **Proprietary generator** - Requires access to and knowledge of the existing generator
- **Dual generators** - Need both old generator (modified) and Kraken (for EF layer)
- **Generator maintenance** - Two generators to maintain long-term
- **Testing effort** - Each migrated domain needs thorough testing

#### Effort Estimate
| Task | Effort |
|------|--------|
| Modify `BOEntity` for EF | 2 weeks |
| Create `IEFRepository` + `EFRepository` | 1 week |
| Modify existing BO code generator | 2-3 weeks |
| Convert kamlbo files to Kraken format | 1 week |
| Generate EF layer (entities, contexts) | 1 week |
| Regenerate BO classes | 1 week |
| Testing and bug fixing | 3-4 weeks |
| **Total** | **11-14 weeks** |

---

### Option B2: Extend Kraken Generator for BO Wrappers (Recommended)

**Concept**: Extend the Kraken code generator to produce **BO wrapper classes** in addition to the EF persistence layer. These wrappers maintain API compatibility with existing BOs while delegating to EF entities.

#### What Changes
- Generate EF entities and `DbContext` using Kraken (already supported)
- **Add new `BOWrapperFileGenerator`** to Kraken to generate BO classes
- Create new `BOEntityEF<>` base class for EF-backed business objects
- Kraken becomes the single source of truth for both EF layer and BO layer

#### Architecture
```
Existing BO Code (unchanged - partial classes)
    ↓
BusinessObject_gen (NEW - Kraken-generated BO wrapper)
    ↓
BOEntityEF<TObject, TEntity, TList> (NEW - EF-aware base class)
    ↓
Kraken Repository (Kraken-generated)
    ↓
DbContext + EF Entities (Kraken-generated)
```

#### Kraken Output (Per Domain)

| File | Purpose | Status |
|------|---------|--------|
| `Entities/{Entity}Entity.cs` | EF entity class | ✅ Already generated |
| `EntityConfigurations/{Entity}EntityTypeConfiguration.cs` | EF configuration | ✅ Already generated |
| `Repositories/{Domain}Context.cs` | EF DbContext | ✅ Already generated |
| `Repositories/{Entity}Repository.cs` | EF repository | ✅ Already generated |
| **`BusinessObjects/{Domain}/Gen/{Entity}_gen.cs`** | **BO wrapper class** | ❌ NEW |
| **`BusinessObjects/{Domain}/Gen/{Entity}List_gen.cs`** | **BO list class** | ❌ NEW |
| **`BusinessObjects/{Domain}/Gen/I{Entity}.cs`** | **BO interface** | ❌ NEW |

#### Example: Generated BO Wrapper

```csharp
// Generated by Kraken KAML BO Generator
namespace Koretech.KommerceServer.BusinessObjects.PaymentTerm.Gen
{
    public class PaymentTerms_gen : BOEntityEF<PaymentTerms, PaymentTermsEntity, PaymentTermsList>
    {
        protected override void Init()
        {
            Repository = ServiceLocator.Get<PaymentTermsRepository>();
            Entity = new PaymentTermsEntity();
        }

        // Properties wrap EF Entity (PascalCase, same API as before)
        public virtual int PaymentTermSeq {
            get => Entity.PaymentTermSeq;
            set { Entity.PaymentTermSeq = value; MarkDirty(); }
        }

        public virtual string TermType {
            get => Entity.TermType;
            set { Entity.TermType = value; MarkDirty(); }
        }

        // Compatible with existing NewQuery pattern
        public static IQueryable<PaymentTerms> NewQuery {
            get {
                var repo = ServiceLocator.Get<PaymentTermsRepository>();
                return repo.Query().Select(e => FromEntity(e));
            }
        }

        public static PaymentTerms FromEntity(PaymentTermsEntity entity)
        {
            var bo = new PaymentTerms();
            bo.Entity = entity;
            bo.MarkClean();
            bo.MarkOld();
            return bo;
        }
    }
}
```

#### Pros
- **Single generator** - Kraken generates everything (EF + BO layers)
- **No proprietary generator dependency** - Uses open Kraken codebase
- **Consistent code style** - All generated code follows same patterns
- **No BO API changes** - Consuming code unchanged
- **Clean architecture** - Clear separation of EF entities and BO wrappers
- **.NET Core ready** - Removes SubSonic dependency
- **Incremental** - Can migrate domain by domain
- **Future-proof** - Kraken investment benefits both layers

#### Cons
- **Kraken generator work** - Need to add new file generators to Kraken
- **New base class** - Must create `BOEntityEF<>` framework class
- **Testing effort** - Each migrated domain needs thorough testing
- **Slightly more effort** - More Kraken development than B1

#### Effort Estimate
| Task | Effort |
|------|--------|
| Create `BOEntityEF<>` base class | 2 weeks |
| Add `BOWrapperFileGenerator` to Kraken | 2-3 weeks |
| Add `BOListFileGenerator` to Kraken | 1 week |
| Add `BOInterfaceFileGenerator` to Kraken | 0.5 weeks |
| Convert kamlbo files to Kraken format | 1 week |
| Generate full stack (EF + BO) | 1 week |
| Testing and bug fixing | 3-4 weeks |
| **Total** | **10-13 weeks** |

---

### B1 vs B2 Comparison

| Criteria | B1 (Modify Existing Generator) | B2 (Extend Kraken) |
|----------|-------------------------------|-------------------|
| **Generator complexity** | Lower (modify existing) | Higher (new generators) |
| **Long-term maintenance** | Two generators | One generator |
| **Proprietary dependency** | Yes | No |
| **Kraken investment** | Partial | Full |
| **Code consistency** | Mixed styles | Unified style |
| **Effort** | 11-14 weeks | 10-13 weeks |
| **Risk** | Medium | Medium |
| **Future flexibility** | Moderate | High |

**Recommendation**: **Option B2** is preferred because it consolidates all code generation in Kraken, reduces long-term maintenance burden, and fully leverages the Kraken investment.

---

### Option C: Dual-Layer Coexistence

**Concept**: Keep existing BOs with SubSonic, but add new Kraken-generated persistence services alongside them. Gradually migrate consuming code to use the new services.

#### What Changes
- Generate complete Kraken persistence layer (entities, contexts, repositories, services)
- Generate Kraken business objects (separate from existing BOs)
- Add new service interfaces to DI container
- Migrate consuming code incrementally

#### Architecture
```
Existing Architecture (unchanged, deprecated)    New Architecture (Kraken)
┌─────────────────────────────┐                 ┌─────────────────────────────┐
│ Existing BO (PaymentTerms)  │                 │ Kraken BO (PaymentTerms)    │
│     ↓                       │                 │     ↓                       │
│ SubSonic Repository         │                 │ Kraken Service              │
│     ↓                       │                 │     ↓                       │
│ SubSonic Data Objects       │                 │ Kraken Repository           │
└─────────────────────────────┘                 │     ↓                       │
                                                │ EF DbContext + Entities     │
                                                └─────────────────────────────┘
```

#### Pros
- **Zero risk to existing code** - Old code continues to work
- **Clean new architecture** - New code uses proper persistence services
- **Gradual migration** - No big bang
- **Future-proof** - Kraken architecture is the target state

#### Cons
- **Dual maintenance** - Two systems to maintain during transition
- **Code duplication** - Similar BOs in two places
- **Long migration** - Every piece of consuming code must be touched
- **Database conflicts** - Both systems writing to same tables

#### Effort Estimate
| Task | Effort |
|------|--------|
| Generate Kraken layer for all domains | 2-3 weeks |
| Set up DI and coexistence | 1 week |
| Migrate consuming code (per domain) | 0.5-1 week each |
| Testing | 2-4 weeks |
| **Total for infrastructure** | **5-8 weeks** |
| **Total including migration** | **20-40+ weeks** (depends on scope) |

---

### Option D: BOEntity Persistence Service Injection

**Concept**: Keep BOEntity structure but inject an `IPersistenceService` that can be either SubSonic or EF. This provides a clean abstraction for future migration.

#### What Changes
- Create `IPersistenceService<TEntity>` interface
- Implement SubSonic version (wrapping existing code)
- Implement EF version (using Kraken-generated layer)
- Modify `BOEntity` to use injected persistence service
- Switch implementations via DI configuration

#### Architecture
```
Existing BO Code (unchanged)
    ↓
BOEntity<TObject, TEntity, TScope, TList> (MODIFIED - uses IPersistenceService)
    ↓
IPersistenceService<TEntity> (NEW - abstraction)
    ↓                           ↓
SubSonicPersistence (wrapper)   EFPersistence (Kraken-generated)
    ↓                               ↓
SubSonic                        EF DbContext
```

#### Pros
- **Clean abstraction** - Persistence is now truly separated
- **Switchable** - Can flip between SubSonic and EF per domain
- **Testable** - Can mock persistence for unit tests
- **Incremental** - Migrate one domain at a time

#### Cons
- **Abstraction overhead** - Additional layer of indirection
- **Complex switchover** - Need to handle both implementations
- **BOEntity changes** - Significant changes to framework code

#### Effort Estimate
| Task | Effort |
|------|--------|
| Create `IPersistenceService` abstraction | 1-2 weeks |
| Create SubSonic wrapper implementation | 2 weeks |
| Create EF implementation | 2 weeks |
| Modify `BOEntity` | 2-3 weeks |
| Testing | 3-4 weeks |
| **Total** | **10-13 weeks** |

---

## Approach Comparison

| Criteria | Option A | Option B1 | Option B2 | Option C | Option D |
|----------|----------|-----------|-----------|----------|----------|
| **Disruption to existing code** | Minimal | Low-Medium | Low-Medium | None | Medium |
| **Disruption to consumers** | None | None | None | High (over time) | None |
| **Effort (weeks)** | 6-8 | 11-14 | 10-13 | 20-40+ | 10-13 |
| **Risk** | Low | Medium | Medium | Low | Medium |
| **Technical debt reduction** | None | Some | Some | High | Medium |
| **Path to .NET Core** | Blocked | Enabled | Enabled | Enabled | Enabled |
| **Path to persistence services** | Hard | Moderate | Moderate | Easy | Easy |
| **Incremental migration** | Yes | Yes | Yes | Yes | Yes |
| **Can run SubSonic + EF side-by-side** | Yes | Per domain | Per domain | Yes | Yes |
| **Single generator** | N/A | No | Yes | Yes | N/A |
| **Proprietary generator dependency** | No | Yes | No | No | No |

---

## Recommendation: Option B2 (Extend Kraken for BO Wrappers)

### Why Option B2?

1. **Single generator** - Kraken generates both EF layer and BO wrappers, eliminating dual-generator maintenance
2. **No proprietary dependency** - All code generation is in the open Kraken codebase
3. **Consistent code style** - All generated code follows the same patterns and conventions
4. **Balanced approach** - Provides meaningful progress without excessive disruption
5. **Enables .NET Core** - Removes SubSonic dependency, unblocking the larger migration
6. **Fully leverages Kraken** - Maximizes the investment in the Kraken code generator
7. **No consumer changes** - BO API remains the same
8. **Incremental** - Can migrate domain by domain, validating as we go
9. **Foundation for future** - Makes future persistence service extraction easier

### Recommended Implementation Plan

#### Phase 1a: Foundation (4-5 weeks)
1. Create `BOEntityEF<>` base class to support EF entities
2. Add `BOWrapperFileGenerator` to Kraken
3. Add `BOListFileGenerator` to Kraken  
4. Add `BOInterfaceFileGenerator` to Kraken
5. Test with one simple domain (e.g., PaymentTerm)

#### Phase 1b: Pilot Migration (3-4 weeks)
1. Convert 5-10 simple domains to EF using Kraken
2. Run in production alongside SubSonic domains
3. Validate performance and correctness
4. Refine generator and patterns based on learnings

#### Phase 1c: Full Migration (4-5 weeks)
1. Convert remaining domains in batches
2. Complex domains (BusinessEntity, Order) done last with extra testing
3. Remove SubSonic dependencies once all domains migrated

### Success Criteria
- All business objects work with EF backend
- No changes to consuming code required
- Performance equal or better than SubSonic
- SubSonic dependency removed
- Solution can target .NET Core

---

## Alternative: Start with Option C for New Development

If there's significant new feature development happening in parallel, consider:

1. Use **Option B** for existing domains
2. Use **Option C** (new Kraken architecture) for **new** domains only
3. Never create new BOs in the old style
4. Over time, the codebase naturally shifts to Kraken architecture

This hybrid approach ensures new code is clean while existing code is stabilized.
