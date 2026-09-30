# HTTP API

Ошибки приходят как `{ "code": "...", "message": "..." }`. Текст `message` зависит от заголовка `Accept-Language`: `en` даёт английский, иначе русский. Код ошибки от языка не зависит.

Токен живёт 24 часа. В нём три поля: `sub` (идентификатор пользователя), `email`, `name` (отображаемое имя).

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

Параметры: `topic` (`Study`, `Everyday`, `City`, `Tech`, `Other`), `status` (`all`, `open`, `resolved`), `sort` (`new`, `old`, `popular`), `q` (фрагмент заголовка), `page`, `pageSize` (не больше 50). В ответах `topic` приходит тем же именем, не числом.

`open` — ещё нет лучшего ответа, `resolved` — лучший ответ выбран.

Ответ `200`:

```json
{
  "items": [
    {
      "id": "...",
      "title": "Как доехать до вокзала?",
      "topic": "City",
      "authorDisplayName": "Ада",
      "createdAt": "2026-03-20T10:00:00Z",
      "answerCount": 2,
      "hasAcceptedAnswer": false
    }
  ],
  "page": 1,
  "pageSize": 20,
  "total": 1
}
```

`GET /api/questions/{id}` возвращает вопрос и уже существующие ответы. Лучший ответ стоит первым. Поле `canEdit` истинно только для автора записи, если запрос пришёл с токеном.

Ответ `200`:

```json
{
  "id": "...",
  "title": "Как доехать до вокзала?",
  "body": "Нужен короткий маршрут.",
  "topic": "City",
  "authorDisplayName": "Ада",
  "createdAt": "2026-03-20T10:00:00Z",
  "updatedAt": "2026-03-20T10:00:00Z",
  "answerCount": 1,
  "hasAcceptedAnswer": true,
  "canEdit": false,
  "answers": [
    {
      "id": "...",
      "body": "На трамвае до конца линии.",
      "authorDisplayName": "Боб",
      "isAccepted": true,
      "createdAt": "2026-03-20T11:00:00Z",
      "updatedAt": "2026-03-20T11:00:00Z",
      "canEdit": false
    }
  ]
}
```

`POST /api/questions`, `PUT /api/questions/{id}` и `DELETE /api/questions/{id}` доступны только вошедшему пользователю. Чужой вопрос изменить нельзя. Тело создания и правки:

```json
{ "title": "Как варить рис?", "body": "Нужен короткий совет.", "topic": "Everyday" }
```

Создание и правка отвечают карточкой вопроса целиком (как `GET /api/questions/{id}`). Удаление отвечает `204`. Вместе с вопросом удаляются его ответы.

## Ответы

Все запросы ниже требуют токен. В ответ приходит карточка вопроса целиком.

`POST /api/questions/{id}/answers`

```json
{ "body": "На трамвае до конца линии." }
```

Текст обязателен и не длиннее 5000 символов. Один человек может ответить несколько раз.

`PUT /api/answers/{id}` меняет свой ответ, тело то же. `DELETE /api/answers/{id}` удаляет свой ответ. Чужой ответ изменить нельзя.

`POST /api/answers/{id}/accept` отмечает ответ лучшим. Это может сделать только автор вопроса, и только чужой ответ. Предыдущая отметка снимается. `DELETE /api/answers/{id}/accept` снимает отметку. После этого вопрос снова в состоянии «ждёт ответа».

## Коды ошибок

Поле `code` стабильно. Ниже — значение и смысл; текст `message` зависит от языка.

| code | Когда |
|---|---|
| `credentials_required` | Не заполнены email, имя или пароль |
| `email_invalid` | Некорректный email |
| `display_name_too_long` | Имя длиннее 50 символов |
| `password_too_short` | Пароль короче 8 символов |
| `email_taken` | Email уже зарегистрирован |
| `invalid_credentials` | Неверная пара email / пароль |
| `sign_in_required` | Нужен токен |
| `question_not_found` | Вопрос не найден |
| `question_edit_forbidden` | Чужой вопрос нельзя менять |
| `question_delete_forbidden` | Чужой вопрос нельзя удалить |
| `question_fields_required` | Пустые заголовок или текст |
| `title_too_long` | Заголовок длиннее 120 символов |
| `question_body_too_long` | Текст вопроса длиннее 5000 символов |
| `topic_required` | Тема не указана |
| `topic_unknown` | Неизвестная тема |
| `status_unknown` | Неизвестный фильтр `status` |
| `sort_unknown` | Неизвестная сортировка |
| `answer_not_found` | Ответ не найден |
| `answer_edit_forbidden` | Чужой ответ нельзя менять |
| `answer_delete_forbidden` | Чужой ответ нельзя удалить |
| `accept_forbidden` | Отметить лучший может только автор вопроса |
| `accept_own_answer` | Нельзя отметить свой ответ |
| `clear_forbidden` | Снять отметку может только автор вопроса |
| `answer_not_accepted` | Ответ не отмечен лучшим |
| `answer_required` | Пустой текст ответа |
| `answer_too_long` | Текст ответа длиннее 5000 символов |
