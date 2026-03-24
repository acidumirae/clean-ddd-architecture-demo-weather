# Requirements Specification

## Goal
Get the historical weather for a specific date range from the weatherapi.com API using the exisiting ASP.NET Core 10 solution using Clean/Domain-Driven Design Architecture.

## User Stories
*   As a user, I want to be able to navigate to a new page to provide city name input and date range to get city location historical weather information.

## Acceptance Criteria
*   Condition A: There is a link to a separate page to get the historical weather
*   Condition B: There is an input field to accept user target location entry eg. city, and date range entry eg. dt and end_dt parameter and submit button to make the historical weather request
*   Condition C: There is a result page or view to display the historical weather for the entered city during specified date range

## Non-Functional Requirements
*   Accessibility: Must comply with WCAG 2.1 AA.
*   Performance: API response time must be under 200ms.
*   Structure: Update 4 projects: Domain, Application, Infrastructure, and Web (MVC).
*   Domain: Utilize existing a WeatherForecastDay and WeatherForecast entities/value objects and a IWeatherService interface in the Domain layer to define the contract.
*   Infrastructure: Update WeatherService in Infrastructure to fetch current historical temperature data from a weather API (e.g., WeatherAPI.com). Handle JSON deserialization.
*   Application: Create a GetHistoricalWeatherQuery and GetHistoricalWeatherQueryHandler (using MediatR) to orchestrate the call.
*   Web: Create MVC Controller/View to display the historical temperature, injecting the Application handler.
*   Config: Use appsettings.json for API keys and endpoint URLs. Use dependency injection throughout.
