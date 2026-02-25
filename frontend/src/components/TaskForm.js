import { useState, useEffect } from "react";
import API from "../api/api";
import { toast } from "react-toastify";

export default function TaskForm({ refreshTasks, editingTask, setEditingTask }) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [loading, setLoading] = useState(false);

  // Populate form when editing
  useEffect(() => {
    if (editingTask) {
      setTitle(editingTask.title);
      setDescription(editingTask.description);
    } else {
      setTitle("");
      setDescription("");
    }
  }, [editingTask]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!title) {
      toast.warning("Task title is required");
      return;
    }

    setLoading(true);
    try {
      if (editingTask) {
        // Update existing task
        await API.put(`/Tasks/${editingTask.id}`, {
          title,
          description,
          completed: editingTask.completed,
          assignedToUserId: editingTask.assignedToUserId || null
        });
        toast.success("Task updated successfully");
        setEditingTask(null); // Reset editing state
      } else {
        // Create new task
        await API.post("/Tasks", {
          title,
          description,
          completed: false,
          assignedToUserId: null
        });
        toast.success("Task created successfully");
      }

      setTitle("");
      setDescription("");
      refreshTasks();
    } catch (err) {
      console.error(err);
      toast.error("Failed to save task. Try again.");
    } finally {
      setLoading(false);
    }
  };

  const handleCancelEdit = () => {
    setEditingTask(null);
    setTitle("");
    setDescription("");
    toast.info("Edit cancelled");
  };

  return (
    <form onSubmit={handleSubmit} className="task-form">
      <input
        placeholder="Task title"
        value={title}
        onChange={e => setTitle(e.target.value)}
        required
        disabled={loading}
      />
      <input
        placeholder="Description"
        value={description}
        onChange={e => setDescription(e.target.value)}
        required
        disabled={loading}
      />
      <div className="task-form-buttons">
        <button type="submit" disabled={loading}>
          {loading ? "Saving..." : editingTask ? "Update Task" : "Add Task"}
        </button>
        {editingTask && (
          <button type="button" onClick={handleCancelEdit} disabled={loading}>
            Cancel
          </button>
        )}
      </div>
    </form>
  );
}