import React from 'react'
import './LoginPage.css'
const LoginPage = () => {
  return (
    <div className='main-login-page'>
      <h1>Login</h1>
      <form className='login-form'>
        <input type="text" placeholder="Usuário" />
        <input type="password" placeholder="Senha" />
        <button type="submit">Entrar</button>
      </form>
    </div>
  )
}

export default LoginPage