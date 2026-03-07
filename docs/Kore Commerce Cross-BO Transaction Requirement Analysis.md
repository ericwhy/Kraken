# Kore Commerce Cross-BO Transaction Requirement Analysis

## Purpose

Determine whether cross-BO transactional behavior is required in current Kore Commerce code paths, and identify where persistence is performed outside standard BO/SubSonic patterns (including Dapper).

Analyzed codebase: `C:\Dev\Source\Repos\KommerceServer` (focus on `Koretech.KommerceServer` and transaction framework dependencies in `Koretech.Framework`).

---

## Executive Conclusion

Cross-BO transaction requirements **do exist** in current Kore Commerce behavior.

They are present at two levels:

1. **Framework-level data entry pipeline** that intentionally saves a primary BO and multiple secondary BOs in one transaction.
2. **Domain-specific business flows** (order approval/abandonment, account provisioning, payment/invoice application, sample-order conversion) that update multiple BO roots in one logical operation.

There are also several direct Dapper/ADO write paths that are outside typical BO save orchestration; some are manually transactional, some are not.

---

## Method

1. Enumerated explicit transaction starts in non-generated Kommerce BO code.
2. Inspected framework transaction plumbing (`TransactionManager`, `DataTransaction`, `DataAwareContext`, UI save pipeline).
3. Reviewed high-impact BO methods that perform multi-entity updates.
4. Scanned for direct Dapper/KtDatabase/ADO write usage outside BO-generated persistence.

---

## Framework Evidence (Cross-BO by Design)

## 1. UI save pipeline builds a multi-object context

`Toolbar` aggregates primary and secondary objects from multiple form/view data sources before save.

Evidence:
- `Toolbar.cs`: lines `1025, 1118, 1167, 1178` ([Toolbar.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Web\Ui\Controls\Toolbar.cs:1025))

## 2. `DataAwareContext.Complete()` transactionally saves primary + secondary

`Complete()` opens a transaction, executes primary create/update/delete, then saves secondary instances, then commits.

Evidence:
- `DataAwareContext.cs`: lines `218, 227, 266, 278` ([DataAwareContext.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Platform\BOCommon\DataAwareContext.cs:218))

## 3. Transaction manager is ambient/nested scope based

Evidence:
- `TransactionManager.cs`: lines `87, 98, 106` ([TransactionManager.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Platform\Transaction\TransactionManager.cs:87))
- `DataTransaction.cs`: lines `222, 242` ([DataTransaction.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Platform\Transaction\DataTransaction.cs:222))

This is a direct indicator that transaction nesting across layered BO operations is expected behavior.

---

## Confirmed Multi-BO Transaction Flows (Domain Code)

## 1. Sales order approval / abandonment (high confidence cross-BO)

`SalesOrderHeader.ApproveSalesOrder` transaction updates:
- `SalesOrderHeader` list
- `Contact`
- `Customer`
- optional `EntityAddress` deletes and customer cleanup

Evidence:
- `SalesOrderHeader.cs` (approve flow): lines `1554, 1559, 1594, 1603, 1616, 1640, 1652` ([SalesOrderHeader.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:1554))

`SalesOrderHeader.AbandonSalesOrder` transaction deletes/updates:
- Sales order
- Contact and contact classes
- User and user-role artifacts
- Customer and entity addresses
- cart cleanup call (`Domain.DeleteContactCart`)

Evidence:
- `SalesOrderHeader.cs` (abandon flow): lines `1670, 1675, 1684, 1713, 1718, 1721` ([SalesOrderHeader.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:1670))

## 2. Account provisioning flow (user/contact/customer coupling)

`BOAccountRequest` wraps contact/customer operations in transaction for existing/new user account activation paths.

Evidence:
- `BOAccountRequest.cs` (transactional paths): lines `316, 319, 362, 366` ([BOAccountRequest.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\BOAccountRequest.cs:316))
- `BOAccountRequest.cs` (known coordination gap TODO): line `605` ([BOAccountRequest.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\BOAccountRequest.cs:605))

That TODO is direct evidence that multi-store/connection transactional coordination is an active concern in this domain.

## 3. Request -> Sales order conversion

`Request.RequestSampleOrder` opens a transaction and creates a `SalesOrderHeader` with item/delivery/web-order artifacts.

Evidence:
- `Request.cs`: lines `165, 169, 251, 253` ([Request.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Request\Request.cs:165))
- API caller: `RequestForSampleToOrderServiceController.cs` line `21` ([RequestForSampleToOrderServiceController.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer.Web\Areas\Store\Controllers\RequestForSampleToOrderServiceController.cs:21))

## 4. Payment flow updates across BO roots

`Payment.Update` updates payment records, invoice-payment records, and invoice status changes in one logical flow.

Evidence:
- `Payment.cs`: lines `96, 113, 141, 145` ([Payment.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Payment\Payment.cs:96))

Even though this method does not start a local transaction itself, it is designed to run under framework transaction orchestration (`DataAwareContext` + nested BO saves).

---

## Explicit Transaction Count in Non-Generated Kommerce BO Code

Non-generated `BeginTransaction` sites found in `Koretech.KommerceServer`: **13 active** (plus 1 commented historical line).

Primary files:

- `SalesOrderHeader.cs` (3)
- `BOAccountRequest.cs` (2 active + 1 commented)
- `UserShopList.cs` (2)
- `Invoice.cs` (1)
- `Request.cs` (1)
- `Contact.cs` (1)
- `Customer.cs` (1)
- `QuoteHeader.cs` (1)
- `BOCustomerRewardTransaction.cs` (manual `SqlTransaction`, not `ITransactionManager`)

---

## Persistence Outside Normal BO Save Patterns (Dapper/ADO)

Confirmed direct write paths using Dapper/ADO include:

- `ApplicationUserStore.cs`: lines `37, 109` ([ApplicationUserStore.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\Identity\ApplicationUserStore.cs:37))
- `BOCustomerRewardTransaction.cs`: lines `99, 173` ([BOCustomerRewardTransaction.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\CustomerRewardTransaction\BOCustomerRewardTransaction.cs:99))
- `BOCustomerRewardPoints.cs`: line `160` ([BOCustomerRewardPoints.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\CustomerRewardPoints\BOCustomerRewardPoints.cs:160))
- `BORewardProgram.cs`: line `120` ([BORewardProgram.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\RewardProgram\BORewardProgram.cs:120))
- `BOPriceBook.cs`: line `62` ([BOPriceBook.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\PriceBook\BOPriceBook.cs:62))
- `BOPriceList.cs`: line `1644` ([BOPriceList.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\PriceList\BOPriceList.cs:1644))
- `BOShopCart.cs`: line `336` ([BOShopCart.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\ShopCart\BOShopCart.cs:336))

---

## Transaction Boundary Observations Relevant to Migration

## 1. Mixed persistence stacks are already present

The code uses:
- BO/SubSonic transaction manager (`ITransactionManager`)
- direct Dapper/KtDatabase calls
- occasional manual `SqlTransaction`

## 2. Dapper connection path uses framework provider connection

`KtDatabase.GetConnection()` uses `DataProviderHelper().GetSqlConnection()`.

Evidence:
- `KtDatabase.cs`: line `17` ([KtDatabase.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Data\KtDatabase.cs:17))
- `DataProviderHelper.cs`: lines `15, 60` ([DataProviderHelper.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Data\DataProviderHelper.cs:15))
- `DapperContext.cs`: line `10` ([DapperContext.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.Framework\Data\Dapper\DapperContext.cs:10))

Inference: Dapper can align with current connection management in many paths, but this is not universally guaranteed by explicit transaction passing, and existing TODOs indicate known coordination gaps in some flows.

## 3. Not all business operations are atomic end-to-end

Example: order creation and cart finalization are split across boundaries.

Evidence:
- `SalesOrderHeader.cs` (order creation transaction): lines `675, 1093` ([SalesOrderHeader.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:675))
- `BOShopCart.cs` (post-order cart save/delete): lines `2131, 2180, 2184` ([BOShopCart.cs](C:\Dev\Source\Repos\KommerceServer\Koretech.KommerceServer\BusinessObjects\ShopCart\BOShopCart.cs:2131))

This means requirement strength is not binary; some flows demand strong atomicity, others already tolerate staged/compensating behavior.

---

## Decision Impact (EF6-first vs direct out-of-process EF Core)

Based on code evidence:

1. A non-trivial set of workflows depends on in-process, nested, cross-object transaction semantics.
2. Framework save orchestration itself is designed for multi-object transactional commits.
3. Some high-value flows already show transaction coordination fragility when persistence stacks diverge.

Therefore, if going directly to out-of-process EF Core, these cross-BO paths would need explicit redesign (coarse-grained command boundaries, idempotency, compensation, and targeted transactional contracts), not just repository/API substitution.

---

## Recommended Next Validation Slice

To remove remaining uncertainty, run a targeted behavioral validation on 4 workflows:

1. Sales order approval (`ApproveSalesOrder`).
2. Sales order abandonment (`AbandonSalesOrder`).
3. Account creation/linking (`SaveAccountExistingUser` and new-user variants).
4. Payment application updating invoice status (`Payment.Update`).

For each workflow, document:
- entities touched,
- failure injection points,
- expected rollback semantics,
- whether current behavior is all-or-nothing or compensating.
