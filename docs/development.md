# Локальный запуск API

Нужны .NET SDK 10 и PostgreSQL на `localhost:5432`. По умолчанию API подключается к базе `sprosi` пользователем `postgres` с паролем `admin`. Строка лежит в `src/Sprosi.Api/appsettings.json`, ключ `ConnectionStrings:Default`.

Ключ JWT в репозиторий не входит:

```powershell
dotnet user-secrets set "Jwt:Key" "dev-only-key-change-me-32chars-min" --project src/Sprosi.Api
dotnet ef database update --project src/Sprosi.Data --startup-project src/Sprosi.Data
dotnet run --project src/Sprosi.Api
```

API слушает `http://localhost:5080`. Проверка жизни: `GET /health`.

Ключ JWT короче 32 символов процесс не запускает. Как устроены таблицы, написано в [database.md](database.md).

Интерфейс:

```powershell
npm install --prefix frontend
npm run dev --prefix frontend
```

Страница открывается на `http://localhost:5173` и проксирует `/api` на API. Если интерфейс ходит на API напрямую, адрес задаётся переменной `VITE_API_URL`.
