import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useContext } from "react";
import { AuthProvider, AuthContext } from "./context/AuthContext";
import Login from "./components/Login";
import Register from "./components/Register";
import Dashboard from "./components/Dashboard";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <AuthProvider>
      <Router>

        {/* ✅ Toast Container MUST be rendered */}
        <ToastContainer
          position="top-right"
          autoClose={3000}
          theme="dark"
          toastStyle={{
            background: "#0d1b2a",
            color: "#87ceeb",
            border: "1px solid #87ceeb"
          }}
        />

        <Routes>
          <Route path="/" element={<LoginWrapper />} />
          <Route path="/register" element={<Register />} />
          <Route path="/dashboard" element={<DashboardWrapper />} />
        </Routes>

      </Router>
    </AuthProvider>
  );
}

const LoginWrapper = () => {
  const { user } = useContext(AuthContext);
  return user ? <Navigate to="/dashboard" /> : <Login />;
};

const DashboardWrapper = () => {
  const { user } = useContext(AuthContext);
  return user ? <Dashboard /> : <Navigate to="/" />;
};

export default App;