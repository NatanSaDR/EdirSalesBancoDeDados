import { createBrowserRouter } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import Municipes from "./pages/Municipes";
import Grupos from "./pages/Grupos";
import NotFound from "./pages/NotFound";
import ProtectedRoute from "./components/ProtectedRoute";
import Layout from "./components/Layout";

const router = createBrowserRouter([
    { path: "/", element: <LoginPage /> },

    {
        path: "/",
        element: <ProtectedRoute element={<Layout />} />, // 🔹 Layout protegido
        children: [
            { path: "/municipes", element: <Municipes /> },
            { path: "/grupos", element: <Grupos /> },
        ],
    },

    { path: "*", element: <NotFound /> },
]);

export default router;
