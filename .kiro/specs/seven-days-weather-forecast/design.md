# Technical Design Specification

## Architecture Overview
Solution is ASP.NET Core 10 Solution using Clean/Domain-Driven Design Architecture.
Structure: Has 4 projects: Domain, Application, Infrastructure, and Web (MVC).
Domain: Has a Weather entity/value object and a IWeatherService interface in the Domain layer to define the contract.
Infrastructure: Has WeatherService in Infrastructure using IHttpClientFactory to fetch current temperature data from a weather API (e.g., OpenWeatherMap). Handle JSON deserialization.
Application: Has a GetWeatherQuery and GetWeatherQueryHandler (using MediatR) to orchestrate the call.
Web: Has a Razor Page or MVC Controller/View to display the temperature, injecting the Application handler.
Config: Uses appsettings.json for API keys and endpoint URLs. Uses dependency injection throughout.

## Component Changes
Extend WeatherApiService.cs infrastructure service with a new API call to weatherapi.com API to get the forecast.
Modify Shared view _Layout.cshtml to include link to a new weather forecast page.

### Domain Layer
*   Entities: WeatherForecast, WeatherForecastDay
*   Interfaces: IWeatherService

### Application Layer
*   Use Cases / Handlers: GetWeatherForecastQueryHandler
*   DTOs: GetWeatherForecast

### Infrastructure Layer
*   Implementations: WeatherApiService

### Web / UI Layer
*   Controllers / Components: WeatherForecastController
*   Views: WeatherForecast

## Test Plan
Create xUnit unit tests for Application, Infrastructure and Web projects
