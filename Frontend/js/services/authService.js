let authToken = localStorage.getItem("app_token");

export function setToken(token) {
    authToken = token;
    if(token){
        localStorage.setItem("app_token", token);
    }
    else{
        localStorage.removeItem("app_token");
    }
    console.log("Token saved in localStorage")
}

export function getToken() {
    return authToken;
}

export function clearToken() {
    authToken = null;
    localStorage.removeItem("app_token");
    console.log("Successfully removed token!")
}

export function isAuthenticated() {
    return authToken !== null && authToken !== "undefined";
}

export function getAuthHeaders() {
    const token = getToken()
    const headers = {
        "Content-Type": "application/json"
    };    
    if (token) {
        headers["Authorization"] = `Bearer ${authToken}`;
    } else {
        console.warn("No token available for auth header!");
    }
    
    return headers;
}