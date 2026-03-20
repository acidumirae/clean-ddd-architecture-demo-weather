# AWS Kiro SDLC Configuration Plan

This plan details the configuration files necessary to manage an end-to-end feature development SDLC using AWS Kiro. The setup uses Kiro's native spec-driven approach, steering intelligence, and agent hooks.

## Proposed Changes

### Configuration Directories
We will operate entirely within the `.kiro/` directory, establishing the following structure:
- `.kiro/steering/`: Markdown documents to guide Kiro's behavior and set standards.
- `.kiro/hooks/`: JSON configuration files for event-driven automation triggers.
- `.kiro/specs_templates/`: Templates for requirements and design.
- `.kiro/docs/`: Process documentation for developers.

### Steering Files (`.kiro/steering/`)
These files will provide persistent context to Kiro for code generation and quality enforcement.
- `[NEW] .kiro/steering/01_planning_and_specs.md`: Guides Kiro on how to break down tasks and create specifications.
- `[NEW] .kiro/steering/02_code_style.md`: Sets the code style rules and quality bar expectations.
- `[NEW] .kiro/steering/03_accessibility.md`: Defines required accessibility features and standards for UI components.
- `[NEW] .kiro/steering/04_testing.md`: Dictates requirements for test generation and coverage thresholds.
- `[NEW] .kiro/steering/05_commit_style.md`: Defines the required commit message alignment (e.g., Conventional Commits).

### Agent Hooks (`.kiro/hooks/`)
Hooks will automate checks and tasks at various stages of the SDLC.
- `[NEW] .kiro/hooks/code_style_check.json`: Triggered `onSave`, automatically formats code or flags style violations.
- `[NEW] .kiro/hooks/accessibility_check.json`: Triggered `onSave` for UI files, runs a11y analysis.
- `[NEW] .kiro/hooks/test_generation.json`: Triggered `postToolUse` or via manual command to scaffold tests for new code.
- `[NEW] .kiro/hooks/coverage_check.json`: Triggered `onTestRun`, enforces code coverage minimums.
- `[NEW] .kiro/hooks/quality_bar_enforcement.json`: Master hook triggered before PR/commit to aggregate all checks.
- `[NEW] .kiro/hooks/commit_msg_alignment.json`: Triggered during commit, validates the message format against the standard.

### Specs & Docs (`.kiro/specs_templates/` & `.kiro/docs/`)
- `[NEW] .kiro/specs_templates/requirements.md`: Template for the Planning phase.
- `[NEW] .kiro/specs_templates/design.md`: Template for the Specification phase.
- `[NEW] .kiro/docs/SDLC_PROCESS.md`: A comprehensive guide on how the Kiro SDLC works end-to-end.

## Verification Plan

### Manual Verification
1. Open the project in the AWS Kiro IDE.
2. Review the `.kiro/docs/SDLC_PROCESS.md` to ensure the outlined phases align with instructions.
3. Test a hook (e.g., save a file and check if `code_style_check.json` triggers appropriately, or try to commit with an invalid message to see if `commit_msg_alignment.json` catches it).
4. Prompt Kiro to generate a new feature and verify that it utilizes the spec templates and respects the steering instructions.
