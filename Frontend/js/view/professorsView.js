import { createElement, clearRoot, createSkeletonCard, createNavBar } from '../misc/domHelpers.js';
import { getCurrentUser } from '../services/userService.js';
import { getProfessorById, getProfessorsByUserId } from '../services/professorService.js';
import { getCourses, createCourse, updateCourse } from '../services/courseService.js'
import { createHeader } from '../misc/domHelpers.js'

export async function renderProfessors()
{
        localStorage.setItem("current_page", "professors");
    
        clearRoot();
        const root = document.getElementById("root");
    
        const header = createHeader();
        const nav = createNavBar();    
        root.append(header, nav);
    
        const content = createElement("main", "main-content");
        
        const pageTitle = createElement("h1", "page-title", "My Professors");
        content.appendChild(pageTitle);
    
        const coursesGrid = createElement("div", "courses-grid");
        content.appendChild(coursesGrid);
    
        root.appendChild(content);
    
        // Floating action button za dodavanje profesora
        const fab = createElement("button", "fab", "+ Add Professor");
        fab.addEventListener("click", openAddProfessorModal);
        root.appendChild(fab);
    
        const courses = await loadCourses(coursesGrid);
    
        sortBySelect.dispatchEvent(new Event("change"));
}

//---

async function openAddProfessorModal() {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Add New Professor");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createProfessorForm();
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function createProfessorForm(professor = null) {
    const form = createElement("form", "course-form");
    form.addEventListener("submit", (e) => handleCourseSubmit(e, course));
    
    //Ime
    const nameGroup = createElement("div", "form-group")
    const nameRow = createElement("div", "form-row");
    const nameLabel = createElement("label", "form-label", "Full name");
    const firstNameInput = createElement("input", "form-input");
    firstNameInput.type = "text";
    firstNameInput.name = "firstName";
    firstNameInput.required = true;
    firstNameInput.placeholder = "First name";
    const lastNameInput = createElement("input", "form-input");
    lastNameInput.type = "text";
    lastNameInput.name = "lastName";
    lastNameInput.required = true;
    lastNameInput.placeholder = "Last name";
    if(professor != null)
    {
        firstNameInput.value = professor.firstName;
        lastNameInput.value = professor.lastName;
    }
    nameRow.append(firstNameInput, lastNameInput);
    nameGroup.append(nameLabel, nameRow);

    //Email
    const emailGroup = createElement("div", "form-group")
    const emailLabel = createElement("label", "form-label", "Email");
    const emailInput = createElement("input", "form-input");
    emailInput.type = "email";
    emailInput.name = "email";
    emailInput.required = true;
    emailInput.placeholder = "email@mail.com";
    if(professor != null)
    {
        emailInput.value = professor.email;
    }
    emailGroup.append(emailLabel, emailInput);

    //Telefon
    const phoneGroup = createElement("div", "form-group")
    const phoneLabel = createElement("label", "form-label", "Phone");
    const phoneInput = createElement("input", "form-input");
    phoneInput.type = "text";
    phoneInput.name = "phone";
    phoneInput.required = true;
    phoneInput.placeholder = "06123456789";
    if(professor != null)
    {
        phoneInput.value = professor.phone;
    }
    phoneGroup.append(phoneLabel, phoneInput);

    //Kabinet
    const officeGroup = createElement("div", "form-group")
    const officeLabel = createElement("label", "form-label", "Office");
    const officeInput = createElement("input", "form-input");
    officeInput.type = "text";
    officeInput.name = "office";
    officeInput.required = true;
    officeInput.placeholder = "331";
    if(professor != null)
    {
        officeInput.value = professor.office;
    }
    officeGroup.append(officeLabel, officeInput);
    
    //Dugmici
    const actions = createElement("div", "form-actions");
    const submitText = professor ? "Save changes" : "Add professor"; 
    const submitBtn = createElement("button", "btn-submit", submitText);
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Cancel");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => {
        document.querySelector(".modal-overlay").remove();
    });
    actions.append(cancelBtn, submitBtn);
    
    form.append(nameGroup, emailGroup, phoneGroup, officeGroup, actions);
    
    return form;
}