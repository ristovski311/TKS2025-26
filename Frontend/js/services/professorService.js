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

export async function deleteProfessor(profId)
{
    try
    {
        const response = await fetch(`${API_ENDPOINTS.PROFESSORS}/${profId}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
        });

        if (!response.ok) {
            throw new Error("Failed to remove the professor with id: " + profId);
        }
        else
        {
            console.log("Removed the professor with the id:" + profId);
        }
    }
    catch(err)
    {
        console.log("Cannot remove the professor: " + err);
        throw err;
    }
}

export async function addProfessor(profData) {
    try {
        const response = await fetch(API_ENDPOINTS.PROFESSORS, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(profData)
        });

        if (!response.ok) {
            throw new Error("Failed to add professor");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot add professor:", err);
        throw err;
    }
}

export async function updateProfessor(profId, profData) {
    try {
        const response = await fetch(`${API_ENDPOINTS.PROFESSORS}/${profId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(profData)
        });

        if (!response.ok) {
            throw new Error("Failed to update professor data for : " + profId);
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot update professro data:", err);
        throw err;
    }
}