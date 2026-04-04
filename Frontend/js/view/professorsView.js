import { createElement, clearRoot, showSkeletons, createNavBar, createConfirmModal, showLoadingOverlay } from '../misc/domHelpers.js';
import { getCurrentUser } from '../services/userService.js';
import { getProfessorsByUserId, addProfessor, deleteProfessor, updateProfessor } from '../services/professorService.js';
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
    
        const professorsGrid = createElement("div", "courses-grid");
        content.appendChild(professorsGrid);
    
        root.appendChild(content);
    
        // Floating action button za dodavanje profesora
        const fab = createElement("button", "fab", "+ Add Professor");
        fab.addEventListener("click", openAddProfessorModal);
        root.appendChild(fab);
    
        showSkeletons(professorsGrid);

        const professors = await loadProfessors(professorsGrid);
}

//---

async function loadProfessors(container) {
    try {
        const currentUser = await getCurrentUser();
        const professors = await getProfessorsByUserId(currentUser.id);

        if (!professors || professors.length === 0) {
            container.innerHTML = "";
            const emptyMsg = createElement("p", "empty-message", "No professors.");
            container.appendChild(emptyMsg);
            return [];
        }

        await renderProfessorCards(container, professors);
        return professors;

    } catch (error) {
        if (handleAuthError(error)) return [];
        container.innerHTML = "";
        console.error("Failed to load professors:", error);
        const errorMsg = createElement("p", "error-message", "Error fetching the professors.");
        container.appendChild(errorMsg);
        return [];
    }
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
    form.addEventListener("submit", (e) => {
        handleProfessorSubmit(e, professor)
    });
    
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
        emailInput.value = professor.mail;
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

//---

async function renderProfessorCards(container, professors) {
    const cards = await Promise.all(professors.map(prof => createProfessorCard(prof)));
    container.innerHTML = "";
    cards.forEach(card => container.appendChild(card));
}

function createProfessorCard(professor) {
    const card = createElement("div", "course-card");

    //Ponovo koristimo iste klase za profesorske kartice kao za kurseve
    const nameWrapper = createElement("div", "course-title-wrapper");

    const name = createElement("h3", "course-title", `Prof. ${professor.firstName} ${professor.lastName}`);
    
    const editBtn = createElement("button", "course-action-btn", "✏️");
    const deleteBtn = createElement("button", "course-action-btn", "❌");

    editBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        console.log("Edit professor data.");
        openEditProfessorModal(course);
    });

    deleteBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        console.log("Remove professor.");
        createConfirmModal("Are you sure you want to remove this professor?", 
            async () => {
                const hideOverlay = showLoadingOverlay();

                try{
                    await deleteProfessor(professor.id);
                    renderProfessors();
                }
                finally{
                    hideOverlay();
                }
            }, null, "NOTE: If you remove this professor, every course connected to it will also be deleted!"
        )
    });

    nameWrapper.append(name, editBtn, deleteBtn);
    
    const email = createElement("p", "course-professor", `Mail: ${professor.mail}`);
    const phone = createElement("p", "course-semester", `Phone: ${professor.phone}`);
    const office = createElement("p", "course-description", `Office: ${professor.office}`);

    card.append(nameWrapper, email, phone, office);
    card.addEventListener("click", () => openEditProfessorModal(professor));
    card.style.cursor = "pointer";

    return card;
}

//---

async function handleProfessorRemove(profId)
{
    createConfirmModal(
        "Are you sure you want to remove this professor?",
        async () => {
            const hideOverlay = showLoadingOverlay();
            try{
                await deleteProfessor(profId);
            }
            finally
            {
                hideOverlay();
            }
            clearRoot();
            renderProfessors();
        },
        null,
        "NOTE: This will remove every course linked to this professor, same for the notes and tasks. We recommend changing the professor of the course first."
    )
}

async function handleProfessorSubmit(e, professor) {
    e.preventDefault();

    const hideOverlay = showLoadingOverlay();
    const currentUser = await getCurrentUser();
    const formData = new FormData(e.target);
    const professorData = {
        firstName: formData.get("firstName"),
        lastName: formData.get("lastName"),
        mail: formData.get("email"),
        phone: formData.get("phone"),
        office: formData.get("office"),
        userId: currentUser.id
    };

    try {
        if (professor)
            await updateProfessor(professor.id, professorData);
        else
            await addProfessor(professorData);
        document.querySelector(".modal-overlay").remove();
        hideOverlay();
        renderProfessors();
    } catch (err) {
        alert("Error saving professor: " + err.message);
    }
}

//---

async function openEditProfessorModal(professor) {
    const overlay = createElement("div", "modal-overlay");
    
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    const modalTitle = createElement("h2", "modal-title", "Edit Professor Data");
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createProfessorForm(professor);

    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}