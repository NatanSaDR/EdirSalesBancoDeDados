import { useState } from 'react'
import axios from 'axios'
import '../styles/LoginPage.css'
import { useNavigate } from "react-router-dom"
  ;
const LoginPage = () => {
  const [username, setUser] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate();
  const handleLogin = async (e) => {
    e.preventDefault()

    try {
      const response = await axios.post(
        "http://localhost:5079/api/user/login",
        { username, password },
        { headers: { "Content-Type": "application/json" } }
      );
      localStorage.setItem("token", response.data.token);
      navigate("/municipes");
    } catch (error) {
      console.log("Erro ao autenticar: " + (error.response?.data.message || "Erro desconhecido"))
    }
  }
  return (
    <div className='main-login-page'>
      <h1>Login</h1>
      <form className='login-form'>
        <input onChange={(e) => setUser(e.target.value)} type="text" placeholder="Usuário" value={username} required />
        <input onChange={(e) => setPassword(e.target.value)} type="password" placeholder="Senha" value={password} required />
        <button type="submit" onClick={handleLogin}>Entrar</button>
      </form>
    </div>
  )
}

export default LoginPage