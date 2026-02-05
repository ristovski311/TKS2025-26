import { renderLogin } from '../view/loginView.js';
import { renderHome } from '../view/homeView.js';
import { formatDate } from './utils.js'

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
    header.addEventListener("click", renderHome); 
    const title = createElement("h1", "app-title", "NoteIT!");
    const rightSection = createElement("div", "header-right");

    const date = createElement("span", "header-date", formatDate(new Date()));
    
    const logoutBtn = document.createElement("button");
    logoutBtn.className = "logout-button";
    logoutBtn.textContent = "Logout";
    logoutBtn.addEventListener("click", async () => {
        await logoutUser();
        renderLogin();
    });

    rightSection.append(date, logoutBtn);
    header.append(title, rightSection);
    
    return header;
}