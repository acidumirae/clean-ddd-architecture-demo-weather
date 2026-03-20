# AWS Kiro SDLC Configuration Walkthrough

I have successfully created an end-to-end SDLC configuration for feature development using AWS Kiro. All configurations reside within the `.kiro/` directory, which Kiro natively recognizes to align with your project processes.

## Configurations Created

### Steering Intelligence
I created steering documents under `.kiro/steering/` to give Kiro constraints and principles:
*   `01_planning_and_specs.md` - Directs Kiro to break down tasks before coding.
*   `02_code_style.md` - Enforces standard idioms, Clean Architecture lines, and static analysis quality.
*   `03_accessibility.md` - Requires WCAG 2.1 AA and proper ARIA usage for UI changes.
*   `04_testing.md` - Mandates isolating units, Arrange-Act-Assert, and >80% coverage on new code.
*   `05_commit_style.md` - Enforces the Conventional Commits specification.

### Specification Templates
Templates located in `.kiro/specs_templates/` standardizing planning phases:
*   `requirements.md` - For user stories, acceptance criteria, and non-functional bounds.
*   `design.md` - For architecture review and layer boundary mapping.

### Agent Hooks
Event-driven JSON hooks located in `.kiro/hooks/` for automated verification:
*   `accessibility_check.json` (onSave) - Validates HTML markup compliance.
*   `code_style_check.json` (onSave) - Synchronizes codebase using `dotnet format`.
*   `test_generation.json` (onSave) - Scaffolds tests using Moq/xUnit directly opposite implementation files.
*   `coverage_check.json` (onTestRun) - Triggers strict fail-conditions if test coverage is under 80%.
*   `quality_bar_enforcement.json` (preCommit) - Runs a final pass of format and test execution.
*   `commit_msg_alignment.json` (preCommit) - Verifies the commit message strictly against conventional protocols.

### Documentation
Included a comprehensive `SDLC_PROCESS.md` in `.kiro/docs/` and a top-level `.kiro/README.md` detailing the operational phases from Planning through Commit stages.

> [!TIP]
> The configuration serves as a direct integration with Kiro's Spec-Driven framework. As you edit `.cs` or UI files, you will notice Kiro automatically executing hooks and adapting generate-responses based on the steering documents.
