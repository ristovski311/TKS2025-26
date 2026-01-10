// authService.js
// Pošto ne možemo koristiti localStorage u artifacts,
// koristimo in-memory storage za token

let authToken = null;

export function setToken(token) {
    authToken = token;
    console.log("🔐 Token set in authService:", token ? "YES" : "NO"); // DEBUG
}

export function getToken() {
    console.log("🔍 Getting token:", authToken ? authToken.substring(0, 20) + "..." : "NULL"); // DEBUG
    return authToken;
}

export function clearToken() {
    authToken = null;
    console.log("🗑️ Token cleared"); // DEBUG
}

export function isAuthenticated() {
    return authToken !== null;
}

export function getAuthHeaders() {
    const headers = {
        "Content-Type": "application/json"
    };
    
    if (authToken) {
        headers["Authorization"] = `Bearer ${authToken}`;
        console.log("✅ Authorization header added"); // DEBUG
    } else {
        console.warn("⚠️ No token available for auth header!"); // DEBUG
    }
    
    return headers;
}