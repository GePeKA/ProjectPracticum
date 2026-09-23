# База данных

PostgreSQL. Схема описана классами EF Core в `Sprosi.Data` и первой миграцией `InitialCreate`. Имена таблиц и колонок в snake_case.

## Таблицы

`users` — учётная запись: `id`, `email` (уникальный), `display_name`, `password_hash`, `created_at`.

`questions` — вопрос: `id`, `author_id`, `title`, `body`, `topic`, `created_at`, `updated_at`. Тема хранится числом: 1 учёба, 2 быт, 3 город, 4 техника, 5 другое.

`answers` — ответ: `id`, `question_id`, `author_id`, `body`, `is_accepted`, `created_at`, `updated_at`.

Удаление вопроса удаляет его ответы. Удаление пользователя, на которого ещё ссылаются записи, база не разрешает.

Индекс `answers_one_accepted` не даёт отметить лучшим больше одного ответа на вопрос: уникален `question_id` среди строк, где `is_accepted = true`.

Моменты времени — `timestamptz`.

## Миграции

Инструмент `dotnet-ef` зафиксирован в `dotnet-tools.json`. Для применения схемы к локальной базе `sprosi` (`localhost:5432`, пользователь `postgres`, пароль `admin`):

```powershell
dotnet ef database update --project src/Sprosi.Data --startup-project src/Sprosi.Data
```

Та же строка записана в `appsettings.json` и в `AppDbContextFactory`.
