import { isAuthenticated } from './services/authService.js';
import { renderHome } from './view/homeView.js';
import { renderLogin } from './view/loginView.js';
import { renderCourses } from './view/coursesView.js';
import { renderRegister } from './view/registerView.js';
import { renderCalendar } from './view/calendarView.js';

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
                renderHome();
                break;
            case "calendar":
                renderCalendar();
                break;
            default:
                renderLogin();
        }
    } else {
        renderLogin()
     }
}

document.addEventListener('DOMContentLoaded', initApp);