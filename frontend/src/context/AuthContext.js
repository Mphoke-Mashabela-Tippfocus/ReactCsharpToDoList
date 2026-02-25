import { createContext, useState } from "react";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);

const login = (token) => {
  const payload = JSON.parse(atob(token.split(".")[1]));

  localStorage.setItem("token", token); // ✅ store token

  setUser({ token, role: payload.role });
};

  const logout = () => {
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};