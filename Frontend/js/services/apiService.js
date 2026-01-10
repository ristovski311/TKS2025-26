// apiService.js
import { API_ENDPOINTS } from '../config/config.js';
import { setToken, clearToken } from './authService.js';

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
    console.log("📥 Login response:", data);
    
    const token = data.token || data.accessToken || data.access_token;
    if (token) {
        console.log("✅ Token saved:", token.substring(0, 20) + "...");
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
            headers: { "Content-Type": "application/json" },
            credentials: "include"
        });

        console.log("📡 Response status:", response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error("❌ Error response:", errorText);
            throw new Error(`Failed to fetch current user: ${response.status}`);
        }

        const userData = await response.json();
        console.log("👤 User data received:", userData);
        return userData;
    } catch (err) {
        console.error("❌ Cannot get current user:", err);
        throw err;
    }
}