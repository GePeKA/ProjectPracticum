import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ApiError, api } from '../api'
import { useAuth } from '../auth'
import { useLocale } from '../locale'
import type { QuestionPage } from '../questions'
import { topicValues } from '../topics'

export function QuestionListPage() {
  const { session } = useAuth()
  const { t, formatWhen, topicLabel, answersLabel } = useLocale()
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
          setError(reason instanceof ApiError ? reason.message : t('loadFailed'))
      })

    return () => {
      ignore = true
    }
  }, [topic, status, sort, search, page, t])

  const totalPages = data ? Math.max(1, Math.ceil(data.total / data.pageSize)) : 1

  return (
    <>
      <div className="page-head">
        <div>
          <p className="mark">{t('feed')}</p>
          <h1>{t('questions')}</h1>
        </div>
        {session ? <Link className="button-link" to="/questions/new">{t('ask')}</Link> : null}
      </div>
      <div className="filters">
        <input
          placeholder={t('search')}
          value={search}
          onChange={event => {
            setPage(1)
            setSearch(event.target.value)
          }}
        />
        <select value={topic} onChange={event => { setPage(1); setTopic(event.target.value) }}>
          <option value="">{t('allTopics')}</option>
          {topicValues.map(value => <option key={value} value={value}>{topicLabel(value)}</option>)}
        </select>
        <select value={status} onChange={event => { setPage(1); setStatus(event.target.value) }}>
          <option value="all">{t('all')}</option>
          <option value="open">{t('waiting')}</option>
          <option value="resolved">{t('resolved')}</option>
        </select>
        <select value={sort} onChange={event => { setPage(1); setSort(event.target.value) }}>
          <option value="new">{t('sortNew')}</option>
          <option value="old">{t('sortOld')}</option>
          <option value="popular">{t('sortPopular')}</option>
        </select>
      </div>
      {error ? <p className="error">{error}</p> : null}
      <div className="cards">
        {data?.items.length === 0 ? <p className="lead">{t('empty')}</p> : null}
        {data?.items.map(item => (
          <article className="card" key={item.id}>
            <div className="card-meta">
              <span>{topicLabel(item.topic)}</span>
              <span>{item.hasAcceptedAnswer ? t('resolvedShort') : t('waitingShort')}</span>
            </div>
            <h2><Link to={`/questions/${item.id}`}>{item.title}</Link></h2>
            <p className="quiet">
              {item.authorDisplayName}
              {' · '}
              {formatWhen(item.createdAt)}
              {' · '}
              {item.answerCount} {answersLabel(item.answerCount)}
            </p>
          </article>
        ))}
      </div>
      {data && data.total > data.pageSize ? (
        <div className="form-actions">
          <button type="button" disabled={page <= 1} onClick={() => setPage(current => current - 1)}>{t('back')}</button>
          <span className="quiet">{page} {t('of')} {totalPages}</span>
          <button type="button" disabled={page >= totalPages} onClick={() => setPage(current => current + 1)}>{t('next')}</button>
        </div>
      ) : null}
    </>
  )
}
