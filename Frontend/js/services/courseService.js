import { API_ENDPOINTS } from '../config/config.js';

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

export async function createCourse(courseData) {
    try {
        const response = await fetch(API_ENDPOINTS.COURSES, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(courseData)
        });

        if (!response.ok) {
            throw new Error("Failed to create course");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot create course:", err);
        throw err;
    }
}

export async function updateCourse(courseId, courseData) {
    try {
        const response = await fetch(`${API_ENDPOINTS.COURSES}/${courseId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(courseData)
        });

        if (!response.ok) {
            throw new Error("Failed to update course");
        }

        return await response.json();
    } catch (err) {
        console.error("Cannot update course:", err);
        throw err;
    }
}