import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import { useLocale } from '../locale'
import type { QuestionDetails } from '../questions'
import { topicValues, type Topic } from '../topics'

export function QuestionFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { session } = useAuth()
  const { t, topicLabel } = useLocale()
  const [title, setTitle] = useState('')
  const [body, setBody] = useState('')
  const [topic, setTopic] = useState<Topic>('Study')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!id)
      return

    api<QuestionDetails>(`/api/questions/${id}`, {}, session?.token)
      .then(question => {
        setTitle(question.title)
        setBody(question.body)
        setTopic(question.topic)
      })
      .catch(reason => setError(reason instanceof ApiError ? reason.message : t('openFailed')))
  }, [id, session?.token, t])

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    if (!session) {
      navigate('/login')
      return
    }

    setError(null)
    try {
      const question = await api<QuestionDetails>(id ? `/api/questions/${id}` : '/api/questions', {
        method: id ? 'PUT' : 'POST',
        body: JSON.stringify({ title, body, topic }),
      }, session.token)
      navigate(`/questions/${question.id}`)
    }
    catch (reason) {
      setError(reason instanceof ApiError ? reason.message : t('saveFailed'))
    }
  }

  return (
    <>
      <h1>{id ? t('editQuestion') : t('newQuestion')}</h1>
      <form onSubmit={onSubmit}>
        <label>
          {t('title')}
          <input value={title} onChange={event => setTitle(event.target.value)} required maxLength={120} />
        </label>
        <label>
          {t('topic')}
          <select value={topic} onChange={event => setTopic(event.target.value as Topic)}>
            {topicValues.map(value => <option key={value} value={value}>{topicLabel(value)}</option>)}
          </select>
        </label>
        <label>
          {t('text')}
          <textarea value={body} onChange={event => setBody(event.target.value)} required maxLength={5000} />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <button type="submit">{t('save')}</button>
      </form>
    </>
  )
}
