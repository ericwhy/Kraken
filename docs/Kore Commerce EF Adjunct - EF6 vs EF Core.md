# Kore Commerce EF Adjunct Analysis - EF6 vs EF Core Hybrid

## Purpose

This document is an adjunct to `Kore Commerce EF Phase 1.md`. It evaluates whether Phase 1 should target:

1. **EF6 in-process on .NET Framework** (with existing Kore Commerce codebase mostly unchanged), or
2. **EF Core directly** via a **hybrid local deployment** where most Kore Commerce code remains on .NET Framework while persistence runs out-of-process on **.NET 10**.

It focuses on risk, complexity, transaction preservation, and performance.

---

## Constraints and Assumptions

1. Existing BO APIs should remain stable where possible (current preference in Phase 1 Option B2).
2. Existing transaction semantics must be preserved where currently used.
3. New implementation must not be slower than current production behavior.
4. Scope is local deployment interoperability (same machine), unless distributed architecture is materially better.

---

## Verified Platform Facts (as of March 4, 2026)

1. **EF Core 10** is released (November 2025), is **LTS**, and requires **.NET 10 runtime**.
2. EF Core 10 **does not run on .NET Framework**.
3. **EF6** is still supported but is in maintenance mode; active feature development is on EF Core.
4. .NET Framework gRPC client support is constrained by HTTP/2 platform/runtime requirements; compatibility is environment-sensitive.
5. `System.Transactions` distributed transaction behavior across modern .NET runtimes/providers is constrained and must be treated as a design risk, especially cross-process.

---

## Option 1: EF6 First (In-Process, .NET Framework)

### Description

Replace SubSonic with EF6 while keeping Kore Commerce in-process on .NET Framework for Phase 1. Keep BO interfaces stable (aligned with existing preferred approach).

### Pros

1. Lowest immediate architectural disruption.
2. No inter-process boundary added in Phase 1.
3. Easiest path to preserve existing transaction behavior and ambient transaction patterns.
4. Lowest latency risk for chatty BO persistence paths.
5. Best fit for current preference to avoid cascading interface/codebase changes.

### Cons

1. Introduces an intermediate persistence target (EF6) that is not the final target.
2. Some migration effort may be repeated when moving from EF6 to EF Core.
3. Slower arrival at full .NET 10 + EF Core architecture.

### Risk Profile

- **Delivery risk**: Low to Medium
- **Runtime risk**: Low
- **Long-term modernization speed**: Medium

---

## Option 2: Direct EF Core via Hybrid Out-of-Process Persistence (.NET Framework + .NET 10)

### Description

Keep most Kore Commerce code on .NET Framework temporarily, but move persistence into a local .NET 10 process using EF Core 10. .NET Framework calls persistence through IPC.

### Local Interop Choices

1. **REST/JSON over localhost**
- Easiest interoperability and diagnostics.
- Highest serialization overhead among practical options.
- Most likely to regress performance on chatty call patterns unless aggressively batched.

2. **gRPC/Protobuf over localhost**
- Better payload and CPU efficiency than JSON.
- .NET Framework client support has environmental constraints; adds deployment/runtime caveats.
- Feasible but integration risk is higher than REST for legacy host.

3. **Named pipes (binary RPC or HTTP over pipes)**
- Strong local-only performance potential.
- More custom plumbing and operational complexity.
- Better as an optimization after proving correctness with simpler transport.

### Pros

1. Reaches target ORM/platform architecture sooner.
2. Avoids a second ORM migration step (EF6 -> EF Core).
3. Forces earlier service-boundary discipline useful for long-term architecture.

### Cons

1. Highest Phase 1 complexity and delivery risk.
2. High risk of net slowdown unless BO call patterns are coarse-grained.
3. Cross-process transaction semantics are significantly harder than in-process EF6.
4. Requires stronger observability, retry, idempotency, and failure-mode engineering from day one.

### Risk Profile

- **Delivery risk**: High
- **Runtime risk**: Medium to High
- **Long-term modernization speed**: High (if successful)

---

## Transaction Semantics Analysis

### Why this is critical

Current Kore Commerce patterns likely rely on transaction scopes spanning multiple persistence operations under one business operation. Crossing a process boundary changes this fundamentally.

### Key implications for Hybrid

1. **Do not assume ambient TransactionScope will transparently flow across processes.**
2. Two practical strategies exist:
- **A. Server-owned unit-of-work per API call**: each call is atomic in the persistence service.
- **B. Explicit remote transaction/session**: client opens a persistence session, executes multiple commands, then commits/rolls back.
3. Strategy B can preserve semantics better, but increases complexity (session lifecycle, timeouts, orphan cleanup, lock duration).
4. Distributed transaction coordination across process/runtime boundaries should be avoided in Phase 1 due to operational and performance risk.

### Practical conclusion

If preserving current multi-step transaction behavior with minimal disruption is non-negotiable, **in-process EF6 has a major advantage** for Phase 1.

---

## Performance Analysis

### Baseline risk

Many existing BO persistence flows are likely chatty (multiple reads/writes per business operation, parent/child traversals, lazy loading patterns). A process boundary magnifies this cost.

### Expected impact by approach

1. **EF6 in-process**
- Removes SubSonic overhead with minimal added call overhead.
- Best chance to match or beat current latency immediately.

2. **Hybrid EF Core out-of-process**
- Adds IPC + serialization overhead to every persistence interaction.
- Can outperform only if calls are re-shaped into coarse-grained operations and query patterns are optimized.

### Performance guardrails required for Hybrid

1. No per-property/per-row remote chatter.
2. Batch-oriented contracts for aggregate operations.
3. Repository/service methods that load/update complete aggregates in one round trip where possible.
4. Strict SLO instrumentation from day one (P50/P95/P99 latency, throughput, error rate).

Without these guardrails, hybrid is unlikely to meet "not slower" requirement.

---

## Decision Matrix

| Criteria | EF6 In-Process (.NET Framework) | EF Core Hybrid (.NET 10 out-of-process) |
|---|---|---|
| Preserve BO interfaces | Strong | Moderate (requires adapter/proxy layer) |
| Preserve transaction semantics | Strong | Weak-Moderate (needs explicit transaction/session design) |
| Phase 1 delivery risk | Low-Medium | High |
| Performance regression risk | Low | Medium-High |
| Time to long-term architecture | Medium | Fast (if successful) |
| Operational complexity | Low | High |
| Best fit with current preferred Option B2 style | Strong | Moderate |

---

## Recommendation

### Primary recommendation

For **Phase 1**, use **EF6 in-process on .NET Framework** while preserving BO interfaces (consistent with current preferred direction and minimal-disruption intent).

### Why

1. It best satisfies the hard constraints: transaction preservation and no performance regression.
2. It minimizes cascading code changes in the existing codebase.
3. It de-risks delivery while still removing SubSonic.

### Strategic caveat

Design EF6 Phase 1 as a **transitional architecture**, not a dead-end:

1. Keep repository/service contracts EF-provider-agnostic where possible.
2. Avoid EF6-specific features that are hard to port.
3. Introduce aggregate-oriented persistence operations now to reduce future chatty patterns.
4. Add telemetry that will later validate parity when moving to EF Core.

---

## If Direct EF Core Hybrid Is Still Desired

A safer way to attempt it is a constrained pilot before committing broadly.

### Pilot scope

1. Pick 1-2 low-complexity domains with simple transaction behavior.
2. Use local-only deployment.
3. Start with transport that minimizes integration risk (often REST first), then optimize to gRPC/pipes only if needed.
4. Use explicit server-side unit-of-work boundaries per call; avoid cross-call transactions initially.

### Exit criteria (must pass)

1. Functional parity with existing domain behavior.
2. Transactional correctness for selected workflows.
3. Equal or better P95 latency and throughput versus current implementation.
4. Operational stability under load/fault tests.

If pilot misses these criteria, continue with EF6-first path and defer hybrid EF Core boundary until more of the app can move to .NET.

---

## EF6 -> EF Core Migration Difficulty (Generated Code Context)

### Short answer

Yes, there are significant differences, and migration difficulty is **moderate to high**. In this codebase, difficulty is driven less by hand-written repository code and more by **generator output shape**, runtime assumptions, and transaction/behavioral compatibility.

### Why it is non-trivial

1. **API and behavior differences are real**
- EF6 and EF Core have different APIs and defaults in key areas (query translation, loading behavior, change tracking details, raw SQL APIs, migrations tooling).
- Some LINQ/query patterns that work in EF6 either fail translation or behave differently in EF Core.

2. **Provider/runtime differences matter**
- Moving from .NET Framework + EF6 to .NET 10 + EF Core changes runtime behavior, dependency graph, and operational diagnostics.
- Transaction behavior and connection management patterns need explicit re-validation.

3. **Generated code multiplies impact**
- A small model-level difference affects many generated files (entities, configurations, contexts, repositories, services).
- If templates are not designed for dual-targeting, migration becomes a broad generator rewrite instead of targeted template swaps.

### Major EF6 vs EF Core differences to account for in generators

1. **Model configuration API differences**
- EF6 uses `DbModelBuilder`/legacy fluent APIs; EF Core uses `ModelBuilder` + `IEntityTypeConfiguration<T>` patterns.
- Generator templates for context and configuration are not directly portable.

2. **Query translation and client/server evaluation rules**
- EF Core is stricter about server translation; unsupported expressions fail at runtime instead of silently switching behavior.
- Generator-emitted query methods must avoid non-translatable constructs.

3. **Loading/navigation behavior**
- Lazy loading, explicit loading, and projection behavior differ and often require explicit design choices in EF Core.
- Generator patterns should prefer explicit aggregate loading patterns to avoid N+1 and proxy coupling.

4. **Change tracking and state handling**
- Attachment/update semantics differ enough that generic upsert/update templates need EF Core-specific logic.

5. **Raw SQL and command execution APIs**
- EF6 and EF Core raw SQL entry points and shape/materialization constraints differ.
- Any generated raw SQL helper methods need separate templates.

6. **Migrations/tooling workflow**
- EF Core migrations and design-time factories are different from EF6 migration tooling.
- Generators should emit predictable context construction for tooling.

7. **Interceptors/conventions/extensibility points**
- Interceptor and convention registration models differ; generator hooks must be refactored.

### Estimated migration effort (EF6-first to EF Core later)

Assuming EF6 Phase 1 is delivered first and code remains generator-driven:

1. **Generator refactor for dual EF targets**: 2-4 weeks
2. **Template conversion (context/config/repo/service)**: 2-4 weeks
3. **Query pattern remediation and translation-safe rewrites**: 2-5 weeks
4. **Transaction/behavior parity validation**: 2-4 weeks
5. **Performance tuning and regression fixes**: 2-4 weeks
6. **Integration/regression testing across domains**: 3-6 weeks

**Total likely range**: **13-27 weeks** depending on how much EF6 output is EF-Core-ready by design.

### How to reduce this migration cost now (recommended)

If we proceed EF6-first, generator strategy should intentionally de-risk EF Core migration:

1. **Introduce a provider-neutral intermediate generation model**
- Keep one metadata model and domain semantics layer.
- Have EF6 and EF Core emitters behind that layer.

2. **Dual-template architecture from day one**
- Separate templates by concern (`Entity`, `Configuration`, `Context`, `Repository`, `Service`) and by provider (`EF6`, `EFCore`).
- Avoid embedding provider-specific logic in shared templates.

3. **Generate translation-safe query building blocks**
- Standardize expression helpers known to translate in both providers where possible.

4. **Codify transaction boundaries in generated service methods**
- Prefer explicit unit-of-work boundaries; avoid hidden ambient-only assumptions.

5. **Golden-output and behavioral test harness**
- Snapshot generated code and run parity tests against both providers for pilot domains.

### Decision impact

- **EF6-first is still the lower-risk Phase 1 delivery option**, but it is not a "free" bridge.
- Without generator architecture changes now, EF6 -> EF Core later can become a large second rewrite.
- With deliberate dual-target generator design in Phase 1, the later EF Core move becomes substantially more predictable.

## Suggested Revised Roadmap

1. **Phase 1 (now):** SubSonic -> EF6 in-process, BO API compatibility preserved.
2. **Phase 1.5:** Introduce persistence contracts and aggregate-oriented methods to reduce chatty behavior.
3. **Phase 2:** Move hosting/runtime slices to .NET (co-locating more business logic with EF Core) to reduce/removing process boundary penalties.
4. **Phase 3:** Complete EF Core migration when sufficient app surface is on modern .NET.

This path balances immediate risk management with long-term modernization.

## Appendix

A file-level Kraken generator impact map and refactor plan is documented in:

- Kore Commerce EF Adjunct Appendix - Kraken Generator Impact Map.md

## Sources

- EF Core 10 release notes: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew
- EF support policy (EF6 vs EF Core): https://learn.microsoft.com/en-us/ef/efcore-and-ef6/support
- EF Core transactions limitations: https://learn.microsoft.com/en-us/ef/core/saving/transactions
- gRPC .NET Framework support constraints: https://learn.microsoft.com/aspnet/core/grpc/netstandard
- Kestrel named pipes transport (for local-only IPC): https://learn.microsoft.com/en-us/aspnet/core/grpc/interprocess-namedpipes

