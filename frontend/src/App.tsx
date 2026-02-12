import './App.css'
import { useAuth } from './context/auth-context'
import { LoginPage } from './pages/login'

function App() {
    const { isAuthenticated } = useAuth()

  if (!isAuthenticated) {
    return <LoginPage />
  }
}

export default App
