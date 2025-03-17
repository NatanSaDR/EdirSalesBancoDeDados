import { useState } from 'react'
import '../styles/TableButtons.css'
const TableButtons = ({totalRegistros}) => {
    const [pagina, setPagina] = useState(1);
    const [tamanhoPagina, setTamanhoPagina] = useState(20);


    return (
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
    )
}

export default TableButtons