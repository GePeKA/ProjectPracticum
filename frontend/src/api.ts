export class ApiError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

const base = import.meta.env.VITE_API_URL ?? ''

export async function api<T>(path: string, options: RequestInit = {}, token?: string | null): Promise<T> {
  const headers = new Headers(options.headers)
  if (options.body)
    headers.set('Content-Type', 'application/json')
  if (token)
    headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${base}${path}`, { ...options, headers })
  if (response.status === 204)
    return undefined as T

  const text = await response.text()
  const data = text ? JSON.parse(text) as { message?: string } : null
  if (!response.ok)
    throw new ApiError(data?.message ?? 'Не получилось выполнить запрос.')

  return data as T
}

export type AuthResult = {
  userId: string
  displayName: string
  token: string
}
