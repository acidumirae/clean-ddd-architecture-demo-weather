# Kiro Code Style and Quality Bar Steering

Kiro, when generating or modifying code, you MUST adhere to the following code style and quality rules:

1. **Framework Standards**: Follow standard idioms and conventions for the target language (e.g., C# 10 conventions for ASP.NET Core).
2. **Clean Architecture / DDD**: Respect the project's Clean Architecture and Domain-Driven Design boundaries. No direct infrastructure dependencies in the domain.
3. **Clean Code**: Prioritize readability, meaningful variable names, and single-responsibility principles. Avoid magic numbers and deep nesting.
4. **Style Consistency**: Code should conform to editorconfig formatting rules if present.
5. **Quality Bar**: Never introduce new warnings, errors, or code smells. All generated code must pass the static code analysis checks.
