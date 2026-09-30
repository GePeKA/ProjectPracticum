# Спроси

Публичный сервис вопросов и ответов. Гость читает ленту. Вошедший пользователь задаёт вопросы и отвечает. Автор вопроса отмечает один чужой ответ как лучший — после этого вопрос считается отвеченным.

Темы: учёба, быт, город, техника, другое.

## Стек

- Backend: .NET 10, EF Core, PostgreSQL, JWT
- Frontend: React, Vite, TypeScript
- Архитектура: Domain → Application → Data / Infrastructure → Api

## Быстрый старт

Нужны .NET SDK 10, Node.js и PostgreSQL на `localhost:5432` (пользователь `postgres`, пароль `admin`). База `sprosi` создаётся при первом запуске API.

```powershell
dotnet run --project src/Sprosi.Api
npm install --prefix frontend
npm run dev --prefix frontend
```

- API: `http://localhost:5080` (`GET /health`)
- Интерфейс: `http://localhost:5173` (проксирует `/api` на API)

Подробности — в [docs/development.md](docs/development.md).

## Документация

| Файл | О чём |
|---|---|
| [docs/architecture.md](docs/architecture.md) | Предметная область |
| [docs/conventions.md](docs/conventions.md) | Слои проектов, коммиты, ветки, PR |
| [docs/database.md](docs/database.md) | Таблицы PostgreSQL и миграции |
| [docs/api.md](docs/api.md) | HTTP API, ответы и коды ошибок |
| [docs/frontend.md](docs/frontend.md) | Экраны интерфейса |
| [docs/locale.md](docs/locale.md) | Язык интерфейса и часовой пояс |
| [docs/development.md](docs/development.md) | Локальный запуск, тесты, CI |
