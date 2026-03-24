# Implementation Plan

- [x] 1. Extend Domain layer with GetHistoricalWeatherAsync contract
    - Add `GetHistoricalWeatherAsync(string location, DateOnly startDate, DateOnly endDate)` to `src/WeatherApp.Domain/Interfaces/IWeatherService.cs`
    - _Requirements: Condition B, Non-Functional: Domain_

- [ ] 2. Implement Application layer query and handler
    - [-] 2.1 Create GetHistoricalWeatherQuery and GetHistoricalWeatherQueryHandler
        - Create `src/WeatherApp.Application/Weather/Queries/GetHistoricalWeatherQuery.cs` as a MediatR `record` query
        - Create `src/WeatherApp.Application/Weather/Queries/GetHistoricalWeatherQueryHandler.cs` delegating to `IWeatherService.GetHistoricalWeatherAsync`
        - _Requirements: Condition B, Non-Functional: Application_

    - [~] 2.2 Write unit tests for GetHistoricalWeatherQueryHandler
        - Create `tests/WeatherApp.Application.Tests/Weather/Queries/GetHistoricalWeatherQueryHandlerTests.cs`
        - Test `Handle_ValidQuery_ReturnsWeatherForecast` — mock `IWeatherService`, assert result forwarded correctly
        - Test `Handle_ServiceThrows_PropagatesException` — verify exceptions bubble up
        - Use AAA pattern; mock all dependencies; aim for >80% coverage
        - _Requirements: Condition C_

- [ ] 3. Extend Infrastructure layer with historical API call
    - [~] 3.1 Implement GetHistoricalWeatherAsync in WeatherApiService
        - Add `GetHistoricalWeatherAsync` to `src/WeatherApp.Infrastructure/Services/WeatherApiService.cs`
        - Call `history.json?key=...&q=...&dt={startDate:yyyy-MM-dd}&end_dt={endDate:yyyy-MM-dd}`
        - Reuse `WeatherForecastApiResponse` for deserialization; map to `WeatherForecast` / `WeatherForecastDay`
        - Throw `InvalidOperationException` on null/malformed response; let `EnsureSuccessStatusCode()` handle HTTP errors
        - _Requirements: Condition B, Non-Functional: Infrastructure_

    - [~] 3.2 Write unit tests for WeatherApiService historical method
        - Create `tests/WeatherApp.Infrastructure.Tests/Services/WeatherApiServiceHistoricalTests.cs`
        - Test `GetHistoricalWeatherAsync_ValidResponse_MapsCorrectly` — mock `HttpClient` with valid JSON fixture
        - Test `GetHistoricalWeatherAsync_NullLocation_ThrowsInvalidOperationException`
        - Test `GetHistoricalWeatherAsync_HttpError_ThrowsHttpRequestException`
        - Use AAA pattern; mock all external dependencies; aim for >80% coverage
        - _Requirements: Condition C_

- [~] 4. Checkpoint — ensure all tests pass
    - Ensure all tests pass, ask the user if questions arise.

- [ ] 5. Implement Web layer controller and view
    - [~] 5.1 Create HistoricalWeatherController
        - Create `src/WeatherApp.Web/Controllers/HistoricalWeatherController.cs`
        - GET `Index` returns empty view
        - POST `Index` validates `startDate <= endDate` before dispatching `GetHistoricalWeatherQuery` via MediatR; returns view with `WeatherForecast` model or model error
        - _Requirements: Condition A, Condition B, Condition C_

    - [~] 5.2 Create Views/HistoricalWeather/Index.cshtml
        - Create `src/WeatherApp.Web/Views/HistoricalWeather/Index.cshtml`
        - Form with `<label for=...>` / `<input id=...>` pairs for city, start date, end date, and a submit button
        - Validation error spans with `role="alert"` for each field
        - Results table with `<thead>` / `<tbody>` and `scope="col"` on header cells (date, max °C, min °C, description)
        - Conditionally render table only when model has days
        - _Requirements: Condition B, Condition C, Non-Functional: Accessibility_

    - [~] 5.3 Add Historical Weather nav link to _Layout.cshtml
        - Modify `src/WeatherApp.Web/Views/Shared/_Layout.cshtml`
        - Add `<li class="nav-item"><a class="nav-link" asp-controller="HistoricalWeather" asp-action="Index">Historical Weather</a></li>` alongside existing nav links
        - _Requirements: Condition A_

    - [~] 5.4 Write unit tests for HistoricalWeatherController
        - Create `tests/WeatherApp.Web.Tests/Controllers/HistoricalWeatherControllerTests.cs`
        - Test `Index_Get_ReturnsViewResult`
        - Test `Index_Post_ValidInput_ReturnsViewWithModel`
        - Test `Index_Post_StartDateAfterEndDate_ReturnsViewWithModelError`
        - Test `Index_Post_EmptyLocation_ReturnsViewWithModelError`
        - Use AAA pattern; mock MediatR; aim for >80% coverage
        - _Requirements: Condition B, Condition C_

- [ ] 6. Property-based tests
    - [~] 6.1 Write property test for Property 1 — location is preserved
        - Add to `tests/WeatherApp.Application.Tests/Weather/Queries/GetHistoricalWeatherQueryHandlerTests.cs` (or a dedicated PBT file)
        - Generate random non-empty location strings and valid date ranges; assert `result.Location` is non-empty
        - Tag: `// Feature: historical-weather, Property 1: Historical query result contains the requested location`
        - Minimum 100 iterations
        - _Requirements: Condition C_

    - [~] 6.2 Write property test for Property 2 — days fall within requested date range
        - Add to `tests/WeatherApp.Infrastructure.Tests/Services/WeatherApiServiceHistoricalTests.cs` (or a dedicated PBT file)
        - Generate random date ranges and mock API responses; assert every `WeatherForecastDay.Date` satisfies `startDate <= day.Date <= endDate`
        - Tag: `// Feature: historical-weather, Property 2: Historical query result days fall within the requested date range`
        - Minimum 100 iterations
        - _Requirements: Condition C_

    - [~] 6.3 Write property test for Property 3 — view renders all forecast days
        - Add to `tests/WeatherApp.Web.Tests/Controllers/HistoricalWeatherControllerTests.cs` (or a dedicated PBT file)
        - Generate random `WeatherForecast` instances with N days (N ∈ [0, 30]); render `Index.cshtml` and assert exactly N data rows appear
        - Tag: `// Feature: historical-weather, Property 3: View renders all forecast days`
        - Minimum 100 iterations
        - _Requirements: Condition C_

- [~] 7. Final checkpoint — ensure all tests pass
    - Ensure all tests pass, ask the user if questions arise.
