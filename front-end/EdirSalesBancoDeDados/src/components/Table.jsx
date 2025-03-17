import { useReactTable, getCoreRowModel, flexRender, createColumnHelper, getSortedRowModel } from "@tanstack/react-table";
import { useMemo, useState } from 'react';
import { useGetData } from '../hooks/useGetData';
import '../styles/Table.css';

const Table = ({ currentPage, returnObj }) => {

    const url = `http://localhost:5079/api/${currentPage}/listartodos`;
    const [pagina, setPagina] = useState(1);
    const [tamanhoPagina, setTamanhoPagina] = useState(20);
    const [filters, setFilters] = useState({}); 


    const { data: dados, loading } = useGetData({ url, pagina, tamanhoPagina });
    const data = dados?.[returnObj] || [];
    const totalRegistros = dados?.totalRegistros || 0;

    // 🔹 Pega todas as chaves dos objetos retornados
    const tableKeys = data.length > 0 ? Object.keys(data[0]) : [];
    const columnHelper = createColumnHelper();

    // 🔹 Cria colunas dinamicamente
    const columns = useMemo(() =>
        tableKeys.map((key) =>
            columnHelper.accessor(key, {
                header: () => (
                    <div>
                        <span>{key.charAt(0).toUpperCase() + key.slice(1)}</span>
                        <input
                            type="text"
                            value={filters[key] || ""}
                            onChange={(e) => setFilters({ ...filters, [key]: e.target.value })}
                            placeholder="Filtrar..."
                            className="filter-input"
                        />
                    </div>
                )
            })
        ),
        [tableKeys, filters]
    );

    const table = useReactTable({
        data: data,
        columns,
        getCoreRowModel: getCoreRowModel(),
    });

    return (
        <div className="container">
            <h1>Lista de {currentPage}</h1>
            <div className="table-container">

                {loading && <p className='loading'>Carregando...</p>}

                <table className="data-table">

                    <thead>
                        <tr>
                            {table.getHeaderGroups().map(headerGroup =>
                                headerGroup.headers.map(column => (
                                    <th key={column.id}>
                                        {flexRender(column.column.columnDef.header, column.getContext())}
                                    </th>
                                ))
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
                <button onClick={() => setPagina((prev) => Math.max(prev - 1, 1))} disabled={pagina === 1}>
                    ⮜ Anterior
                </button>
                <span>Página: <input className='input-pagina' onChange={(e) => setPagina(e.target.value)} type="text" value={pagina} /></span>
                <button onClick={() => setPagina(pagina + 1)}>Próxima ⮞</button>

                <label>Tamanho da Página:</label>
                <select onChange={(e) => setTamanhoPagina(Number(e.target.value))} value={tamanhoPagina}>
                    <option value={10}>10</option>
                    <option value={20}>20</option>
                    <option value={50}>50</option>
                    <option value={100}>100</option>
                </select>
                <span>Total de registros: {totalRegistros}</span>
            </div>
        </div>
    )
}

export default Table