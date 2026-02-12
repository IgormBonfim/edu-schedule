import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import { useAuth } from './context/auth-context'
import { DashboardPage } from './pages/dashboard'
import { LoginPage } from './pages/login'

function App() {
    const { isAuthenticated } = useAuth()

    return (
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />

          <Route 
            path="/dashboard" 
            element={isAuthenticated ? <DashboardPage /> : <Navigate to="/login" />} 
          />

          <Route path="*" element={<Navigate to="/dashboard" />} />
        </Routes>
    </BrowserRouter>
    )
}

export default App
