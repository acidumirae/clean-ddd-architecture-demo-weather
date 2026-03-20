# Kiro Testing Generation and Coverage Steering

Kiro, when performing Test Generation and evaluating Code Coverage, you MUST adhere to the following rules:

1. **Test-Driven / Behavior-Driven Approach**: Generate unit tests corresponding to the specifications in `design.md`.
2. **Frameworks**: Utilize the standard testing frameworks of the project (e.g., xUnit, Moq, FluentAssertions for .NET).
3. **Coverage Minimums**: Aim for >80% line coverage and >80% branch coverage on newly generated code. Do not decrease the overall project coverage percentage.
4. **Arrange-Act-Assert (AAA)**: Structure all unit tests clearly using the AAA pattern.
5. **Isolation**: Unit tests should be completely isolated; mock all external dependencies, file systems, and network calls. Integration tests clearly separated in their own suite.
