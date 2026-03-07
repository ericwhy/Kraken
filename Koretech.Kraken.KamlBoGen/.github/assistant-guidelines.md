Repository assistant guidelines

These rules are the canonical instructions the automated assistant and contributors should follow when modifying this repository. Keep changes explicit and small; enforce with CI where possible.

1. Project & file layout
   - For this repository, the main codebase is in the `Koretech.Kraken.KamlBoGen` project (aka 'tools project').
   - This project implements a code generator that produces source files for the product project.
   - The product project is located outside this repository, in a repository at C:\Dev\Source\Repos\KommerceServer-Kraken, and is called Koretech.KommerceServer.
   - Metadata describing the business objects from the proudct project are located in this repo in the Koretech.Kraken.Data project.
   - Generated code from the tools project is placed in the `Gen` folder under the repo root.  The tools project should not contain any generated code itself, but may contain templates and generator logic.
   - Keep tools code under `Koretech.Kraken.KamlBoGen/` and tests under `Koretech.Kraken.KamlBoGen.Test/`.

2. Tests
   - When creating unit or integration tests for the tools project, always create them in the separate test project named `Koretech.Kraken.KamlBoGen.Test` (aka 'test project').
   - Do not create the `Koretech.Kraken.KamlBoGen.Test` project.  Use the existing project.
   - The test project must reference the tools project via a `<ProjectReference/>` and include only test packages (NUnit/Moq/Test SDK, etc.).
   - Within the test project, place tests in the Tests/ folder, and oranize test classes within that folder using the E2E, Integration, Unit subfolders as appropriate.
   - Use NUnit for tests and Moq for mocking unless a specific test requires otherwise.
   - Tests for any other projects in the solution should also be created in separate test projects named `<ProjectName>.Test` following the same pattern and use the same organization and rules.
   - Prefer testing behavior via public interfaces. If testing internal methods is required, expose them to the test assembly with `InternalsVisibleTo("<TestProjectName>")` in `Properties/AssemblyInfo.cs`.

3. Dependency Injection & registration
   - Register concrete SDK clients and wrappers in `Extensions/ServiceCollectionExtensions.cs` when wiring services.
   - Register other application services in `Extensions/ServiceCollectionExtensions.cs` or in separate files under `Extensions/` if needed.
   - Prefer registering typed clients and factories (e.g., `IHttpClientFactory`) instead of constructing `HttpClient` directly in services.
   - Keep service registrations idempotent and avoid calling `BuildServiceProvider()` in application startup code.

4. Use of external SDKs
   - Prefer the official typed SDK types from installed NuGet packages. If the workspace package layout differs across environments, prefer explicit, typed calls where possible and avoid fragile reflection-only implementations unless required.
   - If a package restore fails due to unavailable transitive packages, update package versions in the csproj to compatible versions that restore from public nuget.org or add a documented alternate feed.

5. Secrets & configuration
   - Never commit secrets (API keys, passwords) to source. Use `appsettings.Development.json`, environment variables, or secure stores for secrets.
   - Keep default config in `appsettings.json` minimal and non-secret. Add example config as `appsettings.example.json` if helpful.
   - Use `IOptions<T>` and `IOptionsMonitor<T>` for configuration binding. Create strongly typed options classes under `Configuration/`.
   - Name the options class after the feature it configures (e.g., `ImporterOptions` for importer-related settings).

6. Option validation
   - Add a validator implementing `IValidateOptions<T>` and register it in `Program.cs`.  Call it `KamlBoGenOptionsValidator` for `KamlBoGenOptions`.
   - Validators should guard CI runs: if running under a CI environment (detect `CI`, `GITHUB_ACTIONS`, `TF_BUILD`, `BUILD_BUILDID`) fail fast when essential configuration (like `SqlConnectionString`) is missing.

7. Coding patterns
   - Prefer async/await and propagate `CancellationToken` on public async methods.
   - Use explicit nullability annotations (nullable reference types enabled). Handle possible nulls responsibly.
   - Log exceptions with contextual messages. Do not swallow exceptions silently.
   - Avoid reflection unless there is no reasonable typed API; if used, document why and add a TODO to replace with typed calls when available.
   - When testing for error conditions using and if-else block, place the success scenario in the if branch and the error handling in the else branch to improve readability.

8. Error handling & fallbacks
   - Fail fast on configuration errors during startup rather than letting the app run with partial config.
   - Provide sensible fallbacks (e.g., return placeholder objects if a service call fails), but log the error clearly so issues are visible.
   - 
9. Code style & ordering
   - Keep files small and focused. Order members in classes consistently: constants, fields, constructors, public methods, internal methods, private helpers.
   - Only use var when the type is obvious from the right-hand side (e.g., `var list = new List<string>()`).
   - Create class comments for every class explaining its purpose.
   - Create method comments for every non-trivial method, especially public APIs.  Include description of parameters and return values.
   - Create inline comments for complex logic or non-obvious behavior.
   - When modifying existing code, do not remove existing comments unless they are incorrect or misleading.  If they are incorrect, update them to reflect the current behavior.
   - Always place code following control structures in a block (e.g., after `if`, `for`, `while`, etc.), except for parameter validation.
   - Structure code so that a method has only one return statement where possible.

10. Documentation
    - When you complete a significant change (new feature, major refactor) create or update a markdown file in the `<project_root>/docs/` folder or an appropriate subfolder explaining the change, how to use it, and any important implementation details.
    - When you're documenting the importer, but not the tools, place the documents in the product project.
    - When you're documenting changes to the tools, place the documents in the tools project.
    - Do not document minor changes or fixes in the docs, but do update code comments as needed.
    - Do not document one-time use scripts or throwaway code.  Do document scripts that will be reused or are non-trivial.
    - When you extend or alter functionality that is already documented, update the existing documentation to reflect the new behavior.
    - An example of a significant change would be adding a new service, implementing a new feature, or refactoring a major component.
    - An example of a minor change would be creating additional logging, adding a parameter to a method, or adding a new test case in an existing test class.

11. CI / PR requirements
    - PRs must run build and tests. Add GitHub Actions to run `dotnet restore`, `dotnet build`, and `dotnet test` for all test projects.
    - Add a CI check that rejects unit tests placed in the product project (simple script scanning for `class .*Tests` under the product project path).

12. Commits & PR messages
    - Keep commits small and focused. In PR descriptions mention the guideline(s) followed, what changed, and why.

13. When editing files
    - If the assistant will edit multiple files across layers (DI, models, service impls, tests), create a short plan first and list files to change.
    - After edits, run `dotnet build` and `dotnet test` and report results. Fix compile/test errors before finishing.

14. Ask before changing conventions
    - If a requested change would alter repository-wide conventions (project layout, library choices, persistent storage for secrets), ask the repo owner before applying.

How to use this file with the assistant
- When requesting code changes, include the instruction: `Follow .github/assistant-guidelines.md`.
- The assistant will read this file and follow these rules when making edits.

Contact
- If any rule is unclear or you want to extend the guidelines (e.g., add code formatting or analyzer rules), open an issue or request an update to this file.

