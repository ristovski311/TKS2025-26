import { API_ENDPOINTS } from '../config/config.js';


export async function getNotesByUserId(userId) {
    try {
        const response = await fetch(API_ENDPOINTS.NOTES_BY_USER(userId), {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to fetch notes");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot get notes:", err);
        throw err;
    }
}

export async function createNote(noteData) {
    try {
        const response = await fetch(API_ENDPOINTS.NOTES, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(noteData)
        });

        if (!response.ok) {
            throw new Error("Failed to create note");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot create note:", err);
        throw err;
    }
}

export async function updateNote(noteId, noteData) {
    try {
        const response = await fetch(`${API_ENDPOINTS.NOTES}/${noteId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(noteData)
        });

        if (!response.ok) {
            throw new Error("Failed to update note");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot update note:", err);
        throw err;
    }
}

export async function deleteNote(noteId) {
    try {
        const response = await fetch(`${API_ENDPOINTS.NOTES}/${noteId}`, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to delete note (folder) : " + noteId);
        }
    } catch (err) {
        console.error("Cannot delete note (folder):", err);
        throw err;
    }
}