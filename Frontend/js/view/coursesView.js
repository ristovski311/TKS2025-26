import { createElement, clearRoot, createSkeletonCard } from '../misc/domHelpers.js';
import { getCurrentUser } from '../services/userService.js';
import { getProfessorById, getProfessorsByUserId } from '../services/professorService.js';
import { getCourses, createCourse, updateCourse } from '../services/courseService.js'
import { createHeader } from '../misc/domHelpers.js'

export async function renderCourses() {
    localStorage.setItem("current_page", "courses");

    clearRoot();
    const root = document.getElementById("root");

    const header = createHeader();
    root.appendChild(header);

    const content = createElement("main", "main-content");
    
    const pageTitle = createElement("h1", "page-title", "My Courses");
    content.appendChild(pageTitle);

    //Sortiranje
    const sortRow = createElement("div", "sort-row");

    const sortBySelect = createElement("select", "form-select");
    sortBySelect.innerHTML = `
        <option value="title">Name</option>
        <option value="grade">Grade</option>
        <option value="semester">Semester</option>
        <option value="professor">Professor</option>
    `;

    const orderSelect = createElement("select", "form-select");
    orderSelect.innerHTML = `
        <option value="asc">Ascending</option>
        <option value="desc">Descending</option>
    `;

    sortRow.append(sortBySelect, orderSelect);
    content.appendChild(sortRow);

    const coursesGrid = createElement("div", "courses-grid");
    content.appendChild(coursesGrid);

    root.appendChild(content);

    // Floating action button za dodavanje kursa
    const fab = createElement("button", "fab", "+ Add Course");
    fab.addEventListener("click", openAddCourseModal);
    root.appendChild(fab);

    const courses = await loadCourses(coursesGrid);

    sortBySelect.value = "semester";
    orderSelect.value = "asc";
    
    setupSorting(courses, coursesGrid, sortBySelect, orderSelect);

    sortBySelect.dispatchEvent(new Event("change"));
}

async function loadCourses(container) {
    try {
        const courses = await getCourses();

        if (!courses || courses.length === 0) {
            container.innerHTML = "";
            const emptyMsg = createElement("p", "empty-message", "Nema kurseva za prikaz.");
            container.appendChild(emptyMsg);
            return [];
        }

        return courses;

    } catch (error) {
        container.innerHTML = "";
        console.error("Failed to load courses:", error);
        const errorMsg = createElement("p", "error-message", "Greska pri ucitavanju kurseva.");
        container.appendChild(errorMsg);
        return [];
    }
}

function showSkeletons(container) {
    container.innerHTML = "";

    const count = Math.floor(Math.random() * (8 - 3 + 1)) + 3;

    for (let i = 0; i < count; i++) {
        container.appendChild(createSkeletonCard());
    }
}

function setupSorting(courses, container, sortBySelect, orderSelect) {
    async function renderSorted() {
        const sortBy = sortBySelect.value;
        const order = orderSelect.value;

    showSkeletons(container);

        const sorted = [...courses].sort((a, b) => {
            let valA, valB;

            switch (sortBy) {
                case "title":
                    valA = a.title.toLowerCase();
                    valB = b.title.toLowerCase();
                    break;

                case "semester":
                    valA = a.semester;
                    valB = b.semester;
                    break;

                case "grade":
                    // ako nema ocene poslednji je
                    if (a.grade == null) return 1;
                    if (b.grade == null) return -1;
                    valA = a.grade;
                    valB = b.grade;
                    break;

                case "professor":
                    valA = a.professorId;
                    valB = b.professorId;
                    break;
            }

            if (valA < valB) return order === "asc" ? -1 : 1;
            if (valA > valB) return order === "asc" ? 1 : -1;
            return 0;
        });

        const cards = await Promise.all(
            sorted.map(course => createCourseCard(course))
        );

        container.innerHTML = "";
        
        cards.forEach(card => container.appendChild(card));
    }

    sortBySelect.addEventListener("change", renderSorted);
    orderSelect.addEventListener("change", renderSorted);
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

    if (course.grade) {
        const grade = createElement("span", "course-grade", `Ocena: ${course.grade}`);
        footer.appendChild(grade);
    } else {
        const passBtn = createElement("button", "btn-pass", "Položi ispit");
        passBtn.addEventListener("click", (e) => {
            e.stopPropagation();
            openPassExamModal(course);
        });
        footer.appendChild(passBtn);
    }

    card.append(title, professor, semester, description, footer);

    card.addEventListener("click", () => openEditCourseModal(course));
    card.style.cursor = "pointer";

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

async function createCourseForm(course = null) {
    const form = createElement("form", "course-form");
    form.addEventListener("submit", (e) => handleCourseSubmit(e, course));
    
    // Title
    const titleGroup = createElement("div", "form-group");
    const titleLabel = createElement("label", "form-label", "Course Title");
    const titleInput = createElement("input", "form-input");
    titleInput.type = "text";
    titleInput.name = "title";
    titleInput.required = true;
    if(course != null)
        titleInput.value = course.title
    titleGroup.append(titleLabel, titleInput);
    
    // Semestar
    const rowGroup = createElement("div", "form-row");
    
    const semesterGroup = createElement("div", "form-group");
    const semesterLabel = createElement("label", "form-label", "Semester");
    const semesterInput = createElement("input", "form-input");
    semesterInput.type = "number";
    semesterInput.name = "semester";
    semesterInput.min = "1";
    semesterInput.max = "12";
    semesterInput.required = true;
    if(course!=null)
        semesterInput.value = course.semester;
    semesterGroup.append(semesterLabel, semesterInput);
    
    //Ocena se unosi kad se ispit "Polozi" postoji posebna opcija za to
    // const gradeGroup = createElement("div", "form-group");
    // const gradeLabel = createElement("label", "form-label", "Grade");
    // const gradeInput = createElement("input", "form-input");
    // gradeInput.type = "number";
    // gradeInput.name = "grade";
    // gradeInput.min = "5";
    // gradeInput.max = "10";
    // gradeGroup.append(gradeLabel, gradeInput);
    
    rowGroup.append(semesterGroup);
    
    // Profesori
    const professorGroup = createElement("div", "form-group");
    const professorLabel = createElement("label", "form-label", "Professor");
    const professorSelect = createElement("select", "form-select");
    professorSelect.name = "professorId";
    professorSelect.required = true;
    
    //Na osnovu course.professorId selektuj odmah profesora

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
            if (course && prof.id === course.professorId) {
                option.selected = true;
            }
            professorSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Failed to load professors:", err);
    }
    
    professorGroup.append(professorLabel, professorSelect);
    
    // Opis
    const descGroup = createElement("div", "form-group");
    const descLabel = createElement("label", "form-label", "Description");
    const descTextarea = createElement("textarea", "form-textarea");
    descTextarea.name = "description";
    descTextarea.required = true;
    if(course!=null)
        descTextarea.value = course.description;
    descGroup.append(descLabel, descTextarea);
    
    //Dugmici
    const actions = createElement("div", "form-actions");
    const submitText = course ? "Save changes" : "Create course"; 
    const submitBtn = createElement("button", "btn-submit", submitText);
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

async function handleCourseSubmit(e, course) {
    e.preventDefault();
    
    const currentUser = await getCurrentUser();
    const formData = new FormData(e.target);
    const courseData = {
        title: formData.get("title"),
        semester: parseInt(formData.get("semester")),
        description: formData.get("description"),
        grade: course ? course.grade : null,
        userId: currentUser.id,
        professorId: parseInt(formData.get("professorId"))
    };
    
    try {
        if(course)
            await updateCourse(course.id, courseData);
        else
            await createCourse(courseData);
        document.querySelector(".modal-overlay").remove();
        renderCourses();
    } catch (err) {
        alert("Greska pri kreiranju kursa: " + err.message);
    }
}

async function openEditCourseModal(course) {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Edit Course");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createCourseForm(course);
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function openPassExamModal(course) {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    modalContent.style.maxWidth = "400px";
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Položi ispit");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = createElement("form", "course-form");
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const grade = parseInt(formData.get("grade"));
   
        const currentUser = await getCurrentUser();
        const courseData = {
            title: course.title,
            semester: course.semester,
            description: course.description,
            grade: grade,
            userId: currentUser.id,
            professorId: course.professorId
        };

        try {
            await updateCourse(course.id,courseData);
            overlay.remove();
            renderCourses();
        } catch (err) {
            alert("Greška pri polaganju ispita: " + err.message);
        }
    });
    
    const courseInfo = createElement("p", "modal-info", `Kurs: ${course.title}`);
    courseInfo.style.marginBottom = "20px";
    courseInfo.style.fontSize = "16px";
    
    const gradeGroup = createElement("div", "form-group");
    const gradeLabel = createElement("label", "form-label", "Ocena");
    const gradeInput = createElement("input", "form-input");
    gradeInput.type = "number";
    gradeInput.name = "grade";
    gradeInput.min = "6";
    gradeInput.max = "10";
    gradeInput.required = true;
    gradeInput.placeholder = "6-10";
    gradeGroup.append(gradeLabel, gradeInput);
    
    const actions = createElement("div", "form-actions");
    const submitBtn = createElement("button", "btn-submit", "Položi");
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Otkaži");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => overlay.remove());
    actions.append(cancelBtn, submitBtn);
    
    form.append(courseInfo, gradeGroup, actions);
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}
