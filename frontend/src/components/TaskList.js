import API from "../api/api";
import { useState } from "react";

export default function TaskList({ tasks, refreshTasks, userRole }) {
  const [editingTaskId, setEditingTaskId] = useState(null); // remove <number | null>
  const [editTitle, setEditTitle] = useState("");
  const [editDescription, setEditDescription] = useState("");

  const startEdit = task => {
    setEditingTaskId(task.id);
    setEditTitle(task.title);
    setEditDescription(task.description);
  };

  const saveEdit = async id => {
    try {
      await API.put(`/Tasks/${id}`, { title: editTitle, description: editDescription, completed: false, assignedToUserId: null });
      setEditingTaskId(null);
      refreshTasks();
    } catch (err) {
      console.error(err);
    }
  };

  const deleteTask = async id => {
    try {
      await API.delete(`/Tasks/${id}`);
      refreshTasks();
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <div className="task-list">
      {tasks.map(task => (
        <div key={task.id} className="task-item">
          {editingTaskId === task.id ? (
            <>
              <input value={editTitle} onChange={e => setEditTitle(e.target.value)} />
              <input value={editDescription} onChange={e => setEditDescription(e.target.value)} />
              <button onClick={() => saveEdit(task.id)}>Save</button>
            </>
          ) : (
            <>
              <h3>{task.title}</h3>
              <p>{task.description}</p>
              <p>Created by: {task.createdByUser.username}</p>
              <button onClick={() => startEdit(task)}>Edit</button>
              {userRole === "Admin" && <button onClick={() => deleteTask(task.id)}>Delete</button>}
            </>
          )}
        </div>
      ))}
    </div>
  );
}