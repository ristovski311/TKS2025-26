import { API_ENDPOINTS } from '../config/config.js';

export async function getProfessorById(professorId) {
    try {
        const response = await fetch(`${API_ENDPOINTS.PROFESSORS}/${professorId}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        })
        if (!response.ok) {
            throw new Error("Failed to fetch professor");
        }

        return await response.json();
    }
    catch (err) {
        console.error("Cannot get professor:", err);
        throw err;
    }
}

export async function getProfessorsByUserId(userId) {
    try {
        const response = await fetch(API_ENDPOINTS.PROFESSORS_BY_USER(userId), {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to fetch professors for user");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot get professors by user:", err);
        throw err;
    }
}