export const API_BASE = "https://localhost:7224/api";

export const ROUTES = {
    LOGIN: 'login',
    REGISTER: 'register',
    HOME: 'home',
    COURSES: 'courses',
    PROFESSORS: 'professors'
};

export const API_ENDPOINTS = {
    LOGIN: `${API_BASE}/Auth/login`,
    REGISTER: `${API_BASE}/Auth/register`,
    LOGOUT: `${API_BASE}/Auth/logout`,
    CURR_USER: `${API_BASE}/Auth/me`,
    COURSES: `${API_BASE}/Courses`,
    PROFESSORS: `${API_BASE}/Professors`
};