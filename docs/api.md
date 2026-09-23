# HTTP API

Ошибки приходят как `{ "message": "..." }`.

Пока описаны только регистрация и вход. Токен живёт 24 часа. В нём три поля: `sub` (идентификатор пользователя), `email`, `name` (отображаемое имя).

## Регистрация

`POST /api/auth/register`

```json
{ "email": "ada@example.com", "password": "password1", "displayName": "Ада" }
```

Ответ `200`:

```json
{ "userId": "...", "displayName": "Ада", "token": "..." }
```

Пароль не короче 8 символов, имя не длиннее 50. Email приводится к нижнему регистру. Повторный email отвечает `409`.

## Вход

`POST /api/auth/login`

```json
{ "email": "ada@example.com", "password": "password1" }
```

Ответ такой же, как у регистрации. Неверная пара отвечает `401` и текстом «Неверный email или пароль.»

Защищённые запросы передают заголовок `Authorization: Bearer <token>`.

## Вопросы

`GET /api/questions`

Параметры: `topic` (`Study`, `Everyday`, `City`, `Tech`, `Other`), `status` (`all`, `open`, `resolved`), `sort` (`new`, `old`, `popular`), `q` (фрагмент заголовка), `page`, `pageSize` (не больше 50).

`open` — ещё нет лучшего ответа, `resolved` — лучший ответ выбран. В каждой строке есть `answerCount` и `hasAcceptedAnswer`.

`GET /api/questions/{id}` возвращает вопрос и уже существующие ответы. Лучший ответ стоит первым. Поле `canEdit` истинно только для автора записи, если запрос пришёл с токеном.

`POST /api/questions`, `PUT /api/questions/{id}` и `DELETE /api/questions/{id}` доступны только вошедшему пользователю. Чужой вопрос изменить нельзя. Тело создания и правки:

```json
{ "title": "Как варить рис?", "body": "Нужен короткий совет.", "topic": "Everyday" }
```

Удаление отвечает `204`. Вместе с вопросом удаляются его ответы.
