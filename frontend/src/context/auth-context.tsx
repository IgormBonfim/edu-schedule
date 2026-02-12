import { createContext, useContext, useState, useCallback, useEffect, type ReactNode } from "react"
import type { AuthState } from "../types/auth-state"
import { api } from "../api/axios-client"

interface AuthContextType extends AuthState {
  login: (email: string, password: string) => Promise<void>
  logout: () => void
  isLoading: boolean
  error: string | null
}

const getInitialState = (): AuthState => {
  if (typeof window === "undefined") {
    return { isAuthenticated: false, user: null, token: null };
  }

  const stored = sessionStorage.getItem("eduschedule_auth");
  if (stored) {
    try {
      return JSON.parse(stored) as AuthState;
    } catch {
      sessionStorage.removeItem("eduschedule_auth");
    }
  }
  return { isAuthenticated: false, user: null, token: null };
};

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuth] = useState<AuthState>(getInitialState())
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const stored = sessionStorage.getItem("eduschedule_auth")
    if (stored) {
      try {
        const parsed = JSON.parse(stored) as AuthState
        setAuth(parsed)
      } catch {
        sessionStorage.removeItem("eduschedule_auth")
      }
    }
  }, [])

  const login = useCallback(async (email: string, password: string) => {
    setIsLoading(true)
    setError(null)

    try {
      const { data } = await api.post('/auth/login', { email, password });
      
      const newAuth: AuthState = {
        isAuthenticated: true,
        user: data.user,
        token: data.token,
      }
      setAuth(newAuth)
      sessionStorage.setItem("eduschedule_auth", JSON.stringify(newAuth))
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || err.message || "Erro ao fazer login";
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setIsLoading(false)
    }
  }, [])

  const logout = useCallback(() => {
    setAuth({ isAuthenticated: false, user: null, token: null })
    sessionStorage.removeItem("eduschedule_auth")
  }, [])

  return (
    <AuthContext.Provider
      value={{ ...auth, login, logout, isLoading, error }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
