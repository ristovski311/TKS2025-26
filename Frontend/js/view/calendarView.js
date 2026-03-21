// calendarView.js
import { formatDate } from '../misc/utils.js';
import { clearRoot,createElement } from '../misc/domHelpers.js';
import { logoutUser} from '../services/userService.js';
import { renderLogin } from './loginView.js';
import { renderHome } from './homeView.js';

let currentDate = new Date();

export async function renderCalendar() {
    localStorage.setItem("current_page", "calendar");

    clearRoot();
    const root = document.getElementById("root");

    // Header
    const header = createHeader();
    root.appendChild(header);

    const content = createElement("main", "main-content");
    
    const pageTitle = createElement("h1", "page-title", "Calendar");
    content.appendChild(pageTitle);

    // Calendar container
    const calendarContainer = createElement("div", "calendar-container");
    
    // Calendar header sa navigacijom
    const calendarHeader = createCalendarHeader();
    calendarContainer.appendChild(calendarHeader);
    
    // Calendar grid
    const calendarGrid = createCalendarGrid();
    calendarContainer.appendChild(calendarGrid);
    
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
        const dayCell = createElement("div", "calendar-day", String(day));
        
        // Označi današnji dan
        if (year === today.getFullYear() && 
            month === today.getMonth() && 
            day === today.getDate()) {
            dayCell.classList.add("today");
        }
        
        // Dodaj hover efekat
        dayCell.addEventListener("click", () => {
            alert(`Clicked on ${day}/${month + 1}/${year}`);
        });
        
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