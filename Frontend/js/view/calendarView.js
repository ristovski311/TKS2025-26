import { formatDate } from '../misc/utils.js';
import { clearRoot,createElement, createNavBar, showLoadingOverlay } from '../misc/domHelpers.js';
import { logoutUser} from '../services/userService.js';
import { renderLogin } from './loginView.js';
import { renderHome } from './homeView.js';
import { getCourses } from '../services/courseService.js'
import { getTasks, createTask, updateTask } from '../services/taskService.js'
import { getCurrentUser } from '../services/userService.js';

let currentDate = new Date();
let tasks = [];

export async function renderCalendar() {
    localStorage.setItem("current_page", "calendar");

    clearRoot();
    const root = document.getElementById("root");

    // Header
    const header = createHeader();
    const nav = createNavBar();    
    root.append(header, nav);

    const content = createElement("main", "main-content");
    
    const pageTitle = createElement("h1", "page-title", "Calendar");
    content.appendChild(pageTitle);

    // Tasks

    const hideOverlay = showLoadingOverlay();
    try
    {
        tasks = await getTasks();
    }
    finally
    {
        hideOverlay();
    }

    // Calendar container
    const calendarContainer = createElement("div", "calendar-container");
    
    // Calendar header sa navigacijom
    const calendarHeader = createCalendarHeader();
    calendarContainer.appendChild(calendarHeader);
    
    // Calendar grid
    const calendarGrid = createCalendarGrid();
    calendarContainer.appendChild(calendarGrid);

    // Floating Action Button
    const fab = createElement("button", "fab", "+ Add Task");
    fab.addEventListener("click", openAddTaskModal);
    root.appendChild(fab);
    
    content.appendChild(calendarContainer);
    root.appendChild(content);
}

function createHeader() {
    const header = createElement("header", "main-header");
    const title = createElement("h1", "app-title", "NoteIT!");
    title.style.cursor = "pointer";
    title.addEventListener("click", renderHome);
    
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

function createCalendarHeader() {
    const header = createElement("div", "calendar-header");
    
    const prevBtn = createElement("button", "calendar-nav-btn", "‹");
    prevBtn.addEventListener("click", () => {
        currentDate.setMonth(currentDate.getMonth() - 1);
        updateCalendar();
    });
    
    const monthYear = createElement("h2", "calendar-month-year");
    updateMonthYearDisplay(monthYear);
    
    const nextBtn = createElement("button", "calendar-nav-btn", "›");
    nextBtn.addEventListener("click", () => {
        currentDate.setMonth(currentDate.getMonth() + 1);
        updateCalendar();
    });
    
    const todayBtn = createElement("button", "calendar-today-btn", "Today");
    todayBtn.addEventListener("click", () => {
        currentDate = new Date();
        updateCalendar();
    });
    
    header.append(prevBtn, monthYear, nextBtn, todayBtn);
    
    return header;
}

function updateMonthYearDisplay(element) {
    const months = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    element.textContent = `${months[currentDate.getMonth()]} ${currentDate.getFullYear()}`;
}

function createCalendarGrid() {
    const grid = createElement("div", "calendar-grid");
    
    // Dani u nedelji
    const daysOfWeek = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];
    daysOfWeek.forEach(day => {
        const dayHeader = createElement("div", "calendar-day-header", day);
        grid.appendChild(dayHeader);
    });
    
    // Dani u mesecu
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();
    
    // Prvi dan meseca (0 = nedelja, 1 = ponedeljak, ...)
    const firstDay = new Date(year, month, 1).getDay();
    // Prebaci da ponedeljak bude prvi (0)
    const firstDayAdjusted = firstDay === 0 ? 6 : firstDay - 1;
    
    // Broj dana u mesecu
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    
    // Broj dana u prethodnom mesecu
    const daysInPrevMonth = new Date(year, month, 0).getDate();
    
    // Dani iz prethodnog meseca
    for (let i = firstDayAdjusted - 1; i >= 0; i--) {
        const dayCell = createElement("div", "calendar-day other-month", String(daysInPrevMonth - i));
        grid.appendChild(dayCell);
    }
    
    // Dani trenutnog meseca
    const today = new Date();
    for (let day = 1; day <= daysInMonth; day++) {
        const dayCell = createElement("div", "calendar-day");

        const dayNumber = createElement("div", "day-number", String(day));
        dayCell.appendChild(dayNumber);

        const tasksContainer = createElement("div", "task-container");
        
        tasks.forEach((task) => {
            const d = new Date(task.date);
            if (d.getFullYear() === year && d.getMonth() === month && d.getDate() === day) {
                const pill = createElement("div", "task-pill", task.title);
                pill.classList.add(getCourseColorClass(task.courseId));
                if (task.completed) {
                    pill.classList.add("task-completed");
                }
                pill.addEventListener("click", () => {
                    openEditTaskModal(task);
                });
                tasksContainer.appendChild(pill);
            }
        });

        dayCell.appendChild(tasksContainer);

        // Označi današnji dan
        if (year === today.getFullYear() && 
            month === today.getMonth() && 
            day === today.getDate()) {
            dayCell.classList.add("today");
        }
        
        grid.appendChild(dayCell);
    }
    
    // Dani iz sledećeg meseca
    const remainingCells = 42 - (firstDayAdjusted + daysInMonth); // 6 redova x 7 dana
    for (let day = 1; day <= remainingCells; day++) {
        const dayCell = createElement("div", "calendar-day other-month", String(day));
        grid.appendChild(dayCell);
    }
    
    return grid;
}

function updateCalendar() {
    const calendarContainer = document.querySelector(".calendar-container");
    
    // Update month/year display
    const monthYearEl = document.querySelector(".calendar-month-year");
    updateMonthYearDisplay(monthYearEl);
    
    // Regenerate calendar grid
    const oldGrid = document.querySelector(".calendar-grid");
    const newGrid = createCalendarGrid();
    calendarContainer.replaceChild(newGrid, oldGrid);
}

async function openAddTaskModal() {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Add New Task");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createTaskForm();
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function openEditTaskModal(task) {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Edit Task");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createTaskForm(task);
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function createTaskForm(task = null) {
    const form = createElement("form", "course-form");
    form.addEventListener("submit", (e) => handleTaskSubmit(e, task));
    
    // Title
    const titleGroup = createElement("div", "form-group");
    const titleLabel = createElement("label", "form-label", "Task Title");
    const titleInput = createElement("input", "form-input");
    titleInput.type = "text";
    titleInput.name = "title";
    titleInput.required = true;
    if(task != null)
        titleInput.value = task.title
    titleGroup.append(titleLabel, titleInput);

    // Type
    const typeGroup = createElement("div", "form-group");
    const typeLabel = createElement("label", "form-label", "Task Type");
    const typeInput = createElement("input", "form-input");
    typeInput.type = "text";
    typeInput.name = "type";
    typeInput.required = true;
    if(task != null)
        typeInput.value = task.type;
    typeGroup.append(typeLabel, typeInput);

    // Description
    const descriptionGroup = createElement("div", "form-group");
    const descriptionLabel = createElement("label", "form-label", "Task Description");
    const descriptionInput = createElement("input", "form-input");
    descriptionInput.type = "text";
    descriptionInput.name = "description";
    descriptionInput.required = true;
    if(task != null)
        descriptionInput.value = task.description;
    descriptionGroup.append(descriptionLabel, descriptionInput);

    // Date
    const dateGroup = createElement("div", "form-group");
    const dateLabel = createElement("label", "form-label", "Date");
    const dateInput = createElement("input", "form-input");
    dateInput.type = "date";
    dateInput.name = "date";
    dateInput.required = true;
    if(task != null)
        dateInput.value = task.date.split("T")[0];
    dateGroup.append(dateLabel, dateInput);

    // Grade Max
    const gradeGroup = createElement("div", "form-group");
    const gradeLabel = createElement("label", "form-label", "Max Grade");
    const gradeInput = createElement("input", "form-input");
    gradeInput.type = "number";
    gradeInput.name = "grade";
    if (task != null)
        gradeInput.value = task.gradeMax;
    gradeGroup.append(gradeLabel, gradeInput);

    

    // Course
    const courseGroup = createElement("div", "form-group");
    const courseLabel = createElement("label", "form-label", "Course");
    const courseSelect = createElement("select", "form-select");
    courseSelect.name = "courseId";
    courseSelect.required = true;

    const defaultOption = createElement("option", "", "Select a course");
    defaultOption.value = "";
    defaultOption.disabled = true;
    defaultOption.selected = true;  
    courseSelect.appendChild(defaultOption);
    
    try {
        const currentUser = await getCurrentUser();
        const courses = await getCourses();
        courses.forEach(course => {
            const option = createElement("option", "", course.title);
            option.value = course.id;
            if (task && course.id === task.courseId) {
                option.selected = true;
            }
            courseSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Failed to load courses:", err);
    }
    
    courseGroup.append(courseLabel, courseSelect);

    // Earned grade
    const footer = createElement("div", "course-footer");

    if (task.completed) {
        const grade = createElement("span", "course-grade", `Grade: ${task.gradeEarned}`);
        footer.appendChild(grade);
    } else {
        const passBtn = createElement("button", "btn-pass", "Finish task");
        passBtn.type = "button";
        passBtn.addEventListener("click", (e) => {
            console.log("CLCIKED");
            e.stopPropagation();
            openFinishTaskModal(task);
        });
        footer.appendChild(passBtn);
    }

    // Dugmici
    const actions = createElement("div", "form-actions");
    const submitText = task ? "Save changes" : "Create task"; 
    const submitBtn = createElement("button", "btn-submit", submitText);
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Cancel");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => {
        document.querySelector(".modal-overlay").remove();
    });
    actions.append(cancelBtn, submitBtn);
    
    form.append(titleGroup, typeGroup, descriptionGroup, dateGroup, gradeGroup, courseGroup, footer, actions);
    
    return form;
}

async function handleTaskSubmit(e, task) {
    e.preventDefault();
    
    const formData = new FormData(e.target);
    const taskData = {
        title: formData.get("title"),
        type: formData.get("type"),
        description: formData.get("description"),
        date: `${formData.get("date")}T12:00:00`,
        completed: task?.completed ?? false,
        gradeMax: formData.get("grade"),
        courseId: formData.get("courseId")
    };
    
    try {
        if(task)
            await updateTask(task.id, taskData);
        else
            await createTask(taskData);
        document.querySelector(".modal-overlay").remove();
        renderCalendar();
    } catch (err) {
        alert("Failed to create task: " + err.message);
    }
}

async function openFinishTaskModal(task) {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    modalContent.style.maxWidth = "400px";
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Finish task");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = createElement("form", "course-form");
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const formData = new FormData(e.target);
        const grade = parseInt(formData.get("grade"));
   
        const taskData = {
            ...task,
            completed: true,
            gradeEarned: grade,
        };

        try {
            await updateTask(task.id, taskData);
            overlay.remove();
            const editModal = document.querySelector(".modal-overlay");
            if (editModal) editModal.remove();
            renderCalendar();
        } catch (err) {
            alert("Error completing task: " + err.message);
        }
    });
    
    const taskInfo = createElement("p", "modal-info", `Task: ${task.title}`);
    taskInfo.style.marginBottom = "20px";
    taskInfo.style.fontSize = "16px";
    
    const gradeGroup = createElement("div", "form-group");
    const gradeLabel = createElement("label", "form-label", "Ocena");
    const gradeInput = createElement("input", "form-input");
    gradeInput.type = "number";
    gradeInput.name = "grade";
    gradeInput.min = "0";
    gradeInput.max = task.gradeMax;
    gradeInput.required = true;
    gradeInput.placeholder = `0-${task.gradeMax}`;
    gradeGroup.append(gradeLabel, gradeInput);
    
    const actions = createElement("div", "form-actions");
    const submitBtn = createElement("button", "btn-submit", "Complete");
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Cancel");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => overlay.remove());
    actions.append(cancelBtn, submitBtn);
    
    form.append(taskInfo, gradeGroup, actions);
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

function getCourseColorClass(courseId) {
    const themes = ['blue', 'green', 'purple', 'orange', 'pink'];
    const index = Math.abs(String(courseId).split('').reduce((a, b) => a + b.charCodeAt(0), 0)) % themes.length;
    return `theme-${themes[index]}`;
}