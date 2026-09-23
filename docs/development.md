# Локальный запуск API

Нужны .NET SDK 10 и PostgreSQL. Строка подключения и ключ JWT в репозиторий не входят, их кладут в user secrets проекта API.

```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=sprosi;Username=postgres;Password=postgres" --project src/Sprosi.Api
dotnet user-secrets set "Jwt:Key" "dev-only-key-change-me-32chars-min" --project src/Sprosi.Api
dotnet ef database update --project src/Sprosi.Data --startup-project src/Sprosi.Data
dotnet run --project src/Sprosi.Api
```

API слушает `http://localhost:5080`. Проверка жизни: `GET /health`.

Ключ JWT короче 32 символов процесс не запускает. Как устроены таблицы, написано в [database.md](database.md).
