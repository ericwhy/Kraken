# Slice-Based Migration Approach

## Executive Overview

This document defines a **slice-based migration approach** for moving Kore Commerce to 100% .NET Core while minimizing breakage and maintaining steady delivery.

The approach migrates business capabilities one slice at a time across four coordinated activity types:

1. Workflow API migration.
2. BO CRUD migration (only where needed by the migrated workflow).
3. Business logic/service migration for the same slice.
4. UI migration for only the entry points that use the migrated slice.

### Benefits

1. Reduces blast radius by migrating bounded capabilities instead of entire layers at once.
2. Delivers visible progress each phase without waiting for big-bang completion.
3. Avoids unnecessary BO CRUD rewrites by tying CRUD effort to workflow needs.
4. Improves rollback safety because cutovers are route/action scoped.

### Primary Risks

1. Dual-path complexity during transition.
2. Contract drift between legacy and new slice behavior.
3. Performance regressions if boundaries become chatty.
4. Sequence risk when slices have hidden cross-domain dependencies.

### How This Gets Us to 100% .NET Core

1. Each completed slice moves a real business path end-to-end to .NET Core.
2. Repeating slices progressively drains legacy runtime responsibility.
3. Once all high-traffic entry points have been strangled over, remaining legacy paths can be retired with low risk.

---

## Scope and Non-Goals

### Scope

1. Migration of business capabilities by vertical slice.
2. Coordinated movement of workflow/API, selective CRUD, service logic, and UI entry points.
3. Incremental runtime coexistence with controlled cutover.

### Non-Goals

1. Full BO CRUD migration before workflow migration.
2. Full UI rewrite up front.
3. Immediate removal of all legacy BO/runtime code in early phases.

---

## Activity Types (Developer Guidance)

## 1) Workflow API Migration

Purpose:

1. Establish authoritative transaction boundaries in .NET Core per business command.

When to do it:

1. First in each slice.

Expected outputs:

1. Endpoint contract for the workflow command.
2. Idempotent mutation semantics.
3. Error and status model aligned with existing user flows.

Reference:

1. `Kore Commerce Transactional Workflow API Endpoints.md`

## 2) BO CRUD Migration (Adjunct Only)

Purpose:

1. Provide supporting data operations needed by the migrated workflow.

When to do it:

1. Only when required by the active workflow slice.

Expected outputs:

1. Minimal CRUD APIs/repositories for slice-critical entities.
2. No broad CRUD migration for unrelated domains.

Reference:

1. BO migration concepts in `Kore Commerce EF Phase 1.md` (Option B/B2 discussion).

### BO Migration Pattern: API-Backed BO Facade

Default pattern for legacy BO migration in this program:

1. Legacy BO public shape remains stable (compatibility first).
2. BO persistence operations are redirected to .NET Core CRUD APIs backed by generated EF Core persistence.
3. Legacy product does **not** introduce EF6 as an intermediate persistence target.
4. For multi-object transactional behavior, use workflow endpoints instead of composing many remote CRUD calls.
5. Migrate BOs to CRUD API only when required by an active slice or to remove repeated legacy coupling in that slice.

## 3) Slice Business Logic / Service Migration

Purpose:

1. Move workflow-specific rules and orchestration to .NET Core service layer.

When to do it:

1. After workflow contract exists.
2. In the same phase as the workflow cutover.

Expected outputs:

1. Domain service(s) for that workflow in .NET Core.
2. Legacy wrappers/adapters reduced to pass-through behavior.

## 4) UI Migration

Purpose:

1. Move only the entry points that invoke the migrated workflow slice.

When to do it:

1. After workflow + service path is testable.

Expected outputs:

1. Route/page/action-level cutover to new slice endpoint.
2. Feature-flagged fallback to legacy path during stabilization.

---

## Definition of Done Per Slice

A slice is complete only when all conditions below are met:

1. Workflow endpoint implemented and production-configurable.
2. Required adjunct CRUD implemented for slice scope only.
3. Slice business logic/service moved to .NET Core and exercised by endpoint.
4. Target UI entry points switched to new slice path (with rollback switch).
5. Parity tests pass for normal and failure paths.
6. Performance meets agreed baseline/SLO thresholds.
7. Operational telemetry confirms stable behavior through soak window.

---

## Readiness and Exit Gates Per Phase

## Phase Readiness (Before Starting a Phase)

1. Scope and dependency map approved.
2. Contracts and test plan approved.
3. Rollback path verified in lower environment.

## Phase Exit (Before Advancing)

1. Slice DoD fully satisfied for all phase slices.
2. Open defects are below agreed severity threshold.
3. Performance and error-rate telemetry are within target bands.
4. Rollback drill executed successfully at least once for the phase.

---

## Testing and Verification Strategy

1. Contract tests for each workflow endpoint.
2. Parity tests comparing legacy vs new outcomes for core scenarios.
3. Failure-injection tests at transaction and external-call boundaries.
4. Performance verification for P50/P95 latency and error rates.
5. End-to-end UI flow tests for migrated entry points.

---

## Dependency and Coupling Map

This map drives sequencing and risk control.

| Slice | Key Dependencies | Coupling Level | Sequencing Note |
|---|---|---|---|
| Shop list from cart/order | Cart context, list persistence | Low | Ideal pilot slice |
| Sample request intake | Account ensure service | Low-Medium | Can run early after pilot |
| Account ensure | Contact/customer creation, identity linkage | Medium | Dependency reducer for later slices |
| Sample request to order | Sales order creation path | Medium | Sequence after account ensure readiness |
| Sales order approval/abandon | Contact/customer/user/address/cart cleanup | Medium-High | Migrate after order services stabilize |
| Checkout submit/order create | Cart, quote/order conversion, payment/provider, background processing | Very High | Defer until lower-coupling slices are proven |
| Account onboarding flows | Session/verification + identity + BO interactions | Very High | Migrate after platform patterns harden |
| Payment apply/invoice close | DataAwareContext secondary instance behavior | Very High | Treat as dedicated workflow migration, not generic CRUD |

Reference:

1. Coupling details in `Kore Commerce Transactional Workflow API Endpoints.md` (Vertical Slice Coupling Matrix).

---

## Environment and Deployment Strategy

1. Maintain dual-path runtime during migration (legacy and .NET Core slice paths).
2. Use route/action-level feature flags for each migrated slice.
3. Promote slices through Dev -> Test -> Stage -> Prod with same flags and telemetry wiring.
4. Require canary/limited rollout for high-coupling slices.
5. Keep rollback immediate by toggling route/action flags, not redeploying binaries.

---

## Suggested Phased Rollout Plan

| Phase | Workflow Focus | Key BOs | Key Services | Slice Points (Cut Locations) | Key UI/Entry Points |
|---|---|---|---|---|---|
| 0 Foundation | Standards + platform seam setup | None (infra prep) | Workflow client abstraction, telemetry, feature flag plumbing | `ICartService`, `IAccountRequestService`, `ISalesOrderService` seams | None (infra only) |
| 1 Pilot | Create shop list from cart | `UserShopList`, `ShopListItem`, `ShopCart` | User shop list + cart read services | `ShopCart_Listing.aspx.cs -> NavToCreateShopList` | `ShopCart_Listing` save-list action |
| 2 Early Low/Medium | Request intake + account ensure + sample-to-order | `Request`, `SalesOrderHeader`, `Contact`, `Customer` | Request service, account ensure service, sales order creation service | `Request_For_Sample` command; `RequestForSampleToOrderServiceController` | Request page submit, sample-to-order API |
| 3 Order Admin | Approve/abandon pending sales orders | `SalesOrderHeader`, `Contact`, `Customer`, `KSUser`, `EntityAddress` | Sales order admin workflow service | `SalesOrder_BC_Approval` command methods | Approval page buttons (`Approve`, `Abandon`) |
| 4 Checkout Core | Submit order + authorize return + completed cart processing | `ShopCart`, `QuoteHeader`, `SalesOrderHeader`, `CCAuthorize`, `WebOrder` | Checkout workflow service, cart/order orchestration, payment adapter services | `CheckoutContext.SubmitOrder`; `CheckoutController`; `CartController.ProcessCompletedCart`; background cart processor | Checkout Review/Payment/Single-page checkout flows; completed-cart processing path |
| 5 High-Coupling Follow-On | Account onboarding and payment apply workflows | `AccountRequest`, `Contact`, `Customer`, `KsUser`, `Payment`, `InvoicePayment`, `Invoice` | Account onboarding workflows, payment-apply workflows | `AccountRequest` workflow methods; payment workflow entry replacing context-dependent save behavior | Account registration pages, payment/invoice payment entry points |
| 6 Completion | Remaining long-tail slices + final legacy retirement | Residual BOs by domain | Residual domain services | Remaining legacy route/page cutovers | Remaining legacy UI/routes |

---

## Related Documents

1. `Kore Commerce Transactional Workflow API Endpoints.md`
2. `Kore Commerce EF Phase 1.md`

