import { createElement, createHomeButton, clearRoot, createHeader, createLoader } from '../misc/domHelpers.js';
import { getCurrentUser } from '../services/userService.js';
import {renderCourses} from './coursesView.js';
import {renderCalendar} from './calendarView.js'

// Glavna funkcija 
export async function renderHome() {
    localStorage.setItem("current_page", "home")
    
    clearRoot();
    const root = document.getElementById("root");

    const header = createHeader();

    const content = createElement("main", "main-content");
    
    // Pozdravna poruka
    const greeting = createElement("h2", "greeting-text");
    greeting.appendChild(createLoader());   
    loadCurrentUser(greeting);

    const buttonRow = createElement("div", "home-button-row");

    const coursesBtn = createHomeButton("My courses");
    coursesBtn.addEventListener("click", handleCourses);
    const notesBtn = createHomeButton("Notes");
    const calendarBtn = createHomeButton("Calendar");
    calendarBtn.addEventListener("click", hangleCalendar);

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
    }
}

async function handleCourses(){
    renderCourses();
}

async function hangleCalendar()
{
    renderCalendar();
}