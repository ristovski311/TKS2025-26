import { renderLogin } from '../view/loginView.js';
import { renderHome } from '../view/homeView.js';
import { formatDate } from './utils.js'
import { logoutUser} from '../services/userService.js';

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

    // MESSAGE
    const text = document.createElement("p");
    text.style.textAlign = "center";
    text.style.marginBottom = "20px";
    text.style.fontSize = "16px";
    text.style.color = "#555";
    text.textContent = message;

    // ACTIONS
    const actions = document.createElement("div");
    actions.className = "form-actions";

    const confirmBtn = document.createElement("button");
    confirmBtn.className = "btn-submit";
    confirmBtn.textContent = "Logout";

    const cancelBtn = document.createElement("button");
    cancelBtn.className = "btn-cancel";
    cancelBtn.textContent = "Cancel";

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
    const footer = createElement("div", "skeleton skeleton-footer");

    card.append(title, professor, semester, description, footer);

    return card;
}

export function createLoader() {
    const loader = document.createElement("div");
    loader.className = "loader";
    return loader;
}