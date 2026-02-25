import axios from "axios";

// Create a plain Axios instance
const API = axios.create({
  baseURL: "http://localhost:5098/api", // Make sure this matches your backend URL
});

// Function to attach token dynamically
export const setAuthToken = (token) => {
  if (token) {
    API.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  } else {
    delete API.defaults.headers.common["Authorization"];
  }
};

export default API;