import React from 'react'
import { createBrowserRouter, Navigate } from "react-router-dom";
import LoginPage from './pages/LoginPage'
import Municipes from './pages/Municipes'
import Grupos from './pages/Grupos'
import ProtectedRoute from './components/ProtectedRoute'
import NotFound from './pages/NotFound';

const router = createBrowserRouter([
    { path: '/', element: <LoginPage /> },
    { path: '/municipes', element: <ProtectedRoute element={<Municipes />} /> },
    { path: '/grupos', element: <ProtectedRoute element={<Grupos />} /> },
    { path: '*', element: <NotFound /> }
])

export default router