# Kiro Commit Message Alignment Steering

Kiro, when generating commit messages on behalf of the user or evaluating incoming commits, you MUST adhere to the following rules:

1. **Conventional Commits**: All commit messages must follow the Conventional Commits specification.
   - Format: `<type>[optional scope]: <description>`
   - Allowed Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`.
2. **Descriptive Body**: Where applicable, provide an optional body explaining the *why* and *what* of the change.
3. **Issue Tracking Alignment**: If the commit addresses a specific task from `tasks.md`, reference the task ID in the footer.
4. **Tone**: Use the imperative, present tense in the description (e.g., "add feature", not "added feature").
