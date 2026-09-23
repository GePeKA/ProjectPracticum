# Локальный запуск API

Нужны .NET SDK 10, Node.js и PostgreSQL на `localhost:5432`. База `sprosi` создаётся при первом запуске, если сервер PostgreSQL уже принимает подключения пользователя `postgres` с паролем `admin`. Строка подключения и ключ JWT лежат в `src/Sprosi.Api/appsettings.json`.

```powershell
dotnet run --project src/Sprosi.Api
```

При старте API само применяет миграции. Слушает `http://localhost:5080`. Проверка жизни: `GET /health`.

Как устроены таблицы, написано в [database.md](database.md).

Интерфейс:

```powershell
npm install --prefix frontend
npm run dev --prefix frontend
```

Страница открывается на `http://localhost:5173` и проксирует `/api` на API. Если интерфейс ходит на API напрямую, адрес задаётся переменной `VITE_API_URL`.
