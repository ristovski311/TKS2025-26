import { renderLogin } from '../view/loginView.js';
import { renderHome } from '../view/homeView.js';
import { renderCourses } from '../view/coursesView.js'
import { renderNotes } from '../view/notesView.js'
import { renderCalendar } from '../view/calendarView.js'
import { formatDate } from './utils.js'
import { logoutUser} from '../services/userService.js';
import {renderProfessors} from '../view/professorsView.js'

export function clearRoot() {
    const root = document.getElementById("root");
    if (root) {
        root.innerHTML = "";
    }
}

export function createInput(type, placeholder, className) {
    const input = document.createElement("input");
    input.type = type;
    input.placeholder = placeholder;
    input.className = "auth-input";
    if (className) {
        input.classList.add(className);
    }
    return input;
}

export function createButton(text, className = "auth-button") {
    const btn = document.createElement("button");
    btn.textContent = text;
    btn.type = "submit";
    btn.className = className;
    return btn;
}

export function createError() {
    const p = document.createElement("p");
    p.className = "auth-error";
    return p;
}

export function createHomeButton(text) {
    const btn = document.createElement("button");
    btn.className = "home-button";
    btn.textContent = text;
    return btn;
}

export function createElement(tag, className, textContent = "") {
    const el = document.createElement(tag);
    if (className) el.className = className;
    if (textContent) el.textContent = textContent;
    return el;
}

export function createHeader() {
    const header = createElement("header", "main-header");
    const title = createElement("h1", "app-title", "NoteIT!");
    title.addEventListener("click", renderHome); 
    const rightSection = createElement("div", "header-right");

    const date = createElement("span", "header-date", formatDate(new Date()));
    
    const logoutBtn = document.createElement("button");
    logoutBtn.className = "logout-button";
    logoutBtn.textContent = "Logout";
    logoutBtn.addEventListener("click", handleLogout);

    rightSection.append(date, logoutBtn);
    header.append(title, rightSection);
    
    return header;
}

export function createConfirmModal(message, onConfirm, onCancel) {
    const overlay = document.createElement("div");
    overlay.className = "modal-overlay";

    const modal = document.createElement("div");
    modal.className = "modal-content";

    const header = document.createElement("div");
    header.className = "modal-header";

    const title = document.createElement("h2");
    title.className = "modal-title";
    title.textContent = "Confirm";

    const closeBtn = document.createElement("button");
    closeBtn.className = "modal-close";
    closeBtn.innerHTML = "&times;";

    closeBtn.onclick = () => {
        document.body.removeChild(overlay);
        if (onCancel) onCancel();
    };

    header.append(title, closeBtn);

    const text = document.createElement("p");
    text.style.textAlign = "center";
    text.style.marginBottom = "20px";
    text.style.fontSize = "16px";
    text.style.color = "#555";
    text.textContent = message;

    const actions = document.createElement("div");
    actions.className = "form-actions";

    const confirmBtn = document.createElement("button");
    confirmBtn.className = "btn-submit";
    confirmBtn.textContent = "Yes";

    const cancelBtn = document.createElement("button");
    cancelBtn.className = "btn-cancel";
    cancelBtn.textContent = "No";

    confirmBtn.onclick = () => {
        document.body.removeChild(overlay);
        onConfirm();
    };

    cancelBtn.onclick = () => {
        document.body.removeChild(overlay);
        if (onCancel) onCancel();
    };

    actions.append(confirmBtn, cancelBtn);

    modal.append(header, text, actions);
    overlay.appendChild(modal);
    document.body.appendChild(overlay);
}

async function handleLogout() {
    createConfirmModal(
        "Are you sure you want to logout?",
        async () => {
            await logoutUser();
            renderLogin();
        }
    )
}

export function createSkeletonCard() {
    const card = createElement("div", "course-card skeleton-card");

    const title = createElement("div", "skeleton skeleton-title");
    const professor = createElement("div", "skeleton skeleton-text");
    const semester = createElement("div", "skeleton skeleton-text");
    const description = createElement("div", "skeleton skeleton-description");
    const semester2 = createElement("div", "skeleton skeleton-text");
    const description2 = createElement("div", "skeleton skeleton-description");
    const footer = createElement("div", "skeleton skeleton-footer");

    card.append(title, professor, semester, description, description2, semester2, footer);

    return card;
}

export function createLoader() {
    const loader = document.createElement("div");
    loader.className = "loader";
    return loader;
}

export function showLoadingOverlay() {
    const overlay = document.createElement("div");
    overlay.className = "loading-overlay";
    overlay.id = "loading-overlay";

    const loader = createLoader();
    overlay.appendChild(loader);
    document.body.appendChild(overlay);

    return () => overlay.remove();
}

export function createNavBar()
{
    const navBar = createElement("nav", "main-nav");

    const navItemHome = createElement("button", "main-nav-item main-nav-item-home", "Home");
    const navItemCourses = createElement("button", "main-nav-item main-nav-item-courses", "Courses");
    const navItemNotes = createElement("button", "main-nav-item main-nav-item-notes", "Notes");
    const navItemCalendar = createElement("button", "main-nav-item main-nav-item-calendar", "Calendar");
    const navItemProfessors = createElement("button", "main-nav-item main-nav-item-professors", "Professors");
    
    navBar.append(navItemHome, navItemProfessors, navItemCourses, navItemNotes, navItemCalendar);

    navItemHome.addEventListener("click", renderHome);
    navItemCourses.addEventListener("click", renderCourses)
    navItemCalendar.addEventListener("click", renderCalendar)
    navItemNotes.addEventListener("click", renderNotes)
    navItemProfessors.addEventListener("click", renderProfessors)
    
    applySelected(navBar);

    return navBar;
}

function applySelected(navbar)
{
    const currentPage = localStorage.getItem("current_page") || "home";

    const map = {
        home: ".main-nav-item-home",
        courses: ".main-nav-item-courses",
        calendar: ".main-nav-item-calendar",
        notes: ".main-nav-item-notes",
        professors: ".main-nav-item-professors"
    };

    const selectedSelector = map[currentPage];

    if (!selectedSelector) return;

    const selectedItem = navbar.querySelector(selectedSelector);

    if (selectedItem)
    {
        selectedItem.classList.add("main-nav-item-selected");
    }
}