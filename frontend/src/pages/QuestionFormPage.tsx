import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import type { QuestionDetails } from '../questions'
import { topicOptions, type Topic } from '../topics'

export function QuestionFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { session } = useAuth()
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
      .catch(reason => setError(reason instanceof ApiError ? reason.message : 'Не получилось открыть вопрос.'))
  }, [id, session?.token])

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
      setError(reason instanceof ApiError ? reason.message : 'Не получилось сохранить вопрос.')
    }
  }

  return (
    <>
      <h1>{id ? 'Правка вопроса' : 'Новый вопрос'}</h1>
      <form onSubmit={onSubmit}>
        <label>
          Заголовок
          <input value={title} onChange={event => setTitle(event.target.value)} required maxLength={120} />
        </label>
        <label>
          Тема
          <select value={topic} onChange={event => setTopic(event.target.value as Topic)}>
            {topicOptions.map(item => <option key={item.value} value={item.value}>{item.label}</option>)}
          </select>
        </label>
        <label>
          Текст
          <textarea value={body} onChange={event => setBody(event.target.value)} required maxLength={5000} />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <button type="submit">Сохранить</button>
      </form>
    </>
  )
}
