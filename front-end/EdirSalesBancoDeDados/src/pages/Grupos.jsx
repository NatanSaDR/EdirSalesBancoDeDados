import Table from '../components/Table'
import { createColumnHelper } from "@tanstack/react-table";
import { useMemo } from 'react';

const columnHelper = createColumnHelper();


const Grupos = () => {

    const columns = [
        columnHelper.accessor("id", { header: "ID" }),
        columnHelper.accessor("nomeGrupo", { header: "Grupo" }),
        columnHelper.accessor("dataCadastro", { header: "Data Cadastro" }),
        columnHelper.accessor("usuarioCadastro", { header: "Usuário Cadastro" }),
        columnHelper.accessor("dataAlteracao", { header: "Data Alteração" }),
        columnHelper.accessor("usuarioAlteracao", { header: "Usuário Alteração" }),
    ];

    return (
        <Table currentPage='Grupo' returnObj="grupos" columns={columns} />
    );
}

export default Grupos