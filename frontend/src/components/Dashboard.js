import { useContext, useEffect, useState } from "react";
import { FaUserCircle, FaEdit, FaTrash } from "react-icons/fa"; 
import { toast } from "react-toastify";
import API from "../api/api";
import { AuthContext } from "../context/AuthContext";
import TaskForm from "./TaskForm";
import "./Dashboard.css";

export default function Dashboard() {
  const { user, logout } = useContext(AuthContext);
  const [tasks, setTasks] = useState([]);
  const [showProfileMenu, setShowProfileMenu] = useState(false);
  const [editingTask, setEditingTask] = useState(null); // For editing

  const fetchTasks = async () => {
    try {
      const res = await API.get("/Tasks");
      setTasks(res.data);
    } catch (err) {
      console.error(err);
      toast.error("Failed to fetch tasks. Try again.");
    }
  };

  useEffect(() => { fetchTasks(); }, []);

  const deleteTask = async (id) => {
    if (!window.confirm("Are you sure you want to delete this task?")) return;
    try {
      await API.delete(`/Tasks/${id}`);
      toast.success("Task deleted successfully");
      fetchTasks();
    } catch (err) {
      console.error(err);
      toast.error("Failed to delete task");
    }
  };

  const editTask = (task) => {
    setEditingTask(task); // Pass the task to the form
    toast.info(`Editing task: ${task.title}`);
  };

  return (
    <div className="dashboard">
      {/* ===== HEADER ===== */}
      <header className="dashboard-header">
        <h1>Task Dashboard</h1>

        <div className="profile-menu-wrapper">
          <FaUserCircle 
            size={36} 
            onClick={() => setShowProfileMenu(!showProfileMenu)} 
            className="profile-icon"
          />
          {showProfileMenu && (
            <div className="profile-dropdown">
              <p className="dropdown-item">Hello, {user?.username}</p>
              <hr />
              <button className="dropdown-item" onClick={logout}>Logout</button>
            </div>
          )}
        </div>
      </header>

      {/* ===== TASK FORM ===== */}
      <TaskForm 
        refreshTasks={fetchTasks} 
        editingTask={editingTask} 
        setEditingTask={setEditingTask} 
      />

      {/* ===== TASK LIST ===== */}
      <div className="tasks-container">
        {tasks.length === 0 ? (
          <p className="no-tasks">No tasks yet. Add one above!</p>
        ) : (
          tasks.map(task => (
            <div className="task-card" key={task.id}>
              <div className="task-card-header">
                <h3>{task.title}</h3>
                <div className="task-actions">
                  <FaEdit onClick={() => editTask(task)} />
                  <FaTrash onClick={() => deleteTask(task.id)} />
                </div>
              </div>
              <p>{task.description}</p>
              <div className="task-footer">
                <span>Status: {task.completed ? "✅ Done" : "⏳ Pending"}</span>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}