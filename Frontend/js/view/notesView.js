import { createElement, clearRoot, createNavBar, createHeader, showSkeletons, handleAuthError, showLoadingOverlay } from '../misc/domHelpers.js';
import { getNotesByUserId, createNote, updateNote } from '../services/noteService.js';
import { getCourses } from '../services/courseService.js';
import { getCurrentUser } from '../services/userService.js';
import { formatDate } from '../misc/utils.js';

// Stanje stranice
let currentFolderId = null;
let allNotes = [];
let allCourses = [];

//TODO: dodati userID kod tasks i notes

export async function renderNotes() {
    localStorage.setItem("current_page", "notes");

    clearRoot();
    const root = document.getElementById("root");

    // Header i Navigacija
    const header = createHeader();
    const nav = createNavBar();    
    root.append(header, nav);

    const content = createElement("main", "main-content");
    
    // Naslov stranice
    const pageTitle = createElement("h1", "page-title", "My Notes");
    content.appendChild(pageTitle);

    // Split-view container
    const notesSplitView = createElement("div", "notes-splitview");
    
    // Sidebar za foldere
    const sidebar = createElement("aside", "notes-sidebar");
    const folderListTitle = createElement("h2", "sidebar-subtitle", "Folders");
    sidebar.appendChild(folderListTitle);

    const newFolderBtn = createElement("button", "btn-submit", "+ New Folder");
    newFolderBtn.addEventListener("click", () => openNoteFormModal(true));
    sidebar.appendChild(newFolderBtn)
    
    const folderList = createElement("div", "folder-list");
    sidebar.appendChild(folderList);
    
    const mainPanel = createElement("section", "notes-main-panel");
    const notesGrid = createElement("div", "notes-grid");
    mainPanel.appendChild(notesGrid);

    notesSplitView.append(sidebar, mainPanel);
    content.appendChild(notesSplitView);

    const fab = createElement("button", "fab", "+ New Note");
    fab.addEventListener("click", () => openNoteFormModal(false));
    root.appendChild(fab);

    root.appendChild(content);

    // Učitavanje podataka
    showSkeletons(notesGrid);
    await loadData();
    
    // Inicijalni render
    renderFolderList(folderList);
    renderNotesGrid(notesGrid);
}

async function loadData() {
    try {
        const currentUser = await getCurrentUser();

        const [notesData, coursesData] = await Promise.all([getNotesByUserId(currentUser.id), getCourses(currentUser.id)]);
        allNotes = notesData;
        allCourses = coursesData;
    } catch (error) {
        if(handleAuthError(error)) return [];
        console.error("Error fetching data:", error);
    }
}

function renderFolderList(container) {
    container.innerHTML = "";

    const rootItem = createFolderItem({ id: null, title: "All Notes" }, true);
    container.appendChild(rootItem);

    const folders = allNotes.filter(note => note.type === 'folder');

    renderFolderHierarchy(folders, null, container, 1);
}

function renderFolderHierarchy(allFolders, parentId, container, level) {
    const children = allFolders.filter(f => f.parentId === parentId);
    
    children.forEach(folder => {
        const folderItem = createFolderItem(folder, false, level);
        container.appendChild(folderItem);
        
        renderFolderHierarchy(allFolders, folder.id, container, level + 1);
    });
}

function createFolderItem(folder, isRoot = false, level = 0) {
    const item = createElement("div", "folder-item");
    if (folder.id === currentFolderId) {
        item.classList.add("active");
    }
    
    item.style.paddingLeft = `${level * 15 + 10}px`;

    const icon = createElement("span", "folder-icon", "📁");
    const title = createElement("span", "folder-title", folder.title);
    
    if (folder.id !== null) {
        const count = allNotes.filter(n => n.parentId === folder.id && n.type !== 'folder').length;
        const countSpan = createElement("span", "folder-count", String(count));
        item.append(icon, title, countSpan);
    } else {
        item.append(icon, title);
    }

    item.addEventListener("click", () => {
        currentFolderId = folder.id;
        
        renderFolderList(document.querySelector(".folder-list"));
        renderNotesGrid(document.querySelector(".notes-grid"));
    });

    return item;
}

function renderNotesGrid(container) {
    container.innerHTML = "";

    let notesToDisplay = allNotes.filter(n => n.parentId === currentFolderId && n.type !== 'folder');
    
    if (currentFolderId === null) {
        notesToDisplay = allNotes.filter(n => n.type !== 'folder');
    }

    if (notesToDisplay.length === 0) {
        const emptyMsg = createElement("p", "empty-message", "Nema beleški u ovom folderu.");
        container.appendChild(emptyMsg);
        return;
    }

    notesToDisplay.sort((a, b) => new Date(b.lastUpdated) - new Date(a.lastUpdated));

    notesToDisplay.forEach(note => {
        const card = createNoteCard(note);
        container.appendChild(card);
    });
}

function createNoteCard(note) {
    const card = createElement("div", "note-card");

    const cardHeader = createElement("div", "note-card-header");
    
    if (note.courseId) {
        const course = allCourses.find(c => c.id === note.courseId);
        if (course) {
            const courseTag = createElement("span", "course-tag", course.title);
            courseTag.classList.add(getCourseColorClass(note.courseId));
            cardHeader.appendChild(courseTag);
        }
    }
    
    const dateSpan = createElement("span", "note-date", formatDate(new Date(note.lastUpdated)));
    cardHeader.appendChild(dateSpan);

    const title = createElement("h3", "note-title", note.title);
    const description = createElement("p", "note-description", note.description);
    
    card.append(cardHeader, title, description);
    
    card.addEventListener("click", () => openNoteFormModal(false, note));
    card.style.cursor = "pointer";

    return card;
}

async function openNoteFormModal(isFolderMode = false, noteToEdit = null) {
    const overlay = createElement("div", "modal-overlay");
    const modalContent = createElement("div", "modal-content");
    
    const header = createElement("div", "modal-header");
    let modalTitleText = isFolderMode ? "New Folder" : "New Note";
    if (noteToEdit) {
        modalTitleText = isFolderMode ? "Folder" : "Note";
    }
    const modalTitle = createElement("h2", "modal-title", modalTitleText);
    const closeBtn = createElement("button", "modal-close", "×");
    closeBtn.addEventListener("click", () => overlay.remove());
    header.append(modalTitle, closeBtn);
    
    const form = await createNoteForm(isFolderMode, noteToEdit);
    
    modalContent.append(header, form);
    overlay.appendChild(modalContent);
    
    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) overlay.remove();
    });
    
    document.body.appendChild(overlay);
}

async function createNoteForm(isFolderMode, note = null) {
    const form = createElement("form", "course-form");
    form.addEventListener("submit", (e) => handleNoteSubmit(e, isFolderMode, note));

    const typeInput = createElement("input");
    typeInput.type = "hidden";
    typeInput.name = "type";
    typeInput.value = isFolderMode ? "folder" : "note";
    form.appendChild(typeInput);

    const parentIdInput = createElement("input");
    parentIdInput.type = "hidden";
    parentIdInput.name = "parentId";
    parentIdInput.value = currentFolderId;
    form.appendChild(parentIdInput);

    // Naslov
    const titleGroup = createElement("div", "form-group");
    const titleLabel = createElement("label", "form-label", isFolderMode ? "Folder Title" : "Note Title");
    const titleInput = createElement("input", "form-input");
    titleInput.type = "text";
    titleInput.name = "title";
    titleInput.required = true;
    if (note) titleInput.value = note.title;
    titleGroup.append(titleLabel, titleInput);
    form.appendChild(titleGroup);

    if (!isFolderMode) {
        // Opis
        const descGroup = createElement("div", "form-group");
        const descLabel = createElement("label", "form-label", "Description");
        const descInput = createElement("input", "form-input");
        descInput.type = "text";
        descInput.name = "description";
        if (note) descInput.value = note.description;
        descGroup.append(descLabel, descInput);

        // Sadrzaj
        const contentGroup = createElement("div", "form-group");
        const contentLabel = createElement("label", "form-label", "Content");
        const contentTextarea = createElement("textarea", "form-textarea");
        contentTextarea.name = "content";
        contentTextarea.rows = 10;
        if (note) contentTextarea.value = note.content;
        contentGroup.append(contentLabel, contentTextarea);

        // Kurs
        const courseGroup = createElement("div", "form-group");
        const courseLabel = createElement("label", "form-label", "Course");
        const courseSelect = createElement("select", "form-select");
        courseSelect.name = "courseId";

        // Add actual courses only
        allCourses.forEach(course => {
            const option = createElement("option", "", course.title);
            option.value = course.id;

            // Select the course if note is connected
            if (note && course.id === note.courseId) {
                option.selected = true;
            }

            courseSelect.appendChild(option);
        });

        courseGroup.append(courseLabel, courseSelect);

        form.append(descGroup, contentGroup, courseGroup);
    }
    
    // Dugmici
    const actions = createElement("div", "form-actions");
    const submitText = note ? "Save changes" : (isFolderMode ? "Create Folder" : "Create Note"); 
    const submitBtn = createElement("button", "btn-submit", submitText);
    submitBtn.type = "submit";
    const cancelBtn = createElement("button", "btn-cancel", "Cancel");
    cancelBtn.type = "button";
    cancelBtn.addEventListener("click", () => {
        const overlay = document.querySelector(".modal-overlay");
        if (overlay) {
            overlay.remove();
        } else {
            renderNotes();
        }
    });
    actions.append(cancelBtn, submitBtn);
    
    form.appendChild(actions);
    
    return form;
}

async function handleNoteSubmit(e, isFolderMode, existingNote) {
    e.preventDefault();
    
    const hideOverlay = showLoadingOverlay(); 

    const formData = new FormData(e.target);
    const now = new Date().toISOString();

    let pId = formData.get("parentId");

    pId = (pId === "" || pId === null)
    ? null
    : parseInt(pId);

    

    // Osnovni podaci
    const noteData = {
        title: formData.get("title"),
        type: formData.get("type"),
        parentId: pId,
        lastUpdated: now
    };

    if (!isFolderMode) {
        noteData.description = formData.get("description");
        noteData.content = formData.get("content");
        let courseId = formData.get("courseId");
        courseId = (courseId === "" || courseId === null)
            ? null
            : parseInt(courseId)
        console.log(courseId);
        noteData.courseId = courseId;
    } else {
        noteData.description = "";
        noteData.content = "";
        noteData.courseId = null;
    }
    
    try {
        if (existingNote) {
            await updateNote(existingNote.id, { ...existingNote, ...noteData, courseId: noteData.courseId });
        } else {
            noteData.createdAt = now;
            await createNote(noteData);
        }
        const overlay = document.querySelector(".modal-overlay");
        if (overlay) overlay.remove();
        
        hideOverlay();
        renderNotes();
    } catch (err) {
        alert("Greska pri cuvanju: " + err.message);
    }
}

function getCourseColorClass(courseId) {
    const themes = ['blue', 'green', 'purple', 'orange', 'pink'];
    const index = Math.abs(String(courseId).split('').reduce((a, b) => a + b.charCodeAt(0), 0)) % themes.length;
    return `theme-${themes[index]}`;
}