import { Link, Outlet } from "react-router-dom";
import "../styles/Layout.css"; // Opcional para estilizar

const Layout = () => {
    return (
        <div className="layout-container">
            {/* 🔹 Barra de navegação */}
            <nav className="navbar">
                <ul>
                    <li><Link to="/municipes">Munícipes</Link></li>
                    <li><Link to="/grupos">Grupos</Link></li>
                    <li><Link to="/">Sair</Link></li>
                </ul>
            </nav>

            {/* 🔹 Área dinâmica para cada página */}
            <main className="content">
                <Outlet /> {/* ⬅️ Aqui as páginas serão renderizadas */}
            </main>
        </div>
    );
};

export default Layout;
