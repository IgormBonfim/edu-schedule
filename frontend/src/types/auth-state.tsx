export interface AuthState {
  isAuthenticated: boolean
  user: {
    name: string
    email: string
    role: string
  } | null
  token: string | null
}