import { API_ENDPOINTS } from '../config/config.js';

export async function getTasks() {
    try {
        const response = await fetch(API_ENDPOINTS.TASKS, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to fetch tasks");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot get tasks:", err);
        throw err;
    }
}

export async function createTask(taskData) {
    try {
        const response = await fetch(API_ENDPOINTS.TASKS, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(taskData)
        });

        if (!response.ok) {
            throw new Error("Failed to create task");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot create task:", err);
        throw err;
    }
}

export async function updateTask(taskId, taskData) {
    try {
        const response = await fetch(`${API_ENDPOINTS.TASKS}/${taskId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(taskData)
        });

        if (!response.ok) {
            throw new Error("Failed to update task");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot update task:", err);
        throw err;
    }
}