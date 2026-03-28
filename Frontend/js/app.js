import { isAuthenticated } from './services/authService.js';
import { renderHome } from './view/homeView.js';
import { renderLogin } from './view/loginView.js';
import { renderCourses } from './view/coursesView.js';
import { renderRegister } from './view/registerView.js';
import { renderCalendar } from './view/calendarView.js';
import {renderProfessors} from './view/professorsView.js';
import {renderNotes} from './view/notesView.js'

function initApp(){
    if(isAuthenticated()){
        switch (localStorage.getItem("current_page"))
        {
            case null:
                renderHome();
                break;
            case "home":
                renderHome();
                break;
            case "login":
                renderLogin();
                break;
            case "register":
                renderRegister();
                break;
            case "courses":
                renderCourses();
                break;
            case "notes":
                renderNotes();
                break;
            case "calendar":
                renderCalendar();
                break;
            case "professors":
                renderProfessors();
                break;
            default:
                renderLogin();
        }
    } else {
        renderLogin()
     }
}

document.addEventListener('DOMContentLoaded', initApp);