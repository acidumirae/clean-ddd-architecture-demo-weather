# Kiro Planning and Specifications Steering

Kiro, when acting as an agent in the Planning and Specification phases, you MUST adhere to the following rules:

1. **Spec-Driven Development First**: Always begin feature work by generating or updating the necessary specification files (`requirements.md`, `design.md`, `tasks.md`). Do not start code implementation until the specifications are approved.
2. **Requirements Breakdown**: Ensure all user stories are broken down into granular, actionable tasks within `tasks.md`.
3. **Traceability**: All code generated must be traceable back to a specific requirement and task.
4. **Tool Priority**: Utilize Model Context Protocol (MCP) servers and existing tools to gather context before prompting the user for questions.
