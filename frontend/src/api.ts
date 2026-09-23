export class ApiError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

const base = import.meta.env.VITE_API_URL ?? ''

function currentLanguage() {
  const raw = localStorage.getItem('sprosi-locale')
  if (!raw)
    return 'ru'

  try {
    const parsed = JSON.parse(raw) as { language?: string }
    return parsed.language === 'en' ? 'en' : 'ru'
  }
  catch {
    return 'ru'
  }
}

export async function api<T>(path: string, options: RequestInit = {}, token?: string | null): Promise<T> {
  const headers = new Headers(options.headers)
  headers.set('Accept-Language', currentLanguage())
  if (options.body)
    headers.set('Content-Type', 'application/json')
  if (token)
    headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${base}${path}`, { ...options, headers })
  if (response.status === 204)
    return undefined as T

  const text = await response.text()
  const data = text ? JSON.parse(text) as { message?: string } : null
  if (!response.ok) {
    const fallback = currentLanguage() === 'en' ? 'The request failed.' : 'Не получилось выполнить запрос.'
    throw new ApiError(data?.message ?? fallback)
  }

  return data as T
}

export type AuthResult = {
  userId: string
  displayName: string
  token: string
}
