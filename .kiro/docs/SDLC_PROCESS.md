# AWS Kiro SDLC Process Documentation

This document outlines the end-to-end feature development Software Development Life Cycle (SDLC) managed by AWS Kiro in this project. 

The configuration in the `.kiro` directory automates the phases below:

## 1. Planning 
- Driven by the `.kiro/steering/01_planning_and_specs.md` steering document.
- When a new feature is requested, Kiro initiates by creating or updating `requirements.md` based on `.kiro/specs_templates/requirements.md`.
- Kiro breaks requirements into granular tasks (`tasks.md`).

## 2. Specification
- Kiro populates `design.md` utilizing the `.kiro/specs_templates/design.md`.
- A technical architecture review is performed by Kiro to ensure alignment with existing patterns (Clean Architecture/DDD).

## 3. Code Generation
- Implementation begins only after specs are approved.
- Code generation respects guidelines in `.kiro/steering/02_code_style.md`.
- Any UI component generation respects `.kiro/steering/03_accessibility.md`.

## 4. Accessibility Task Handling
- Handled automatically by the `accessibility_check.json` hook.
- Triggered `onSave` for `.html` or `.cshtml` files, it verifies WCAG 2.1 AA compliance and provides a report.

## 5. Test Generation
- Handled automatically by the `test_generation.json` hook.
- Triggered `onSave` for new C# code in `src/`, Kiro will generate xUnit tests following AAA patterns as prescribed in `.kiro/steering/04_testing.md`.

## 6. Code Style Check
- Handled via `code_style_check.json`.
- Automatically executes `dotnet format` to enforce editorconfig alignments.

## 7. Code Coverage Check
- Governed by `coverage_check.json` hook.
- Triggers after automated tests to ensure test coverage does not fall below 80%.

## 8. Commit Message Alignment
- Governed by `commit_msg_alignment.json`.
- Verifies that any code commits align with the Conventional Commits specification outlined in `.kiro/steering/05_commit_style.md`.

## 9. Quality Bar Enforcement
- Executed via `quality_bar_enforcement.json` prior to final commits or pull requests.
- Runs compiler checks, tests, and formatting to ensure no degradation of codebase health.
