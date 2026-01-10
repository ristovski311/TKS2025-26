import { clearRoot, createInput, createButton, createError, createElement } from '../misc/domHelpers.js';
import { loginUser } from '../services/apiService.js';
import { renderRegister } from './registerView.js';
import { renderHome } from './homeView.js';

export function renderLogin() {
    clearRoot();
    const root = document.getElementById("root");

    const wrapper = createElement("div", "auth-wrapper");
    const container = createElement("div", "auth-container");
    const title = createElement("h2", "auth-title", "Login");
    const form = createElement("form", "auth-form");

    const emailInput = createInput("email", "Email", "login-email");
    const passwordInput = createInput("password", "Password", "login-pass");
    const button = createButton("Login");
    const error = createError();

    form.append(emailInput, passwordInput, button);
    form.addEventListener("submit", handleLogin);

    const switchText = createElement("p", "auth-switch", "Nemate nalog? ");
    const link = createElement("span", "auth-link", "Registrujte se");
    link.addEventListener("click", renderRegister);
    switchText.appendChild(link);

    container.append(title, form, switchText, error);
    wrapper.appendChild(container);
    root.appendChild(wrapper);

    form._error = error;
}

async function handleLogin(e) {
    e.preventDefault();

    const form = e.target;

    const email = document.querySelector(".login-email").value;
    const password = document.querySelector(".login-pass").value;

    try {
        const data = await loginUser(email, password);
        console.log("Login success:", data);
        renderHome();
    } catch (err) {
        form._error.textContent = err.message;
    }
}