import { isAuthenticated } from './services/authService.js';
import { renderHome } from './view/homeView.js';
import { renderLogin } from './view/loginView.js';

function initApp(){
    if(isAuthenticated()){
        console.log("Korisnik je vec ulogovan!")
        renderHome()
    } else {
        renderLogin()
     }
}

document.addEventListener('DOMContentLoaded', initApp);