import { useState, useContext } from "react";
import { useNavigate, Link } from "react-router-dom";
import { toast } from "react-toastify";
import API, { setAuthToken } from "../api/api";
import { AuthContext } from "../context/AuthContext";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const { login } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!username || !password) {
      toast.warning("Please enter username and password");
      return;
    }

    try {
      setLoading(true);

      const res = await API.post("/Auth/login", { username, password });

      const token = res.data.token;

      //  Save token in AuthContext
      login(token);

      //  Set token in Axios for future requests
      setAuthToken(token);

      toast.success("Login successful");

      setTimeout(() => {
        navigate("/dashboard");
      }, 1000);
    } catch (err) {
      console.log("Login error:", err.response || err);

      if (err.response?.status === 401) {
        toast.error("Invalid username or password");
      } else if (err.response?.status === 404) {
        toast.error("Login endpoint not found");
      } else {
        toast.error("Something went wrong. Try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <h2>Login</h2>

      <form onSubmit={handleSubmit}>
        <input
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          disabled={loading}
          required
        />

        <input
          placeholder="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={loading}
          required
        />

        <button type="submit" disabled={loading}>
          {loading ? "Logging in..." : "Login"}
        </button>
      </form>

      <p>
        Don't have an account? <Link to="/register">Register here</Link>
      </p>
    </div>
  );
}