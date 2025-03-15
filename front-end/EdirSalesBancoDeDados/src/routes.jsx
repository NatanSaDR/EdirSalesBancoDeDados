import React from 'react'
import { createBrowserRouter, Navigate } from "react-router-dom";
import LoginPage from './pages/LoginPage'
import Municipes from './pages/Municipes'
import ProtectedRoute from './components/ProtectedRoute'
import NotFound from './pages/NotFound';

const router = createBrowserRouter([
    { path: '/', element: <LoginPage /> },
    { path: '/municipes', element: <ProtectedRoute element={<Municipes />} /> },
    { path: '*', element: <NotFound /> }
])

export default router