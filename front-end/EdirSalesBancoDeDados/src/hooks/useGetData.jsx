import { useState, useEffect } from 'react'
import axios from 'axios'
import { useNavigate } from "react-router-dom"

export const useGetData = ({ url, pagina, tamanhoPagina }) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();


  const token = localStorage.getItem("token");

  const auth = async () => {
    if (!token) {
      navigate("/");
      return;
    }
    const response = await axios.get(
      "http://localhost:5079/api/users/validate-token", {
      headers: { Authorization: `Bearer ${token}` }
    }
    )
    if (!response.data === true) {
      return navigate("/");
    }
  }

  useEffect(() => {
    const fetchData = async () => {
      auth()
      setLoading(true);
      
      try {
        const response = await axios.get(url, {
          params: { pagina, tamanhoPagina },
          headers: { Authorization: `Bearer ${token}` },
        });

        if (!response.data) {
          console.log("Erro ao buscar dados: " + (response.data?.message || "Erro desconhecido"));
          return;
        }

        setData(response.data);
      } catch (error) {
        console.log("Erro ao buscar dados: " + (error.response?.data?.message || "Erro desconhecido"));
      } finally {
        setLoading(false);
      }
    };
    
    fetchData();
  }, [url, pagina, tamanhoPagina]);

  return { data, loading };
};
