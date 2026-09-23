import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './auth'
import { Layout } from './Layout'
import { LoginPage } from './pages/LoginPage'
import { QuestionFormPage } from './pages/QuestionFormPage'
import { QuestionListPage } from './pages/QuestionListPage'
import { QuestionPage } from './pages/QuestionPage'
import { RegisterPage } from './pages/RegisterPage'

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route element={<Layout />}>
            <Route index element={<QuestionListPage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="register" element={<RegisterPage />} />
            <Route path="questions/new" element={<QuestionFormPage />} />
            <Route path="questions/:id" element={<QuestionPage />} />
            <Route path="questions/:id/edit" element={<QuestionFormPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
