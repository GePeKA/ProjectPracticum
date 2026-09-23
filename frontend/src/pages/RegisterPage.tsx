import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError, api, type AuthResult } from '../api'
import { useAuth } from '../auth'
import { useLocale } from '../locale'

export function RegisterPage() {
  const { setSession } = useAuth()
  const { t } = useLocale()
  const navigate = useNavigate()
  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    try {
      const result = await api<AuthResult>('/api/auth/register', {
        method: 'POST',
        body: JSON.stringify({ email, password, displayName }),
      })
      setSession(result)
      navigate('/')
    }
    catch (reason) {
      setError(reason instanceof ApiError ? reason.message : t('registerFailed'))
    }
  }

  return (
    <>
      <h1>{t('register')}</h1>
      <form onSubmit={onSubmit}>
        <label>
          {t('name')}
          <input value={displayName} onChange={event => setDisplayName(event.target.value)} required />
        </label>
        <label>
          Email
          <input type="email" value={email} onChange={event => setEmail(event.target.value)} required />
        </label>
        <label>
          {t('password')}
          <input type="password" value={password} onChange={event => setPassword(event.target.value)} minLength={8} required />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <div className="form-actions">
          <button type="submit">{t('createAccount')}</button>
          <Link to="/login">{t('haveAccount')}</Link>
        </div>
      </form>
    </>
  )
}
