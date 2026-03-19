# Clean Architecture Weather Solution

This plan outlines the creation of a new ASP.NET Core 10 Solution using Clean Architecture principles, specifically separating the codebase into Domain, Application, Infrastructure, and Web layers.

## Proposed Changes

We will create a root directory `WeatherApp` and initialize a new solution along with 4 projects.

---

### Solution and Projects Setup
- Create `WeatherApp.sln`
- Create `WeatherApp.Domain` (Class Library)
- Create `WeatherApp.Application` (Class Library)
- Create `WeatherApp.Infrastructure` (Class Library)
- Create `WeatherApp.Web` (MVC App)

#### Project References
- `WeatherApp.Application` references `WeatherApp.Domain`
- `WeatherApp.Infrastructure` references `WeatherApp.Application`
- `WeatherApp.Web` references `WeatherApp.Application` and `WeatherApp.Infrastructure`

---

### Domain Layer
Defines the enterprise logic and contracts.
- **[NEW]** `WeatherApp.Domain/Entities/Weather.cs`: Represents the weather data (e.g., Temperature, Description, Location).
- **[NEW]** `WeatherApp.Domain/Interfaces/IWeatherService.cs`: Contract for fetching weather data `Task<Weather> GetCurrentWeatherAsync(string location)`.

---

### Application Layer
Contains business use cases and orchestrates tasks using MediatR.
- **Dependencies**: Add `MediatR` NuGet package.
- **[NEW]** `WeatherApp.Application/Weather/Queries/GetWeatherQuery.cs`: A record/class representing the request for weather data `IRequest<Weather>`.
- **[NEW]** `WeatherApp.Application/Weather/Queries/GetWeatherQueryHandler.cs`: Handles the query by calling `IWeatherService`.

---

### Infrastructure Layer
Implements interfaces defined in the Application/Domain layer, communicating with external systems.
- **[NEW]** `WeatherApp.Infrastructure/Services/WeatherApiService.cs`: Implements `IWeatherService`. Uses `IHttpClientFactory` to call an external weather API (e.g., weatherapi.com) and deserializes the JSON response into the `Weather` entity.
- **[NEW]** `WeatherApp.Infrastructure/DependencyInjection.cs`: Extension method to register Infrastructure services (e.g., `services.AddHttpClient<IWeatherService, WeatherApiService>()`).

---

### Web Layer (MVC)
The entry point of the application.
- **[MODIFY]** `WeatherApp.Web/appsettings.json`: Add configuration for the Weather API:
  ```json
  "WeatherApi": {
    "BaseUrl": "https://api.weatherapi.com/v1/",
    "ApiKey": "YOUR_API_KEY"
  }
  ```
- **[MODIFY]** `WeatherApp.Web/Program.cs`: Wire up dependency injection (register MediatR, call Infrastructure's DI configuration).
- **[NEW]** `WeatherApp.Web/Controllers/WeatherController.cs`: Injects `IMediator`, sends `GetWeatherQuery`, and returns the `Index` view.
- **[NEW]** `WeatherApp.Web/Views/Weather/Index.cshtml`: Displays the temperature and weather description.

## Verification Plan

### Automated Verification
- Run `dotnet build` from the solution root to ensure all projects compile successfully with zero errors.

### Manual Verification
- Run `dotnet run --project WeatherApp.Web` and navigate to `/Weather` (or use the browser tool if applicable) to ensure the page loads and attempts to retrieve weather data (it may fail gracefully if an invalid API key is used, which is expected for a template, but the DI and application flow will be verified).
