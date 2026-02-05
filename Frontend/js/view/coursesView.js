import { createElement, clearRoot } from '../misc/domHelpers.js';
import { getCourses, getProfessorById, getProfessorsByUserId, getCurrentUser, createCourse } from '../services/apiService.js';
import { createHeader } from '../misc/domHelpers.js'

export async function renderCourses() {
    localStorage.setItem("current_page", "courses")
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

    // Floating action button za dodavanje kursa
    const fab = createElement("button", "fab", "+ Add Course");
    fab.addEventListener("click", openAddCourseModal);
    root.appendChild(fab);

    await loadCourses(coursesGrid);
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



async function openAddCourseModal() {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Add New Course");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createCourseForm();
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function createCourseForm() {
    const form = createElement("form", "course-form");
    form.addEventListener("submit", handleCourseSubmit);
    
    // Title
    const titleGroup = createElement("div", "form-group");
    const titleLabel = createElement("label", "form-label", "Course Title");
    const titleInput = createElement("input", "form-input");
    titleInput.type = "text";
    titleInput.name = "title";
    titleInput.required = true;
    titleGroup.append(titleLabel, titleInput);
    
    // Semester & Grade row
    const rowGroup = createElement("div", "form-row");
    
    const semesterGroup = createElement("div", "form-group");
    const semesterLabel = createElement("label", "form-label", "Semester");
    const semesterInput = createElement("input", "form-input");
    semesterInput.type = "number";
    semesterInput.name = "semester";
    semesterInput.min = "1";
    semesterInput.max = "12";
    semesterInput.required = true;
    semesterGroup.append(semesterLabel, semesterInput);
    
    const gradeGroup = createElement("div", "form-group");
    const gradeLabel = createElement("label", "form-label", "Grade");
    const gradeInput = createElement("input", "form-input");
    gradeInput.type = "number";
    gradeInput.name = "grade";
    gradeInput.min = "5";
    gradeInput.max = "10";
    gradeGroup.append(gradeLabel, gradeInput);
    
    rowGroup.append(semesterGroup, gradeGroup);
    
    // Professor select
    const professorGroup = createElement("div", "form-group");
    const professorLabel = createElement("label", "form-label", "Professor");
    const professorSelect = createElement("select", "form-select");
    professorSelect.name = "professorId";
    professorSelect.required = true;
    
    const defaultOption = createElement("option", "", "Select a professor");
    defaultOption.value = "";
    defaultOption.disabled = true;
    defaultOption.selected = true;  
    professorSelect.appendChild(defaultOption);
    
    try {
        const currentUser = await getCurrentUser();
        const professors = await getProfessorsByUserId(currentUser.id);
        professors.forEach(prof => {
            const option = createElement("option", "", `${prof.firstName} ${prof.lastName} - ${prof.office}`);
            option.value = prof.id;
            professorSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Failed to load professors:", err);
    }
    
    professorGroup.append(professorLabel, professorSelect);
    
    // Description
    const descGroup = createElement("div", "form-group");
    const descLabel = createElement("label", "form-label", "Description");
    const descTextarea = createElement("textarea", "form-textarea");
    descTextarea.name = "description";
    descTextarea.required = true;
    descGroup.append(descLabel, descTextarea);
    
    // Actions
    const actions = createElement("div", "form-actions");
    const submitBtn = createElement("button", "btn-submit", "Create Course");
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Cancel");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => {
        document.querySelector(".modal-overlay").remove();
    });
    actions.append(cancelBtn, submitBtn);
    
    form.append(titleGroup, rowGroup, professorGroup, descGroup, actions);
    
    return form;
}

async function handleCourseSubmit(e) {
    e.preventDefault();
    
    const currentUser = await getCurrentUser();
    const formData = new FormData(e.target);
    const courseData = {
        title: formData.get("title"),
        semester: parseInt(formData.get("semester")),
        description: formData.get("description"),
        grade: formData.get("grade") ? parseInt(formData.get("grade")) : null,
        userId: currentUser.id,
        professorId: parseInt(formData.get("professorId"))
    };
    
    try {
        await createCourse(courseData);
        document.querySelector(".modal-overlay").remove();
        renderCourses();
    } catch (err) {
        alert("Greška pri kreiranju kursa: " + err.message);
    }
}