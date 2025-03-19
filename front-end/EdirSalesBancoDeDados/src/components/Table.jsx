import { useReactTable, getCoreRowModel, flexRender, getSortedRowModel } from "@tanstack/react-table";
import { useState } from 'react';
import MenuDrop from '../components/MenuDrop';

import { useGetData } from '../hooks/useGetData';
import '../styles/Loading.css';
import '../styles/Table.css';

const Table = ({ currentPage, returnObj, columns }) => {
    const url = `http://localhost:5079/api/${currentPage}/listartodos`;
    const [pagina, setPagina] = useState(1);
    const [tamanhoPagina, setTamanhoPagina] = useState(20);

    const { data: dados, loading } = useGetData({ url, pagina, tamanhoPagina });
    const data = dados?.[returnObj] || [];
    const totalRegistros = dados?.totalRegistros || 0;
    const totalPorPagina = Math.min(pagina * tamanhoPagina, totalRegistros);
    const ultimaPagina = Math.ceil(totalRegistros / tamanhoPagina);

    const handleSetTamanho = (e) => {
        setTamanhoPagina(e.target.value);
        setPagina(1);
    };

    const table = useReactTable({
        data,
        columns,
        getCoreRowModel: getCoreRowModel(),
        getSortedRowModel: getSortedRowModel(),
    });

    return (
        <div className="container">
            <h1>Lista de {currentPage}</h1>
            <div className="table-container">
                {loading && <div className='loading'><p>Carregando...</p></div>}

                <table className="data-table">
                    <thead>
                        {table.getHeaderGroups().map(headerGroup => (
                            <tr key={headerGroup.id}>
                                {headerGroup.headers.map(column => (
                                    <th key={column.id}>
                                        <div className="header-content">
                                            {flexRender(column.column.columnDef.header, column.getContext())}
                                            <MenuDrop column={column} currentPage={currentPage} pagina={pagina} tamanhoPagina={tamanhoPagina} />
                                        </div>
                                    </th>
                                ))}
                            </tr>
                        ))}
                    </thead>
                    <tbody>
                        {table.getRowModel().rows.map(row => (
                            <tr key={row.id}>
                                {row.getVisibleCells().map(cell => (
                                    <td key={cell.id}>
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="footer">
                <div className="botoes-acoes">
                    <button onClick={() => setPagina(1)}>🡄</button>
                    <button onClick={() => setPagina((prev) => Math.max(prev - 1, 1))} disabled={pagina === 1}>🠸</button>
                    <span>Página: <input className='input-pagina' onChange={(e) => setPagina(e.target.value)} type="text" value={pagina} /></span>
                    <span>de</span>
                    <span><input className='input-pagina' onChange={(e) => setPagina(e.target.value)} type="text" value={ultimaPagina} disabled /></span>
                    <button onClick={() => setPagina((prev) => Math.min(prev + 1, ultimaPagina))} disabled={pagina >= ultimaPagina}>🠺</button>
                    <button onClick={() => setPagina(ultimaPagina)}>🡆</button>
                    <label>Exibir:</label>
                    <select onChange={handleSetTamanho} value={tamanhoPagina}>
                        <option value={10}>10</option>
                        <option value={20}>20</option>
                        <option value={50}>50</option>
                        <option value={100}>100</option>
                    </select>
                </div>
                <div className="total-registros">
                    <span>
                        Dados: {totalPorPagina} de {totalRegistros}
                    </span>
                </div>
            </div>
        </div>
    );
};

export default Table;
