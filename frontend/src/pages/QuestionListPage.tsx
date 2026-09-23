import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import type { QuestionPage } from '../questions'
import { formatWhen, topicLabel, topicOptions } from '../topics'

export function QuestionListPage() {
  const { session } = useAuth()
  const [topic, setTopic] = useState('')
  const [status, setStatus] = useState('all')
  const [sort, setSort] = useState('new')
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [data, setData] = useState<QuestionPage | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const params = new URLSearchParams({ status, sort, page: String(page), pageSize: '20' })
    if (topic)
      params.set('topic', topic)
    if (search.trim())
      params.set('q', search.trim())

    let ignore = false
    api<QuestionPage>(`/api/questions?${params.toString()}`)
      .then(result => {
        if (!ignore)
          setData(result)
      })
      .catch(reason => {
        if (!ignore)
          setError(reason instanceof ApiError ? reason.message : 'Не получилось загрузить вопросы.')
      })

    return () => {
      ignore = true
    }
  }, [topic, status, sort, search, page])

  const totalPages = data ? Math.max(1, Math.ceil(data.total / data.pageSize)) : 1

  return (
    <>
      <div className="page-head">
        <div>
          <p className="mark">Лента</p>
          <h1>Вопросы</h1>
        </div>
        {session ? <Link className="button-link" to="/questions/new">Задать вопрос</Link> : null}
      </div>
      <div className="filters">
        <input
          placeholder="Поиск по заголовку"
          value={search}
          onChange={event => {
            setPage(1)
            setSearch(event.target.value)
          }}
        />
        <select value={topic} onChange={event => { setPage(1); setTopic(event.target.value) }}>
          <option value="">Все темы</option>
          {topicOptions.map(item => <option key={item.value} value={item.value}>{item.label}</option>)}
        </select>
        <select value={status} onChange={event => { setPage(1); setStatus(event.target.value) }}>
          <option value="all">Все</option>
          <option value="open">Ждут ответа</option>
          <option value="resolved">Есть лучший ответ</option>
        </select>
        <select value={sort} onChange={event => { setPage(1); setSort(event.target.value) }}>
          <option value="new">Сначала новые</option>
          <option value="old">Сначала старые</option>
          <option value="popular">Больше ответов</option>
        </select>
      </div>
      {error ? <p className="error">{error}</p> : null}
      <div className="cards">
        {data?.items.length === 0 ? <p className="lead">Пока ничего не нашлось.</p> : null}
        {data?.items.map(item => (
          <article className="card" key={item.id}>
            <div className="card-meta">
              <span>{topicLabel(item.topic)}</span>
              <span>{item.hasAcceptedAnswer ? 'Есть лучший ответ' : 'Ждёт ответа'}</span>
            </div>
            <h2><Link to={`/questions/${item.id}`}>{item.title}</Link></h2>
            <p className="quiet">
              {item.authorDisplayName}
              {' · '}
              {formatWhen(item.createdAt)}
              {' · '}
              {item.answerCount} {answersWord(item.answerCount)}
            </p>
          </article>
        ))}
      </div>
      {data && data.total > data.pageSize ? (
        <div className="form-actions">
          <button type="button" disabled={page <= 1} onClick={() => setPage(current => current - 1)}>Назад</button>
          <span className="quiet">{page} из {totalPages}</span>
          <button type="button" disabled={page >= totalPages} onClick={() => setPage(current => current + 1)}>Дальше</button>
        </div>
      ) : null}
    </>
  )
}

function answersWord(count: number) {
  const mod10 = count % 10
  const mod100 = count % 100
  if (mod10 === 1 && mod100 !== 11)
    return 'ответ'
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14))
    return 'ответа'
  return 'ответов'
}
