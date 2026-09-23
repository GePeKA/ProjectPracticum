import { Link, NavLink, Outlet } from 'react-router-dom'
import { useAuth } from './auth'
import { useLocale } from './locale'

export function Layout() {
  const { session, setSession } = useAuth()
  const { t } = useLocale()

  return (
    <>
      <header className="top">
        <Link to="/" className="brand">Спроси</Link>
        <nav>
          <NavLink to="/settings">{t('settings')}</NavLink>
          {session ? (
            <>
              <span className="who">{session.displayName}</span>
              <button type="button" className="text-button" onClick={() => setSession(null)}>{t('signOut')}</button>
            </>
          ) : (
            <>
              <NavLink to="/login">{t('signIn')}</NavLink>
              <NavLink to="/register">{t('register')}</NavLink>
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
