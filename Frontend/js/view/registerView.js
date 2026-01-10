import { clearRoot, createInput, createButton, createError, createElement } from '../misc/domHelpers.js';
import { registerUser } from '../services/apiService.js';
import { renderLogin } from './loginView.js';

export function renderRegister() {
    clearRoot();
    const root = document.getElementById("root");

    const wrapper = createElement("div", "auth-wrapper");
    const container = createElement("div", "auth-container");
    const title = createElement("h2", "auth-title", "Register");
    const form = createElement("form", "auth-form");

    const username = createInput("text", "Username", "register-username");
    const email = createInput("email", "Email", "register-email");
    const password = createInput("password", "Password", "register-pass");
    const firstName = createInput("text", "First name", "register-first-name");
    const lastName = createInput("text", "Last name", "register-last-name");
    const semester = createInput("number", "Semester", "register-semester");
    const phone = createInput("text", "Phone", "register-phone");

    const button = createButton("Register");
    const error = createError();

    form.append(username, email, password, firstName, lastName, semester, phone, button);
    form.addEventListener("submit", handleRegister);

    const switchText = createElement("p", "auth-switch", "Već imate nalog? ");
    const link = createElement("span", "auth-link", "Prijavite se");
    link.addEventListener("click", renderLogin);
    switchText.appendChild(link);

    container.append(title, form, switchText, error);
    wrapper.appendChild(container);
    root.appendChild(wrapper);

    form._error = error;
}

async function handleRegister(e) {
    e.preventDefault();

    const form = e.target;

    const userData = {
        username: form.querySelector(".register-username").value,
        email: form.querySelector(".register-email").value,
        password: form.querySelector(".register-pass").value,
        firstName: form.querySelector(".register-first-name").value,
        lastName: form.querySelector(".register-last-name").value,
        semester: Number(form.querySelector(".register-semester").value),
        phone: form.querySelector(".register-phone").value
    };

    try {
        await registerUser(userData);
        renderLogin();
    } catch (err) {
        form._error.textContent = err.message;
    }
}