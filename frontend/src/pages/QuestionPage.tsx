import { useEffect, useState, type FormEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import { useLocale } from '../locale'
import type { QuestionDetails } from '../questions'

export function QuestionPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { session } = useAuth()
  const { t, formatWhen, topicLabel } = useLocale()
  const [question, setQuestion] = useState<QuestionDetails | null>(null)
  const [answer, setAnswer] = useState('')
  const [editingId, setEditingId] = useState<string | null>(null)
  const [draft, setDraft] = useState('')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!id)
      return

    let ignore = false
    api<QuestionDetails>(`/api/questions/${id}`, {}, session?.token)
      .then(result => {
        if (!ignore)
          setQuestion(result)
      })
      .catch(reason => {
        if (!ignore)
          setError(reason instanceof ApiError ? reason.message : t('openFailed'))
      })

    return () => {
      ignore = true
    }
  }, [id, session?.token, t])

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
      setError(reason instanceof ApiError ? reason.message : t('changeFailed'))
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
      setError(reason instanceof ApiError ? reason.message : t('deleteFailed'))
    }
  }

  if (!question)
    return error ? <p className="error">{error}</p> : <p className="lead">{t('loading')}</p>

  return (
    <>
      <p className="mark">{topicLabel(question.topic)}</p>
      <h1>{question.title}</h1>
      <p className="quiet">{question.authorDisplayName} · {formatWhen(question.createdAt)}</p>
      <p className="body">{question.body}</p>
      {question.canEdit ? (
        <div className="form-actions">
          <Link to={`/questions/${question.id}/edit`}>{t('edit')}</Link>
          <button type="button" className="text-button" onClick={removeQuestion}>{t('delete')}</button>
        </div>
      ) : null}
      <h2>{t('answers')}</h2>
      {question.answers.length === 0 ? <p className="lead">{t('noAnswers')}</p> : null}
      <div className="cards">
        {question.answers.map(item => (
          <article className={item.isAccepted ? 'card best' : 'card'} key={item.id}>
            {item.isAccepted ? <p className="best-label">{t('bestAnswer')}</p> : null}
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
                  <button type="submit">{t('save')}</button>
                  <button type="button" className="text-button" onClick={() => setEditingId(null)}>{t('cancel')}</button>
                </div>
              </form>
            ) : <p className="body">{item.body}</p>}
            <p className="quiet">{item.authorDisplayName} · {formatWhen(item.createdAt)}</p>
            <div className="form-actions">
              {item.canEdit ? (
                <>
                  <button type="button" className="text-button" onClick={() => { setEditingId(item.id); setDraft(item.body) }}>{t('edit')}</button>
                  <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}`, { method: 'DELETE' })}>{t('delete')}</button>
                </>
              ) : null}
              {question.canEdit && !item.canEdit && !item.isAccepted ? (
                <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}/accept`, { method: 'POST' })}>{t('markBest')}</button>
              ) : null}
              {question.canEdit && item.isAccepted ? (
                <button type="button" className="text-button" onClick={() => void change(`/api/answers/${item.id}/accept`, { method: 'DELETE' })}>{t('clearMark')}</button>
              ) : null}
            </div>
          </article>
        ))}
      </div>
      {session ? (
        <form onSubmit={event => void onAnswer(event)}>
          <label>
            {t('yourAnswer')}
            <textarea value={answer} onChange={event => setAnswer(event.target.value)} required />
          </label>
          <button type="submit">{t('reply')}</button>
        </form>
      ) : <p className="lead"><Link to="/login">{t('signInToReply')}</Link>{t('signInToReplyTail')}</p>}
      {error ? <p className="error">{error}</p> : null}
    </>
  )
}
