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