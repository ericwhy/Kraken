# Kore Commerce EF Adjunct Appendix - Kraken Generator Impact Map

## Purpose

This appendix maps EF6 -> EF Core migration impacts to specific Kraken generator files and proposes a generator architecture that minimizes rework.

Base project analyzed: `Koretech.Tools.KrakenGenerator`

---

## Current Generator Topology (As-Is)

### Entry and orchestration

- `Program.cs`
- `KamlBoGenerator.cs`

### Model layer

- `KamlBoModel/KamlBoDomain.cs`
- `KamlBoModel/KamlBoEntity.cs` (also contains `KamlEntityProperty`)
- `KamlBoModel/KamlEntityRelation.cs`

### File generators

- `FileGenerators/FileGenerator.cs` (base class)
- `FileGenerators/EntityFileGenerator.cs`
- `FileGenerators/ConfigurationFileGenerator.cs`
- `FileGenerators/ContextFileGenerator.cs`
- `FileGenerators/RepositoryFileGenerator.cs`
- `FileGenerators/ServiceFileGenerator.cs`
- `FileGenerators/BusinessObjectFileGenerator.cs`

### Observed coupling points

1. Generators write C# directly with `StreamWriter` and provider assumptions inline.
2. EF-Core-specific APIs are already present in several generators (for example `IEntityTypeConfiguration<T>`, `ModelBuilder`, `ApplyConfigurationsFromAssembly`).
3. Metadata model is still minimal and not yet parity-complete with Importer features (composites, inheritance, soft delete, external keys, partitioning).
4. Cross-cutting logic (type mapping, key parameter formatting, relation shape) is embedded in generators.

---

## File-by-File Impact Map

## Legend

- `Keep`: low change
- `Refactor`: medium change
- `Rewrite`: high change
- `Add`: new files/classes

| File | Current Role | EF6-First Impact | EF Core-Target Impact | Recommendation |
|---|---|---|---|---|
| `Program.cs` | CLI entrypoint | Add `--provider` option (`EF6`, `EFCore`) | Same | `Refactor` |
| `KamlBoGenerator.cs` | Orchestrates parse + generator execution | Must route to provider-specific emitters | Same | `Refactor` |
| `FileGenerators/FileGenerator.cs` | Shared helper base | Split provider-agnostic helpers from provider-specific code | Same | `Refactor` |
| `FileGenerators/EntityFileGenerator.cs` | Emits entity classes | Mostly reusable (POCOs) with small provider differences | Reusable | `Keep` + minor split |
| `FileGenerators/ConfigurationFileGenerator.cs` | Emits EF Core config | Needs EF6 variant (different fluent API/context shape) | Keep EF Core path | `Rewrite` into provider-specific emitters |
| `FileGenerators/ContextFileGenerator.cs` | Emits DbContext (EF Core style) | Needs EF6 context generation path | Keep EF Core path | `Rewrite` into provider-specific emitters |
| `FileGenerators/RepositoryFileGenerator.cs` | Emits repository code | Needs dual query/transaction behavior templates | Keep EF Core path | `Refactor` with provider abstraction |
| `FileGenerators/ServiceFileGenerator.cs` | Emits service contracts/impl | Mostly provider-neutral contracts; provider-specific internals | Same | `Refactor` |
| `FileGenerators/BusinessObjectFileGenerator.cs` | Emits BO wrappers | Mostly provider-neutral if persistence access abstracted | Same | `Refactor` (reduce ORM leakage) |
| `KamlBoModel/KamlBoDomain.cs` | Domain metadata | Needs richer metadata (scope function, inheritance/partition) | Same | `Refactor`/expand |
| `KamlBoModel/KamlBoEntity.cs` | Entity + property parsing | Needs major expansion (SuperType, composite, soft delete, external key fields) | Same | `Refactor`/expand |
| `KamlBoModel/KamlEntityRelation.cs` | Relation metadata | Needs target/entity resolution hooks, inverse relation support | Same | `Refactor` |
| `KamlBoGeneratorExt.cs`, `SqlTypes.cs` | helper extensions/type map | Expand type mapping for dual-provider compatibility rules | Same | `Refactor` |

---

## Proposed Generator Boundaries (Target Design)

Create provider-neutral planning + provider-specific emission.

### 1. Planning layer (provider-neutral)

Suggested new namespace/folder: `GenerationPlan/`

Add:
- `DomainGenerationPlan.cs`
- `EntityPlan.cs`
- `PropertyPlan.cs`
- `RelationPlan.cs`
- `RepositoryPlan.cs`
- `ServicePlan.cs`

Responsibility:
- Convert `KamlBoModel` into normalized generation plans.
- Enforce shared semantics once (keys, relation cardinality, naming, nullability, ownership, scopes).

### 2. Provider abstraction layer

Suggested new namespace/folder: `Providers/`

Add:
- `PersistenceProviderKind.cs` (`EF6`, `EFCore`)
- `IPersistenceEmitterFactory.cs`
- `IPersistenceEmitter.cs`

Responsibility:
- Choose provider-specific emitters at runtime.
- Keep orchestration independent of ORM-specific syntax.

### 3. Provider-specific emitters

Suggested folders:
- `Emitters/EF6/`
- `Emitters/EFCore/`

Add emitter sets per concern:
- `EntityEmitter.cs`
- `ConfigurationEmitter.cs`
- `ContextEmitter.cs`
- `RepositoryEmitter.cs`
- `ServiceEmitter.cs`

Responsibility:
- Generate provider-specific code only.
- Share templates/helpers where semantics are identical.

### 4. Shared code writer helpers

Suggested folder: `CodeWriting/`

Add:
- `CodeWriter.cs`
- `NamespaceBuilder.cs`
- `UsingSet.cs`
- `TypeNameMapper.cs`

Responsibility:
- Eliminate repetitive raw `StreamWriter` logic.
- Standardize formatting and reduce template drift.

### 5. Model expansion layer

Suggested folder additions under `KamlBoModel/`:
- `KamlBoCompositeData.cs`
- `KamlBoTable.cs`
- `KamlBoTableRelation.cs`
- `KamlBoTableKeyMap.cs`
- `KamlBoCriteria.cs`
- `KamlBoModel.cs`

Responsibility:
- Reach metadata parity required for Importer-level persistence features.

---

## Template Split Plan (Concrete)

Split templates into two axes:

1. **Concern axis**
- `Entity`
- `Configuration`
- `Context`
- `Repository`
- `Service`
- `BusinessObject`

2. **Provider axis**
- `EF6`
- `EFCore`

Expected result:
- `Entity` and much of `BusinessObject` can be mostly shared.
- `Configuration`, `Context`, and parts of `Repository` must be provider-specific.
- `Service` stays mostly provider-neutral if repository contract is stable.

---

## Generator Rework Work Packages

## WP1: Provider selection plumbing

Files touched:
- `Program.cs`
- `KamlBoGenerator.cs`

Changes:
1. Add provider option in CLI.
2. Instantiate emitter factory from provider option.
3. Keep output shape stable for consumers.

## WP2: Plan model extraction

Files touched:
- `FileGenerators/*`
- New `GenerationPlan/*`

Changes:
1. Move naming, key, relation, and type decisions out of emitters.
2. Emitters consume normalized plans only.

## WP3: Dual provider emitters

Files touched:
- Replace/refactor current `ConfigurationFileGenerator`, `ContextFileGenerator`, `RepositoryFileGenerator`.
- Add `Emitters/EF6/*` and `Emitters/EFCore/*`.

Changes:
1. EF6-specific output path.
2. EF Core-specific output path.
3. Shared contract so orchestration is unchanged.

## WP4: Metadata parity expansion

Files touched:
- `KamlBoModel/*` (expanded)

Changes:
1. Add missing metadata structures used by Importer parity analysis.
2. Add parse/post-parse resolution hooks.
3. Add model validation for new constructs.

## WP5: Regression harness for generated output

Add test project (recommended):
- `Koretech.Tools.KrakenGenerator.Tests`

Tests:
1. Golden-file tests for generated source per provider.
2. Compile tests for generated artifacts.
3. Behavioral integration tests for pilot domains.

---

## High-Risk Areas to Address Early

1. Query generation that is translation-valid in EF Core.
2. Relation mapping generation (especially composite and cross-domain).
3. Transaction boundary semantics in generated repositories/services.
4. BO wrapper expectations that currently assume in-process and active-record-like behavior.

---

## Practical Sequencing Recommendation

1. Implement WP1 + WP2 first (architecture scaffolding).
2. Implement EF Core emitters as baseline from current code.
3. Add EF6 emitters second for Phase 1 delivery.
4. Expand metadata (WP4) in parallel with high-value domains first.
5. Lock with golden-file tests before broad regeneration.

This sequencing keeps immediate delivery feasible while preventing a second major rewrite during EF Core transition.

---

## Acceptance Criteria for This Appendix Plan

1. Same kamlbo input can generate EF6 or EF Core persistence outputs via provider switch.
2. No provider-specific logic remains in provider-neutral planning layer.
3. Generated code for both providers compiles for pilot domains.
4. BO-facing service/repository contracts remain stable unless intentionally versioned.
5. Migration from EF6 output to EF Core output is template/provider switch plus targeted parity fixes, not a full generator rewrite.
