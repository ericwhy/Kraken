# Kore Commerce Transactional Workflow API Endpoints

## Purpose

Define how to use **transactional workflow API endpoints** in the EF migration approach where BO CRUD goes through an API, and identify where this is easiest/hardest in current Kore Commerce code.

This document includes:

1. What a transactional workflow endpoint is.
2. Which workflows are good candidates, with relative effort/disruption/risk.
3. Three detailed endpoint sketches:
   - one low-effort,
   - one medium-effort,
   - one high-effort.
4. A rollout plan that starts with a pilot aligned to the low-effort sketch.

---

## What It Is

A transactional workflow endpoint is a **single API command** that executes a complete business operation spanning multiple persistence actions and/or multiple BO roots, with one server-owned transaction boundary per command.

This differs from generic CRUD endpoints:

- CRUD endpoints persist one object/aggregate at a time.
- Workflow endpoints persist a business transaction (for example, "approve order", "convert sample request to order", "submit checkout").

Core design rules:

1. Server owns transaction scope per workflow call.
2. Calls are coarse-grained (avoid chatty remote persistence).
3. Idempotency key is supported for retry safety on mutating operations.
4. External side effects (email/payment/provider calls) are explicit in response shape/state.

---

## Current-Code Signals That Matter

1. `Toolbar -> DataAwareContext.Complete()` already commits primary + secondary objects in one transaction.
2. Domain workflows already include explicit `BeginTransaction` usage in non-generated BO code.
3. Some critical paths mix persistence stacks (BO/SubSonic, Dapper/KtDatabase, identity stores), increasing cross-boundary risk.

Representative code locations:

- `Koretech.Framework\Web\Ui\Controls\Toolbar.cs:1025`
- `Koretech.Framework\Platform\BOCommon\DataAwareContext.cs:218`
- `Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:670`
- `Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\BOAccountRequest.cs:605`
- `Koretech.KommerceServer\BusinessObjects\Payment\Payment.cs:96`

---

## Candidate Workflow Matrix (Relative)

Baseline for effort ratio is `1.0x` (easiest candidate).

| Workflow Candidate | Relative Effort | Disruption | Risk | Notes |
|---|---:|---|---|---|
| Customer save + business classes (`SaveWithBusinessClasses`) | 1.0x | Low | Low | Small, explicit transaction, simple write set. |
| Shop list creation from cart/order (`CreateShopList`) | 1.2x | Low | Low | Narrow write scope and clear call sites. |
| Request create with contact/business (`CreateWithContactAndBusiness`) | 1.8x | Low-Medium | Medium | Depends on account-ensure logic. |
| Sample request to sales order (`RequestSampleOrder`) | 2.2x | Medium | Medium | Multi-entity order creation, bounded scope. |
| Payment apply/update (`Payment.Update`) | 3.0x | High | High | Depends on `DataAwareContext.GetAllInstancesOfType(...)`. |
| Sales order approval (`ApproveSalesOrder`) | 3.4x | Medium | High | Multi-root update + conditional cleanup. |
| Sales order abandonment (`AbandonSalesOrder`) | 3.8x | Medium | High | Cascading deletes across contact/user/customer/cart. |
| Account existing-user link + sign-in | 4.0x | Medium-High | High | Identity + contact/customer coordination. |
| Account new-user flows (public/customer/token) | 5.0x | High | Very High | Session + identity + persistence coordination; known transaction gap. |
| Checkout submit/create order (cart completion + order conversion) | 6.5x | High | Very High | Largest surface; payment/provider/email side effects; multiple entry points. |

---

## Discussion Notes

1. Strongest fit for workflow endpoints:
   - order approval/abandon,
   - sample request conversion,
   - account provisioning flows,
   - checkout submit/create order.

2. `Payment.Update` is a special case:
   - behavior depends on context-cached secondary instances, not only DB state.
   - if remoted, payload must explicitly carry applied invoice lines/flags.

3. Checkout is highest-risk because it has:
   - multi-step states (`CompleteCart`, hosted auth return, `CreateOrder`),
   - multiple entry points (`CheckoutController`, `CartController.ProcessCompletedCart`, single-page checkout),
   - external side effects.

---

## Endpoint Sketch 1 (Low Effort)

### Workflow

Create shop list from cart (pilot candidate).

Current core path:

- `Koretech.KommerceServer.Web\Pages\ShopCart_Listing.aspx.cs:67`
- `Koretech.KommerceServer\BusinessObjects\ShopList\UserShopList\UserShopList.cs:226`

### Proposed API

`POST /api/workflows/shoplists/create-from-cart`

Request:

```json
{
  "cartNo": 12345,
  "contactNo": 6789,
  "businessNo": 4321,
  "storeCd": "WEB",
  "idempotencyKey": "f9f7e4e2-4a4f-4a42-9f77-4a9fbc9bc001"
}
```

Response:

```json
{
  "shopListNo": 9876,
  "createdItemCount": 14,
  "warnings": []
}
```

### Required Changes at Impacted Points

1. `ShopCart_Listing.aspx.cs:67`
   - Replace direct `UserShopList.NewInstance().CreateShopList(cart)` with a workflow client call.
   - Keep existing redirect behavior to `ShopList_Copy` using returned `shopListNo`.

2. `UserShopList.cs:226`
   - Keep method as compatibility wrapper initially.
   - Option A: method calls workflow client (strangler).
   - Option B: controller/page calls workflow client directly and method is gradually deprecated.

3. `ShopList_Copy.aspx.cs:25`
   - Optional follow-up: align order-to-list flow to workflow API as well for consistency.

4. Add a workflow API client abstraction in KommerceServer web/domain layer
   - Example: `IWorkflowApiClient.CreateShopListFromCart(...)`.

Why low effort:

- Small, bounded payload and write set.
- Simple success/error semantics.
- Minimal side effects and low transactional ambiguity.

---

## Endpoint Sketch 2 (Medium Effort)

### Workflow

Convert sample request to sales order.

Current core path:

- `Koretech.KommerceServer.Web\Areas\Store\Controllers\RequestForSampleToOrderServiceController.cs:21`
- `Koretech.KommerceServer\BusinessObjects\Request\Request.cs:165`

### Proposed API

`POST /api/workflows/requests/sample-to-order`

Request:

```json
{
  "requestNo": 4567,
  "businessNo": 4321,
  "contactNo": 6789,
  "itemProfileId": "ABC-123",
  "requestQty": 2,
  "shipTo": {
    "name": "Jane Doe",
    "addressLine1": "1 Main St",
    "addressLine2": "",
    "addressLine3": "",
    "city": "Portland",
    "stateProvCd": "OR",
    "zipPostal": "97201",
    "country": "US",
    "phone": "555-555-5555"
  },
  "comment": "Sample request",
  "idempotencyKey": "39a2f202-88b8-4f6e-98d0-6762dbfb8828"
}
```

Response:

```json
{
  "orderNo": 112233,
  "orderXrefId": null,
  "contactNo": 6789,
  "businessNo": 4321
}
```

### Required Changes at Impacted Points

1. `RequestForSampleToOrderServiceController.cs:21`
   - Replace direct `r.RequestSampleOrder(r)` with workflow API call.
   - Keep response model mapping (`SampleOrderModel`) unchanged where possible.

2. `Request.cs:165`
   - Keep method temporarily as compatibility wrapper (or make internal helper for old paths).
   - Migration target is for external/API callers to use workflow endpoint directly.

3. Input and validation harmonization
   - Normalize request XML-to-DTO mapping now done in controller and object model.
   - Ensure same validation messages for current callers.

Why medium effort:

- More entities and business defaults than low example.
- Still bounded compared to checkout/account flows.

---

## Endpoint Sketch 3 (High Effort)

### Workflow

Checkout submit order (including payment flow states and order creation path).

Current core path includes:

- `Koretech.KommerceServer.Web\Areas\Store\Models\Checkout\CheckoutContext.cs:1009`
- `Koretech.KommerceServer.Web\Areas\Store\Controllers\CheckoutController.cs:252`
- `Koretech.KommerceServer.Web\Areas\Store\Controllers\CheckoutController.cs:348`
- `Koretech.KommerceServer.Web\Areas\Store\Controllers\CheckoutController.cs:685`
- `Koretech.KommerceServer.Web\Areas\Store\Controllers\CartController.cs:194`
- `Koretech.KommerceServer\BusinessObjects\ShopCart\ShopCart.cs:1617`
- `Koretech.KommerceServer\BusinessObjects\ShopCart\BOShopCart.cs:2131`
- `Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:670`
- `Koretech.KommerceServer\BusinessObjects\Order\Quote\QuoteHeader.cs:443`

### Proposed API

`POST /api/workflows/checkout/submit-order`

Request:

```json
{
  "cartNo": 12345,
  "useHostedWorkflow": true,
  "paymentAccountNo": 999,
  "authorizeReturnToken": null,
  "requestedByUserId": "jdoe",
  "idempotencyKey": "1f064bc1-d829-4f6d-bae3-b47b74d4f8c8"
}
```

Response union:

```json
{
  "status": "redirect_required",
  "redirectUrl": "https://payment-provider/...",
  "iframeViewName": "PaymentElementsExpressIFramePartial",
  "orderNo": null,
  "warnings": []
}
```

```json
{
  "status": "completed",
  "orderNo": 112233,
  "remainingSplitItemCount": 0,
  "warnings": []
}
```

```json
{
  "status": "failed",
  "errors": [
    "Unable to retrieve card details for submitting this order"
  ]
}
```

### Required Changes at Impacted Points

1. `CheckoutContext.cs:1009`
   - Replace `_carts.CompleteCart(...)` direct orchestration with workflow API client call.
   - Keep existing domain result mapping contract for controllers.

2. `CheckoutController.cs:252`, `:348`, `:685`
   - Existing submit calls should consume unified workflow response state (`completed`, `redirect_required`, `failed`) and preserve current UX redirects.

3. `CheckoutController.cs:467`
   - `AuthorizeReturn` path should call same workflow endpoint with `authorizeReturnToken` (or a paired `/authorize-return` workflow endpoint if split by design).

4. `CartController.cs:194`
   - Replace direct `new BOShopCart().CreateOrder(cart, auth)` usage with workflow API call for completed-cart processing.

5. BO compatibility wrappers (transition)
   - Meaning: keep existing BO methods as temporary pass-through adapters so current callers do not break while call sites are migrated.
   - In this phase, `ShopCart.CompleteCart`, `BOShopCart.CreateOrder`, `SalesOrderHeader.CreateOrder`, and `QuoteHeader.ConvertToOrder` should delegate to the new workflow API client (or workflow service), not own long-term orchestration logic.
   - Removal target: delete these wrappers only after all known entry points (`CheckoutController`, `CartController.ProcessCompletedCart`, single-page checkout) are migrated and parity telemetry is stable.

Why high effort:

- Largest orchestration surface.
- Multiple existing entry points.
- Payment-provider and post-submit behavior must remain stable.
- Highest transactional/performance/correctness risk if contract is too chatty or ambiguous.

---

## Vertical Slice Coupling Matrix

This section evaluates **coupling between candidate vertical slices** and recommends extraction boundaries that minimize migration risk.

Boundary rules used:

1. Cut at workflow entry points (controller/page command methods), not inside generic BO CRUD plumbing.
2. Prefer existing service seams (`ICartService`, `IAccountRequestService`, `ISalesOrderService`) where they already exist.
3. Avoid early slicing through `DataAwareContext` multi-instance save behavior.

Baseline for difficulty ratio is `1.0x` (easiest extraction).

| Recommended Slice | Boundary (Where to Cut) | Key Coupling | Relative Extraction Difficulty |
|---|---|---|---:|
| Shop list from cart/order | UI command -> workflow service | Cart/order item copy only; narrow write set | 1.0x |
| Sample request intake (`CreateWithContactAndBusiness`) | Request page command -> workflow service | Depends on account ensure path | 1.5x |
| Guest/implicit account ensure (`EnsureAccountExists`) | `IAccountRequestService` operation | Shared dependency of request + checkout | 2.0x |
| Sample request -> order conversion | API controller -> workflow service | Couples to sales-order creation path | 2.3x |
| Sales order approval/abandon | Approval page commands -> workflow service | Contact/customer/user/address/cart cleanup | 3.0x |
| Full account registration/onboarding | AccountRequest methods -> workflow service | Session/verification + identity + customer/contact + tx coordination | 4.8x |
| Payment apply/invoice close | Explicit payment workflow endpoint (not generic save) | Depends on `DataAwareContext` cached secondary objects | 5.4x |
| Checkout submit + completed-cart processing | `CheckoutContext.SubmitOrder` / `AuthorizeReturn` and completed-cart path | Highest coupling: cart/order/quote/payment/account/session/providers/background service | 6.5x |

### Code Anchors

- Shop list slice:
  - `Koretech.KommerceServer.Web\Pages\ShopCart_Listing.aspx.cs:67`
  - `Koretech.KommerceServer\BusinessObjects\ShopList\UserShopList\UserShopList.cs:226`
- Sample intake slice:
  - `Koretech.KommerceServer.Web\Pages\Request_For_Sample.kaml.cs:209`
  - `Koretech.KommerceServer\BusinessObjects\Request\Request.cs:261`
- Guest account ensure slice:
  - `Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\IAccountRequestService.cs:7`
  - `Koretech.KommerceServer\BusinessObjects\ShopCart\BOShopCart.cs:1286`
  - `Koretech.KommerceServer\BusinessObjects\Request\Request.cs:270`
- Sample request -> order slice:
  - `Koretech.KommerceServer.Web\Areas\Store\Controllers\RequestForSampleToOrderServiceController.cs:21`
  - `Koretech.KommerceServer\BusinessObjects\Request\Request.cs:165`
- Sales order approval slice:
  - `Koretech.KommerceServer.Web\Pages\SalesOrder_BC_Approval.kaml.cs:260`
  - `Koretech.KommerceServer.Web\Pages\SalesOrder_BC_Approval.kaml.cs:265`
  - `Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:1554`
  - `Koretech.KommerceServer\BusinessObjects\Order\SalesOrder\SalesOrderHeader.cs:1670`
- Account onboarding slice:
  - `Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\AccountRequest.cs:209`
  - `Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\BOAccountRequest.cs:323`
  - `Koretech.KommerceServer\BusinessObjects\Person\AccountRequest\BOAccountRequest.cs:605`
- Checkout slice:
  - `Koretech.KommerceServer.Web\Areas\Store\Models\Checkout\CheckoutContext.cs:40`
  - `Koretech.KommerceServer.Web\Areas\Store\Models\Checkout\CheckoutContext.cs:1005`
  - `Koretech.KommerceServer.Web\Areas\Store\Controllers\CheckoutController.cs:252`
  - `Koretech.KommerceServer.Web\Areas\Store\Controllers\CartController.cs:194`
  - `Koretech.KommerceServer\BusinessObjects\ShopCart\BOShopCart.cs:2072`
- Payment apply slice:
  - `Koretech.KommerceServer.Web\Pages\Payment_Detail.kaml.cs:27`
  - `Koretech.KommerceServer\BusinessObjects\Payment\Payment.cs:96`
  - `Koretech.KommerceServer\BusinessObjects\Payment\Payment.cs:113`
  - `Koretech.Framework\Platform\BOCommon\DataAwareContext.cs:218`

### Inter-Slice Dependency Notes

1. Checkout depends on order creation and account ensure flows.
2. Sample request intake depends on account ensure.
3. Sample request -> order depends on sales-order creation internals.
4. Payment apply is coupled to framework UI/context orchestration and should be migrated as a dedicated workflow, not generic CRUD.

---
## Rollout Plan (Pilot-First)

### Phase 0: Foundations (short)

1. Introduce workflow API conventions:
   - idempotency key,
   - correlation ID,
   - uniform error envelope,
   - command result states.
2. Build a single client abstraction in KommerceServer for workflow calls.

### Phase 1: Pilot (Low Endpoint)

Pilot endpoint: `POST /api/workflows/shoplists/create-from-cart` (Endpoint Sketch 1).

Goals:

1. Validate API shape, auth, tracing, and retry behavior.
2. Validate that current page UX remains unchanged.
3. Validate perf and operational telemetry (P50/P95 latency, failure rates).

Exit criteria:

1. Functional parity with current cart-to-list behavior.
2. No measurable UX regression.
3. Stable retry/idempotency behavior under induced faults.

### Phase 2: Medium Endpoint

Implement `POST /api/workflows/requests/sample-to-order` (Endpoint Sketch 2).

Goals:

1. Validate multi-entity transactional command behavior.
2. Confirm migration pattern from controller -> workflow API client.

### Phase 3: High Endpoint (Checkout)

Implement `POST /api/workflows/checkout/submit-order` (Endpoint Sketch 3).

Goals:

1. Consolidate checkout orchestration behind explicit workflow contract.
2. Preserve current redirect/auth-return behavior.
3. De-risk largest flow before broader workflow endpoint expansion.

### Phase 4: Broader High-Risk Workflows

1. Account registration/provisioning workflows.
2. Sales order approve/abandon.
3. Payment apply/update.

This phase should start only after checkout workflow telemetry and rollback procedures are proven.


