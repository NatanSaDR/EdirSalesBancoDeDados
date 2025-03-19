import { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from "react-router-dom";

export const useGetData = ({ url, pagina, tamanhoPagina }) => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const token = localStorage.getItem("token");

    useEffect(() => {
        if (!token) {
            navigate("/");
            return;
        }

        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await axios.get(url, {
                    params: { pagina, tamanhoPagina },
                    headers: { Authorization: `Bearer ${token}` },
                });

                setData(response.data || []);
            } catch (error) {
                console.error("Erro ao buscar dados:", error.response?.data?.message || "Erro desconhecido");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [url, pagina, tamanhoPagina]);

    return { data, loading };
};
