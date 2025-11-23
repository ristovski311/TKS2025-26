import {
    createTask,
    getTasks,
    getTaskById,
    updateTask,
    deleteTask,
    searchTasks
} from "../services/taskService.js"

export const addTask = async (req, res) => {
    try {
        const taskData = req.body;
        const newTask = await createTask(taskData);
        res.status(201).json(newTask);
    } catch (error) {
        res.status(400).json({ error: error.message });
    }
};

export const fetchTasks = async (req, res) => {
    try {
        const tasks = await getTasks();
        res.status(200).json(tasks);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

export const fetchTaskById = async (req, res) => {
    try {
        const { id } = req.params;
        const task = await getTaskById(id);
        if (!task) {
            return res.status(404).json({ error: "Task not found!" });
        }
        res.status(200).json(task);
    } catch (error) {
        res.status(500).json({ error: error.message });
    };
};

export const modifyTask = async (req, res) => {
    try {
        const { id } = req.params;
        const updates = res.body;
        const updatedTask = await updateTask(id, updates);
        if (!updatedTask || updatedTask.length === 0) {
            return res.status(404).json({ error: "Task not found!" });
        }
        res.status(200).json(updatedTask);
    } catch (error) {
        res.status(400).json({ error: error.message });
    }
};

export const removeTask = async (req, res) => {
    try {
        const { id } = req.params;
        const result = deleteTask(id);
        res.status(200).json(result);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};

export const findTasks = async (req, res) => {
    try {
        const { query } = req.query;
        if (!query) {
            return res.status(400).json({ error: "Search query is required!" });
        }
        const tasks = searchTasks(query);
        res.status(200).json(tasks);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
};