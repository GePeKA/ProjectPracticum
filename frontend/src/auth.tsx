import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import type { AuthResult } from './api'

const storageKey = 'sprosi-session'

type AuthContextValue = {
  session: AuthResult | null
  setSession: (session: AuthResult | null) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

function readSession(): AuthResult | null {
  const raw = localStorage.getItem(storageKey)
  if (!raw)
    return null

  try {
    return JSON.parse(raw) as AuthResult
  }
  catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSessionState] = useState<AuthResult | null>(readSession)
  const value = useMemo<AuthContextValue>(() => ({
    session,
    setSession(next) {
      setSessionState(next)
      if (next)
        localStorage.setItem(storageKey, JSON.stringify(next))
      else
        localStorage.removeItem(storageKey)
    },
  }), [session])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const value = useContext(AuthContext)
  if (!value)
    throw new Error('AuthProvider is missing')

  return value
}
