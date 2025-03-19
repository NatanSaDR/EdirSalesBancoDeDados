import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import '../styles/Loading.css';

import axios from "axios";

export default function ProtectedRoute({ element }) {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const token = localStorage.getItem("token");

    useEffect(() => {
        const fetchAuth = async () => {
            if (!token) {
                setIsAuthenticated(false);
                return;
            }

            try {
                const response = await axios.get("http://localhost:5079/api/user/validate-token", {
                    headers: { Authorization: `Bearer ${token}` },
                });

                setIsAuthenticated(response.data === true);
            } catch (error) {
                setIsAuthenticated(false);
            }
        };

        fetchAuth();
    }, [token]);

    if (isAuthenticated === null) {
        return <div className="loading"><p>Verificando o acesso...</p></div>; // 🔹 Mostra uma tela de carregamento enquanto verifica o token
    }

    return isAuthenticated ? element : <Navigate to="/" replace />;
}
