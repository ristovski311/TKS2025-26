import { createElement, createHomeButton, clearRoot, createHeader, createLoader, createNavBar } from '../misc/domHelpers.js';
import { getCurrentUser } from '../services/userService.js';
import {renderCourses} from './coursesView.js';
import {renderCalendar} from './calendarView.js'

// Glavna funkcija 
export async function renderHome() {
    localStorage.setItem("current_page", "home")
    
    clearRoot();
    const root = document.getElementById("root");

    const header = createHeader();
    const nav = createNavBar();

    const content = createElement("main", "main-content");
    
    // Pozdravna poruka
    const greeting = createElement("h2", "greeting-text");
    greeting.appendChild(createLoader());   
    loadCurrentUser(greeting);

    const centerText = createElement("h2", "home-center-text", "What is up today?");

    content.append(greeting, centerText);
    root.append(header, nav, content);
}

async function loadCurrentUser(greetingElement) {
    try {
        const user = await getCurrentUser();
        greetingElement.textContent = `Hi, ${user.username}!`;
    } catch (error) {
        console.error("Session expired or invalid!", error);
        await handleLogout();
    }
}

async function handleCourses(){
    renderCourses();
}

async function hangleCalendar()
{
    renderCalendar();
}