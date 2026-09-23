import { Link, NavLink, Outlet } from 'react-router-dom'
import { useAuth } from './auth'

export function Layout() {
  const { session, setSession } = useAuth()

  return (
    <>
      <header className="top">
        <Link to="/" className="brand">Спроси</Link>
        <nav>
          {session ? (
            <>
              <span className="who">{session.displayName}</span>
              <button type="button" className="text-button" onClick={() => setSession(null)}>Выйти</button>
            </>
          ) : (
            <>
              <NavLink to="/login">Войти</NavLink>
              <NavLink to="/register">Регистрация</NavLink>
            </>
          )}
        </nav>
      </header>
      <main className="shell">
        <Outlet />
      </main>
    </>
  )
}
