import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError, api, type AuthResult } from '../api'
import { useAuth } from '../auth'
import { useLocale } from '../locale'

export function LoginPage() {
  const { setSession } = useAuth()
  const { t } = useLocale()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)

  async function onSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    try {
      const result = await api<AuthResult>('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
      })
      setSession(result)
      navigate('/')
    }
    catch (reason) {
      setError(reason instanceof ApiError ? reason.message : t('signInFailed'))
    }
  }

  return (
    <>
      <h1>{t('signIn')}</h1>
      <form onSubmit={onSubmit}>
        <label>
          Email
          <input type="email" value={email} onChange={event => setEmail(event.target.value)} required />
        </label>
        <label>
          {t('password')}
          <input type="password" value={password} onChange={event => setPassword(event.target.value)} required />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <div className="form-actions">
          <button type="submit">{t('signIn')}</button>
          <Link to="/register">{t('noAccount')}</Link>
        </div>
      </form>
    </>
  )
}
