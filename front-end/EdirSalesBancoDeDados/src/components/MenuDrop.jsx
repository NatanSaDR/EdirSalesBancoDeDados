import '../styles/MenuDrop.css'
import * as DropdownMenu from "@radix-ui/react-dropdown-menu";
import { ChevronDown, Menu, Calendar, Filter } from "lucide-react"; // Ícone para a seta
import { useState } from 'react';
import axios from 'axios';

const MenuDrop = ({ column, currentPage, pagina, tamanhoPagina }) => {
    const [dateStart, setDateStart] = useState("");
    const [dateEnd, setDateEnd] = useState("");



    const formatarData = (data) => {
        if (!data) return "";
        const [ano, mes, dia] = data.split("-"); // Divide "YYYY-MM-DD"
        return `${dia}/${mes}/${ano}`; // Retorna "dd/MM/yyyy"
    };

    const handleFilter = async (columnId, dateStart, dateEnd) => {
        try {
            const token = localStorage.getItem("token");

            // Criar objeto de parâmetros dinamicamente
            let params = {
                pagina,
                tamanhoPagina
            };

            // Mapear a coluna para os parâmetros corretos da API
            if (columnId === "aniversario") {
                params.aniversarioInicio = formatarData(dateStart);
                params.aniversarioFim = formatarData(dateEnd);
            } else if (columnId === "dataCadastro") {
                params.dataCadInicio = formatarData(dateStart);
                params.dataCadFim = formatarData(dateEnd);
            } else if (columnId === "dataAlteracao") {
                params.dataAltInicio = formatarData(dateStart);
                params.dataAltFim = formatarData(dateEnd);
            }

            const response = await axios.get(`http://localhost:5079/api/${currentPage}/filtrar`, {
                params,
                headers: { Authorization: `Bearer ${token}` },
            });

            console.log(response.data);
        } catch (error) {
            console.error("Erro ao buscar dados:", error);
        }
    };



    return (
        <div>
            <DropdownMenu.Root>
                <DropdownMenu.Trigger asChild>
                    <button className="dropdown-trigger">
                        <ChevronDown size={18} />
                    </button>
                </DropdownMenu.Trigger>


                <DropdownMenu.Content className="dropdown-menu">
                    <DropdownMenu.Item
                        onClick={() => column.column.toggleSorting(false)}
                        className="dropdown-item">
                        Ordenar A-Z
                    </DropdownMenu.Item>


                    <DropdownMenu.Item
                        onClick={() => column.column.toggleSorting(true)}
                        className="dropdown-item"
                    >
                        Ordenar Z-A
                    </DropdownMenu.Item>

                    {/* 🔹 Submenu de Filtros */}
                    <DropdownMenu.Sub>
                        <DropdownMenu.SubTrigger className="dropdown-item">
                            <Filter size={14} /> Filtrar
                        </DropdownMenu.SubTrigger>

                        <DropdownMenu.Portal>
                            <DropdownMenu.SubContent className="dropdown-submenu">
                                {column.column.id === "aniversario" || column.column.id === "dataCadastro" || column.column.id === "dataAlteracao" ? (
                                    <>
                                        <label className="filter-label">Data Início:</label>
                                        <input
                                            type="date"
                                            className="filter-input"
                                            value={dateStart}
                                            onChange={(e) => setDateStart(e.target.value)}
                                        />

                                        <label className="filter-label">Data Fim:</label>
                                        <input
                                            type="date"
                                            className="filter-input"
                                            value={dateEnd}
                                            onChange={(e) => setDateEnd(e.target.value)}
                                        />

                                        <DropdownMenu.Item
                                            onClick={() => handleFilter(column.column.id, dateStart, dateEnd)}
                                            className="dropdown-item"
                                        >
                                            Aplicar Filtro
                                        </DropdownMenu.Item>
                                    </>
                                ) : (
                                    <DropdownMenu.Item className="dropdown-item">
                                        Filtro padrão (Em breve)
                                    </DropdownMenu.Item>
                                )}
                            </DropdownMenu.SubContent>
                        </DropdownMenu.Portal>
                    </DropdownMenu.Sub>

                    <DropdownMenu.Item className="dropdown-item">
                        Selecionar Colunas (Em breve)
                    </DropdownMenu.Item>

                </DropdownMenu.Content>
            </DropdownMenu.Root>
        </div>
    )
}

export default MenuDrop