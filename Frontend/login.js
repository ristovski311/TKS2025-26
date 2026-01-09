const API_BASE = "https://localhost:7224/api/Auth";
const root = document.getElementById("root");

renderLogin();

function clearRoot() {
    root.innerHTML = "";
}

function createInput(type, placeholder, classname) {
    const input = document.createElement("input");
    input.type = type;
    input.placeholder = placeholder;
    input.className = "auth-input";
    input.classList.add(classname)
    return input;
}

function createButton(text) {
    const btn = document.createElement("button");
    btn.textContent = text;
    btn.type = "submit";
    btn.className = "auth-button";
    return btn;
}

function createError() {
    const p = document.createElement("p");
    p.className = "auth-error";
    return p;
}

function renderLogin() {
    clearRoot();

    const container = document.createElement("div");
    container.className = "auth-container";

    const title = document.createElement("h2");
    title.className = "auth-title";
    title.textContent = "Login";

    const form = document.createElement("form");
    form.className = "auth-form";

    const emailInput = createInput("email", "Email", "login-email");
    const passwordInput = createInput("password", "Password", "login-pass");

    const button = createButton("Login");
    const error = createError();

    form.append(emailInput, passwordInput, button);
    form.addEventListener("submit", handleLogin);

    const switchText = document.createElement("p");
    switchText.className = "auth-switch";
    switchText.textContent = "Nemate nalog? ";

    const link = document.createElement("span");
    link.className = "auth-link";
    link.textContent = "Registrujte se";
    link.addEventListener("click", renderRegister);

    switchText.appendChild(link);

    container.append(title, form, switchText, error);

    const wrapper = document.createElement("div");
    wrapper.className = "auth-wrapper";

    wrapper.appendChild(container);
    root.appendChild(wrapper)

    form._error = error;
}


function renderRegister() {
    clearRoot();

    const container = document.createElement("div");
    container.className = "auth-container";

    const title = document.createElement("h2");
    title.className = "auth-title";
    title.textContent = "Register";

    const form = document.createElement("form");
    form.className = "auth-form";

    const username = createInput("text", "Username", "register-username");
    const email = createInput("email", "Email", "register-email");
    const password = createInput("password", "Password", "register-pass");
    const firstName = createInput("text", "First name", "register-first-name");
    const lastName = createInput("text", "Last name", "register-last-name");
    const semester = createInput("number", "Semester", "register-semester");
    const phone = createInput("text", "Phone", "register-phone");

    const button = createButton("Register");
    const error = createError();

    form.append(
        username,
        email,
        password,
        firstName,
        lastName,
        semester,
        phone,
        button
    );

    form.addEventListener("submit", handleRegister);

    const switchText = document.createElement("p");
    switchText.className = "auth-switch";
    switchText.textContent = "Već imate nalog? ";

    const link = document.createElement("span");
    link.className = "auth-link";
    link.textContent = "Prijavite se";
    link.addEventListener("click", renderLogin);

    switchText.appendChild(link);

    container.append(title, form, switchText, error);
    const wrapper = document.createElement("div");
    wrapper.className = "auth-wrapper";

    wrapper.appendChild(container);
    root.appendChild(wrapper)

    form._error = error;
}


async function handleLogin(e) {
    e.preventDefault();
    const form = e.target;

    const body = {
        email: document.querySelector(".login-email").value,
        password: document.querySelector(".login-pass").value
    };

    try {
        const res = await fetch(`${API_BASE}/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (!res.ok) throw new Error("Neuspešan login");

        const data = await res.json();
        console.log("Login success:", data);

        // localStorage.setItem("token", data.token);

        renderHome();

    } catch (err) {
        form._error.textContent = err.message;
    }
}

async function handleRegister(e) {
    e.preventDefault();
    const form = e.target;

    const body = {
        username: form.querySelector(".register-username").value,
        email: form.querySelector(".register-email").value,
        password: form.querySelector(".register-pass").value,
        firstName: form.querySelector(".register-first-name").value,
        lastName: form.querySelector(".register-last-name").value,
        semester: Number(form.querySelector(".register-semester").value),
        phone: form.querySelector(".register-phone").value
    };

    try {
        const res = await fetch(`${API_BASE}/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (!res.ok) throw new Error("Registracija nije uspela");

        renderLogin();
    } catch (err) {
        form._error.textContent = err.message;
    }
}

function formatDate(date) {
    const d = String(date.getDate()).padStart(2, "0");
    const m = String(date.getMonth() + 1).padStart(2, "0");
    const y = date.getFullYear();

    return `${d}.${m}.${y}.`;
}

function createHomeButton(text) {
    const btn = document.createElement("button");
    btn.className = "home-button";
    btn.textContent = text;
    return btn;
}

function renderHome() {
    clearRoot();

    const header = document.createElement("header");
    header.className = "main-header";

    const title = document.createElement("h1");
    title.className = "app-title";
    title.textContent = "NoteIT!";

    const rightSection = document.createElement("div");
    rightSection.className = "header-right";

    const date = document.createElement("span");
    date.className = "header-date";
    date.textContent = formatDate(new Date());

    const logoutBtn = document.createElement("button");
    logoutBtn.className = "logout-button";
    logoutBtn.textContent = "Logout";
    logoutBtn.addEventListener("click", handleLogout);

    rightSection.append(date, logoutBtn);
    header.append(title, rightSection);

    const content = document.createElement("main");
    content.className = "main-content";
    
    const buttonRow = document.createElement("div");
    buttonRow.className = "home-button-row";

    const coursesBtn = createHomeButton("My courses");
    const notesBtn = createHomeButton("Notes");
    const calendarBtn = createHomeButton("Calendar");

    buttonRow.append(coursesBtn, notesBtn, calendarBtn);

    const centerText = document.createElement("h2");
    centerText.className = "home-center-text";
    centerText.textContent = "What is up today?";

    content.append(buttonRow, centerText);

    root.append(header, content);
}

async function handleLogout() {
    try {
        await fetch(`${API_BASE}/logout`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                // "Authorization": `Bearer ${localStorage.getItem("token")}`
            }
        });
    } catch (err) {
        console.warn("Logout failed:", err);
    } finally {
        // localStorage.removeItem("token");
        renderLogin();
    }
}   