import { useEffect, useState, useMemo } from 'react';
import { useReactTable, getCoreRowModel, flexRender, createColumnHelper, getSortedRowModel } from "@tanstack/react-table";
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import '../styles/Municipes.css';

const Municipes = () => {
    const [municipes, setMunicipes] = useState([]);
    const [loading, setLoading] = useState(false);
    const [sorting, setSorting] = useState([]);
    const navigate = useNavigate();
    const [pagina, setPagina] = useState(1);
    const [tamanhoPagina, setTamanhoPagina] = useState(20);
    const [totalRegistros, setTotalRegistros] = useState(0);
    const [filters, setFilters] = useState({
        id: "",
        nome: "",
        sexo: "",
        aniversario: "",
        logradouro: "",
    });

    // 🔹 Busca inicial dos munícipes
    const fetchMunicipes = async () => {
        setLoading(true);
        const token = localStorage.getItem("token");
        if (!token) {
            navigate('/');
            return;
        }

        try {
            const response = await axios.get('http://localhost:5079/api/Municipe/', {
                params: { pagina, tamanhoPagina },
                headers: { Authorization: `Bearer ${token}` },
            });

            setMunicipes(response.data.municipes || []);
            setTotalRegistros(response.data.totalRegistros || 0);
        } catch (error) {
            console.log("Erro ao buscar municipes: " + (error.response?.data.message || "Erro desconhecido"));
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchMunicipes();
    }, [pagina, tamanhoPagina]);

    // 🔹 Filtragem de munícipes ao digitar nos inputs (com debounce)
    useEffect(() => {
        const fetchFilteredMunicipes = async () => {
            setLoading(true);
            const token = localStorage.getItem("token");
            if (!token) {
                navigate('/');
                return;
            }

            try {
                // 🔹 Se o ID for preenchido, usa GetById e ignora os outros filtros
                if (filters.id) {
                    const response = await axios.get(`http://localhost:5079/api/Municipe/${filters.id}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    setMunicipes(response.data ? [response.data] : []);
                    setTotalRegistros(response.data ? 1 : 0);
                    return;
                }

                // 🔹 Se outros filtros forem preenchidos, chama o endpoint de Filtrar
                const params = {};
                Object.keys(filters).forEach((key) => {
                    if (filters[key]) {
                        params[key] = filters[key];
                    }
                });

                if (Object.values(params).length === 0) {
                    fetchMunicipes();
                    return;
                }

                const response = await axios.get('http://localhost:5079/api/Municipe/Filtrar', {
                    params: { ...params, pagina, tamanhoPagina },
                    headers: { Authorization: `Bearer ${token}` },
                });

                setMunicipes(response.data.municipes || []);
                setTotalRegistros(response.data.totalRegistros || 0);
            } catch (error) {
                console.log("Erro ao buscar municipes: " + (error.response?.data.message || "Erro desconhecido"));
            } finally {
                setLoading(false);
            }
        };

        const timer = setTimeout(() => {
            fetchFilteredMunicipes();
        }, 500);

        return () => clearTimeout(timer);
    }, [filters, pagina, tamanhoPagina]);

    const deleteMunicipe = async (id) => {
        if (!window.confirm("Tem certeza que deseja excluir este munícipe?")) return;

        try {
            await axios.delete(`http://localhost:5079/api/Municipe/${id}`, {
                headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
            });

            setMunicipes((prev) => prev.filter((municipe) => municipe.id !== id));
            alert("Munícipe excluído com sucesso!");
        } catch (error) {
            alert("Erro ao excluir munícipe.");
        }
    };

    const columnHelper = createColumnHelper();
    const columns = useMemo(() => [
        columnHelper.accessor("id", { header: "ID" }),
        columnHelper.accessor("nome", { header: "Nome" }),
        columnHelper.accessor("sexo", { header: "Sexo" }),
        columnHelper.accessor("aniversario", { header: "Aniversário" }),
        columnHelper.accessor("logradouro", { header: "Endereço" }),
        columnHelper.display({
            id: "actions",
            header: "Ações",
            cell: ({ row }) => (
                <div className="actions">
                    <button onClick={() => navigate(`/editar/${row.original.id}`)} className="btn-update">🔄 Atualizar</button>
                    <button onClick={() => deleteMunicipe(row.original.id)} className="btn-delete">🗑️ Excluir</button>
                </div>
            ),
        }),
    ], [sorting]);

    const table = useReactTable({
        data: municipes,
        columns,
        getCoreRowModel: getCoreRowModel(),
        getSortedRowModel: getSortedRowModel(),
        state: {
            sorting,
        },
        onSortingChange: setSorting,
    });

    return (
        <div className="container">
            <h1>Lista de Munícipes</h1>

            {loading && <p>Carregando...</p>}

            <div className="table-container">
                <table className="data-table">
                    <thead>
                        <tr>
                            {table.getHeaderGroups()?.map(headerGroup =>
                                headerGroup.headers.map(column => (
                                    <th key={column.id}>
                                        {flexRender(column.column.columnDef.header, column.getContext())}
                                    </th>
                                ))
                            )}
                        </tr>
                        <tr>
                            {table.getHeaderGroups()?.map(headerGroup =>
                                headerGroup.headers.map(column => {
                                    const accessor = column.column.columnDef.accessorKey;
                                    return (
                                        <th key={column.id}>
                                            <input
                                                type="text"
                                                value={filters[accessor] || ""}
                                                onChange={(e) =>
                                                    setFilters((prev) => ({
                                                        ...prev,
                                                        [accessor]: e.target.value,
                                                    }))
                                                }
                                                placeholder={`Filtrar ${accessor}`}
                                                className="filter-input"
                                            />
                                        </th>
                                    );
                                })
                            )}
                        </tr>
                    </thead>
                    <tbody>
                        {table.getRowModel().rows.map((row) => (
                            <tr key={row.id}>
                                {row.getVisibleCells().map((cell) => (
                                    <td key={cell.id}>
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="botoes-acoes">
                <button onClick={() => setPagina(pagina - 1)} disabled={pagina === 1}>⮜ Anterior</button>
                <span>Página: {pagina}</span>
                <button onClick={() => setPagina(pagina + 1)}>Próxima  ⮞</button>

                <label>Tamanho da Página: </label>
                <select onChange={(e) => setTamanhoPagina(Number(e.target.value))} value={tamanhoPagina}>
                    <option value={10}>10</option>
                    <option value={20}>20</option>
                    <option value={50}>50</option>
                    <option value={100}>100</option>
                </select>
                <span>Total de registros:</span>
                <input className='total-registro' type="text" value={totalRegistros} disabled />
            </div>
        </div>
    );
};

export default Municipes;
