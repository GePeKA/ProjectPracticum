import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import type { QuestionDetails } from '../questions'
import { formatWhen, topicLabel } from '../topics'

export function QuestionPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { session } = useAuth()
  const [question, setQuestion] = useState<QuestionDetails | null>(null)
  const [answer, setAnswer] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [draft, setDraft] = useState('')
  const [error, setError] = useState<string | null>(null)

  function load() {
    if (!id)
      return

    api<QuestionDetails>(`/api/questions/${id}`, {}, session?.token)
      .then(setQuestion)
      .catch(reason => setError(reason instanceof ApiError ? reason.message : 'Не получилось открыть вопрос.'))
  }

  useEffect(() => {
    load()
  }, [id, session?.token])

  async function change(path: string, options: RequestInit) {
    if (!session)
      return

    setError(null)
    try {
      const updated = await api<QuestionDetails>(path, options, session.token)
      setQuestion(updated)
      setAnswer('')
      setEditingId(null)
    }
    catch (reason) {
      setError(reason instanceof ApiError ? reason.message : 'Не получилось сохранить изменение.')
    }
  }

  async function onAnswer(event: FormEvent) {
    event.preventDefault()
    await change(`/api/questions/${id}/answers`, {
      method: 'POST',
      body: JSON.stringify({ body: answer }),
    })
  }

  async function removeQuestion() {
    if (!session || !id)
      return

    setError(null)
    try {
      await api(`/api/questions/${id}`, { method: 'DELETE' }, session.token)
      navigate('/')
    }
    catch (reason) {
      setError(reason instanceof ApiError ? reason.message : 'Не получилось удалить вопрос.')
    }
  }

  if (!question)
    return error ? <p className="error">{error}</p> : <p className="lead">Загрузка…</p>

  return (
    <>
      <p className="mark">{topicLabel(question.topic)}</p>
      <h1>{question.title}</h1>
      <p className="quiet">{question.authorDisplayName} · {formatWhen(question.createdAt)}</p>
      <p className="body">{question.body}</p>
      {question.canEdit ? (
        <div className="form-actions">
          <Link to={`/questions/${question.id}/edit`}>Изменить</Link>
          <button type="button" className="text-button" onClick={removeQuestion}>Удалить</button>
        </div>
      ) : null}
      <h2>Ответы</h2>
      {question.answers.length === 0 ? <p className="lead">Пока никто не ответил.</p> : null}
      <div className="cards">
        {question.answers.map(item => (
          <article className={item.isAccepted ? 'card best' : 'card'} key={item.id}>
            {item.isAccepted ? <p className="best-label">Лучший ответ</p> : null}
            {editingId === item.id ? (
              <form onSubmit={event => {
                event.preventDefault()
                void change(`/api/answers/${item.id}`, {
                  method: 'PUT',
                  body: JSON.stringify({ body: draft }),
                })
              }}
              >
                <textarea value={draft} onChange={event => setDraft(event.target.value)} />
                <div className="form-actions">
                  <button type="submit">Сохранить</button>
                  <button type="button" className="text-button" onClick={() => setEditingId(null)}>Отмена</button>
                </div>
              </form>
            ) : <p className="body">{item.body}</p>}
            <p className="quiet">{item.authorDisplayName} · {formatWhen(item.createdAt)}</p>
            <div className="form-actions">
              {item.canEdit ? (
                <>
                  <button type="button" className="text-button" onClick={() => { setEditingId(item.id); setDraft(item.body) }}>Изменить</button>
                  <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}`, { method: 'DELETE' })}>Удалить</button>
                </>
              ) : null}
              {question.canEdit && !item.canEdit && !item.isAccepted ? (
                <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}/accept`, { method: 'POST' })}>Отметить лучшим</button>
              ) : null}
              {question.canEdit && item.isAccepted ? (
                <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}/accept`, { method: 'DELETE' })}>Снять отметку</button>
              ) : null}
            </div>
          </article>
        ))}
      </div>
      {session ? (
        <form onSubmit={event => void onAnswer(event)}>
          <label>
            Ваш ответ
            <textarea value={answer} onChange={event => setAnswer(event.target.value)} required />
          </label>
          <button type="submit">Ответить</button>
        </form>
      ) : <p className="lead"><Link to="/login">Войдите</Link>, чтобы ответить.</p>}
      {error ? <p className="error">{error}</p> : null}
    </>
  )
}
