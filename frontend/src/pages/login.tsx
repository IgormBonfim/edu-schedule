import { AlertCircle, Eye, EyeOff, GraduationCap, Lock, Mail } from "lucide-react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "../components/ui/card";
import { useState } from "react";
import { Input } from "../components/ui/input";
import { Button } from "../components/ui/button";
import { useAuth } from "../context/auth-context";
import { useNavigate } from "react-router-dom";

export function LoginPage() {
  const { login, isLoading, error } = useAuth()
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [showPassword, setShowPassword] = useState(false)
  const [localError, setLocalError] = useState<string | null>(null)
  const navigate = useNavigate();

  const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault()
    setLocalError(null)

    if (!email || !password) {
      setLocalError("Preencha todos os campos")
      return
    }

    try {
      await login(email, password)
      navigate("/dashboard")
    } catch {

    }
  }

  const displayError = localError || error

  return (
    <div className="flex min-h-screen items-center justify-center bg-background px-4">
      <div className="w-full max-w-md">
        <div className="mb-8 flex flex-col items-center gap-3">
          <div className="flex h-14 w-14 items-center justify-center rounded-xl bg-primary text-white">
            <GraduationCap className="h-8 w-8" />
          </div>
          <div className="text-center">
            <h1 className="text-2xl font-bold tracking-tight text-slate-900">
              EduSchedule
            </h1>
            <p className="text-sm text-slate-500">
              Gerenciamento de Agendas Educacionais
            </p>
          </div>
        </div>

        <Card className="border-slate-200/60 bg-white shadow-lg">
          <CardHeader className="pb-4">
            <CardTitle className="text-lg font-semibold text-slate-900">Entrar na plataforma</CardTitle>
            <CardDescription className="text-sm text-slate-500">
              Insira suas credenciais para acessar o painel
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              {displayError && (
                <div className="flex items-center gap-2 rounded-lg border border-red-200 bg-red-50 px-3 py-2.5 text-sm text-red-600">
                  <AlertCircle className="h-4 w-4 shrink-0" />
                  {displayError}
                </div>
              )}
              <div className="flex flex-col gap-2">
                <label htmlFor="email" className="text-sm font-medium text-slate-700">E-mail</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
                  <Input
                    id="email"
                    type="email"
                    placeholder="seu.email@instituicao.edu.br"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="pl-9"
                    autoComplete="email"
                    required
                  />
                </div>
              </div>

              <div className="flex flex-col gap-2">
                <label htmlFor="password">Senha</label>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    placeholder="Sua senha"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="pl-9 pr-10"
                    autoComplete="current-password"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 cursor-pointer text-slate-400 hover:text-slate-700"
                    aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}
                  >
                    {showPassword ? (
                      <EyeOff className="h-4 w-4" />
                    ) : (
                      <Eye className="h-4 w-4" />
                    )}
                  </button>
                </div>
              </div>

              <Button
                type="submit"
                className="mt-2 w-full"
                disabled={isLoading}
              >
                {isLoading ? "Entrando..." : "Entrar"}
              </Button>

              <p className="text-center text-xs text-slate-400">
                Use qualquer e-mail e senha (min. 6 caracteres) para acessar a demo
              </p>
            </form>
          </CardContent>
        </Card>

        <p className="mt-6 text-center text-xs text-slate-400">
          Integrado com Microsoft Graph API
        </p>
      </div>
    </div>
  )
}