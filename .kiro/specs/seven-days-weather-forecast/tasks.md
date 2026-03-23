# Implementation Plan

- [ ] 1. Modify WeatherApiService.cs
    - Create a new method to get 7 days weather forecast
    - Implement JSON deserialization of WeatherForecastDay list

- [ ] 1.1 Write backend code to get 7 days weather forecast
    - Create new method to get 7 days weather forecast from weatherapi.com API in WeatherApiService.cs
    - Create new model to handle weather forecast API response in the infrastructure layer
    - Create new query and handler in the application layer
    - Create new entities in domain layer
    - Write unit tests for application and infrastructure
    - Ensure test coverage is 80%

- [ ] 1.2 Write frontend handling code to get 7 days weather forecast in the web layer
    - Create a new page to get 7 days weather forecast from weatherapi.com API
    - Create a new controller to handle weather forecast input and output
    - Write unit tests for web layer
    - Ensure test coverage is 80%
