import { renderLogin } from '../view/loginView.js';
import { renderHome } from '../view/homeView.js';
import { renderCourses } from '../view/coursesView.js'
import { renderNotes } from '../view/notesView.js'
import { renderCalendar } from '../view/calendarView.js'
import { formatDate } from './utils.js'
import { logoutUser, getCurrentUser, deleteUser } from '../services/userService.js';
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

    const titleWrapper = createElement("div", "app-title-wrapper");
    const logo = createElement("img", "app-logo");
    logo.src = "../../images/notebook_transparent.png";
    logo.alt = "NoteIT Logo";
    const title = createElement("h1", "app-title", "NoteIT!");
    title.addEventListener("click", renderHome);
    titleWrapper.append(logo, title);
    titleWrapper.addEventListener("click", renderHome);
    titleWrapper.style.cursor = "pointer";

    const rightSection = createElement("div", "header-right");
    const date = createElement("span", "header-date", formatDate(new Date()));
    
    const logoutBtn = document.createElement("button");
    logoutBtn.className = "logout-button";
    logoutBtn.textContent = "Logout";
    logoutBtn.addEventListener("click", handleLogout);
    

    const deleteBtn = document.createElement("button");
    deleteBtn.className = "delete-button";
    deleteBtn.textContent = "Delete account";
    deleteBtn.addEventListener("click", handleDeleteAccount);
    rightSection.append(date, deleteBtn, logoutBtn);
    header.append(titleWrapper, rightSection);
    
    
    return header;
}

export function createConfirmModal(message, onConfirm, onCancel, notemsg = "") {
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

    const note = document.createElement("p");
    note.style.textAlign = "center";
    note.style.marginBottom = "20px";
    note.style.fontSize = "16px";
    note.style.color = "#f40000";
    note.textContent = notemsg;

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

    modal.append(header, text, note, actions);
    overlay.appendChild(modal);
    document.body.appendChild(overlay);
}

async function handleLogout() {
    createConfirmModal(
        "Are you sure you want to logout?",
        async () => {
            const hideOverlay = showLoadingOverlay();
            try
            {
                await logoutUser();
                renderLogin();
            }
            finally
            {
                hideOverlay();
            }
        }
    )
}

async function handleDeleteAccount() {
    createConfirmModal(
        "Are you sure you want to delete your account?",
        async () => {
            const hideOverlay = showLoadingOverlay();
            try {
                const user = await getCurrentUser();
                await deleteUser(user.id);
                renderLogin();
            } catch (error) {
                if (!handleAuthError(error)) {
                    console.error("Account deletion failed:", error);
                }
            } finally {
                hideOverlay();
            }
        },
        null,
        "This action is permanent and cannot be undone."
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

export function showSkeletons(container) {
    container.innerHTML = "";

    const count = 5;

    for (let i = 0; i < count; i++) {
        container.appendChild(createSkeletonCard());
    }
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

export function handleAuthError(error) {
    if (error.message === "SESSION_EXPIRED") {
        showSessionExpiredModal();
        return true;
    }
    return false;
}

export function showSessionExpiredModal() {
    if (document.getElementById("session-expired-modal")) return;

    const overlay = createElement("div", "modal-overlay");
    overlay.id = "session-expired-modal";

    const modalContent = createElement("div", "modal-content");
    modalContent.style.maxWidth = "360px";
    modalContent.style.textAlign = "center";

    const icon = createElement("div", "session-icon", "🔒");
    icon.style.fontSize = "2.5rem";
    icon.style.marginBottom = "12px";

    const title = createElement("h2", "modal-title", "Session Expired");
    const message = createElement("p", "modal-info", "Your session has expired. Please log in again to continue.");
    message.style.margin = "12px 0 24px";

    const okBtn = createElement("button", "btn-submit", "OK");
    okBtn.style.width = "100%";
    okBtn.addEventListener("click", () => {
        overlay.remove();
        renderLogin();
    });

    modalContent.append(icon, title, message, okBtn);
    overlay.appendChild(modalContent);
    document.body.appendChild(overlay);
}