# MyWebAPI

ASP.NET Core 8.0 Web API avec contrôleurs Health et WeatherForecast.

## Démarrage

dotnet run

text

Accès à l'API: `http://localhost:5137`
Documentation Swagger: `http://localhost:5137/swagger`

## Endpoints

- `GET /Health` - Statut de santé de l'API
- `GET /Health/detailed` - Diagnostics détaillés
- `GET /Health/ping` - Test de connectivité
- `GET /WeatherForecast` - Prévisions météo (5 jours)
