import { useMemo } from 'react';
import { createColumnHelper } from "@tanstack/react-table";
import Table from '../components/Table';

const columnHelper = createColumnHelper();

const Municipes = () => {
    const columns = useMemo(() => [
        columnHelper.accessor("id", { header: "ID" }),
        columnHelper.accessor("nome", { header: "Nome" }),
        columnHelper.accessor("sexo", { header: "Sexo" }),
        columnHelper.accessor("aniversario", { header: "Aniversário" }),
        columnHelper.accessor("logradouro", { header: "Endereço" }),
        columnHelper.accessor("numero", { header: "Número" }),
        columnHelper.accessor("complemento", { header: "Complemento" }),
        columnHelper.accessor("bairro", { header: "Bairro" }),
        columnHelper.accessor("cidade", { header: "Cidade" }),
        columnHelper.accessor("estado", { header: "Estado" }),
        columnHelper.accessor("cep", { header: "CEP" }),
        columnHelper.accessor("observacao", { header: "Observação" }),
        columnHelper.accessor("email", { header: "E-mail" }),
        columnHelper.accessor("grupos", { header: "Grupos" }),
        columnHelper.accessor("dataCadastro", { header: "Data Cadastro" }),
        columnHelper.accessor("usuarioCadastro", { header: "Usuário Cadastro" }),
        columnHelper.accessor("dataAlteracao", { header: "Data Alteração" }),
        columnHelper.accessor("usuarioAlteracao", { header: "Usuário Alteração" }),
    ], []);

    return <Table currentPage="municipe" returnObj="municipes" columns={columns} />;
};

export default Municipes;
