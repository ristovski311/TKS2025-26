import { formatDate } from '../misc/utils.js';
import { createElement, createHomeButton, clearRoot } from '../misc/domHelpers.js';
import { logoutUser, getCurrentUser } from '../services/apiService.js';
import { renderLogin } from './loginView.js';
import {renderCourses} from './coursesView.js';

export async function renderHome() {
    clearRoot();
    const root = document.getElementById("root");

    const header = createElement("header", "main-header");
    const title = createElement("h1", "app-title", "NoteIT!");
    const rightSection = createElement("div", "header-right");

    const date = createElement("span", "header-date", formatDate(new Date()));
    
    const logoutBtn = document.createElement("button");
    logoutBtn.className = "logout-button";
    logoutBtn.textContent = "Logout";
    logoutBtn.addEventListener("click", handleLogout);

    rightSection.append(date, logoutBtn);
    header.append(title, rightSection);

    const content = createElement("main", "main-content");
    
    // Pozdravna poruka
    const greeting = createElement("h2", "greeting-text", "Hi, Guest!");
    loadCurrentUser(greeting);

    const buttonRow = createElement("div", "home-button-row");

    const coursesBtn = createHomeButton("My courses");
    coursesBtn.addEventListener("click", handleCourses);
    const notesBtn = createHomeButton("Notes");
    const calendarBtn = createHomeButton("Calendar");

    buttonRow.append(coursesBtn, notesBtn, calendarBtn);

    const centerText = createElement("h2", "home-center-text", "What is up today?");

    content.append(greeting, centerText, buttonRow);
    root.append(header, content);
}

async function loadCurrentUser(greetingElement) {
    try {
        const user = await getCurrentUser();
        greetingElement.textContent = `Hi, ${user.username}!`;
    } catch (error) {
        console.error("Session expired or invalid!", error);
        await handleLogout();
        greetingElement.textContent = "Hi, Guest!";
    }
}

async function handleLogout() {
    await logoutUser();
    renderLogin();
}

async function handleCourses(){
    renderCourses();
}