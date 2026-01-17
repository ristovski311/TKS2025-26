import { formatDate } from '../misc/utils.js';
import { createElement, clearRoot } from '../misc/domHelpers.js';
import { getCourses, logoutUser, getProfessorById } from '../services/apiService.js';
import { renderLogin } from './loginView.js';

export async function renderCourses() {
    clearRoot();
    const root = document.getElementById("root");

    const header = createHeader();
    root.appendChild(header);

    const content = createElement("main", "main-content");
    
    const pageTitle = createElement("h1", "page-title", "My Courses");
    content.appendChild(pageTitle);

    const coursesGrid = createElement("div", "courses-grid");
    content.appendChild(coursesGrid);

    root.appendChild(content);

    await loadCourses(coursesGrid);
}

function createHeader() {
    const header = createElement("header", "main-header");
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

async function loadCourses(container) {
    try {
        const courses = await getCourses();
        
        if (!courses || courses.length === 0) {
            const emptyMsg = createElement("p", "empty-message", "Nema kurseva za prikaz.");
            container.appendChild(emptyMsg);
            return;
        }

        const cardPromises = courses.map(course => createCourseCard(course));
        const cards = await Promise.all(cardPromises);
        
        cards.forEach(card => {
            container.appendChild(card);
        });
    } catch (error) {
        console.error("Failed to load courses:", error);
        const errorMsg = createElement("p", "error-message", "Greška pri učitavanju kurseva.");
        container.appendChild(errorMsg);
    }
}

async function createCourseCard(course) {
    const card = createElement("div", "course-card");

    const title = createElement("h3", "course-title", course.title);
    
    let professorText = `Prof. ${course.professorId}`;
    try {
        const professor = await getProfessorById(course.professorId);
        professorText = `Prof. ${professor.firstName} ${professor.lastName} | ${professor.office}`;
    } catch (err) {
        console.warn("Failed to load professor:", err);
    }
    
    const professor = createElement("p", "course-professor", professorText);
    const semester = createElement("p", "course-semester", `Semester: ${course.semester}`);
    const description = createElement("p", "course-description", course.description);
    
    const footer = createElement("div", "course-footer");
    const grade = createElement("span", "course-grade", `Ocena: ${course.grade || 'N/A'}`);
    footer.appendChild(grade);

    card.append(title, professor, semester, description, footer);

    return card;
}