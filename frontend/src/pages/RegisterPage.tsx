import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { ApiError, api, type AuthResult } from '../api'
import { useAuth } from '../auth'

export function RegisterPage() {
  const { setSession } = useAuth()
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
      setError(reason instanceof ApiError ? reason.message : 'Не получилось зарегистрироваться.')
    }
  }

  return (
    <>
      <h1>Регистрация</h1>
      <form onSubmit={onSubmit}>
        <label>
          Имя
          <input value={displayName} onChange={event => setDisplayName(event.target.value)} required />
        </label>
        <label>
          Email
          <input type="email" value={email} onChange={event => setEmail(event.target.value)} required />
        </label>
        <label>
          Пароль
          <input type="password" value={password} onChange={event => setPassword(event.target.value)} minLength={8} required />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <div className="form-actions">
          <button type="submit">Создать аккаунт</button>
          <Link to="/login">Уже есть аккаунт</Link>
        </div>
      </form>
    </>
  )
}
