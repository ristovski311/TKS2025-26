import { API_ENDPOINTS } from '../config/config.js';
import { setToken, clearToken } from './authService.js';
import { getAuthHeaders } from './authService.js'; 

export async function loginUser(email, password) {
    const response = await fetch(API_ENDPOINTS.LOGIN, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
        throw new Error("Neuspešan login");
    }

    const data = await response.json();
    const token = data.token || data.accessToken || data.access_token;
    if (token) {
        setToken(token);
    }
    
    return data;
}

export async function registerUser(userData) {
    const response = await fetch(API_ENDPOINTS.REGISTER, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(userData)
    });

    if (!response.ok) {
        throw new Error("Registracija nije uspela");
    }

    return await response.json();
}

export async function logoutUser() {
    try {
        await fetch(API_ENDPOINTS.LOGOUT, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });
    } catch (err) {
        console.warn("Logout failed:", err);
    } finally {
        clearToken();
    }
}

export async function getCurrentUser() {
    try {
        const response = await fetch(API_ENDPOINTS.CURR_USER, {
            method: "GET",
            headers: getAuthHeaders(),
            credentials: "include"
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Failed to fetch current user: ${response.status}`);
        }

        const userData = await response.json();
        return userData;
    } catch (err) {
        throw err;
    }
}

export async function getCourses() {
    try {
        const response = await fetch(API_ENDPOINTS.COURSES, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        if (!response.ok) {
            throw new Error("Failed to fetch courses");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot get courses:", err);
        throw err;
    }
}

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