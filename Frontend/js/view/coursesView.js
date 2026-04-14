//     import { createElement, clearRoot, showSkeletons, createNavBar, createConfirmModal, showLoadingOverlay, createLoader, handleAuthError } from '../misc/domHelpers.js';
// import { getCurrentUser } from '../services/userService.js';
// import { getProfessorById, getProfessorsByUserId } from '../services/professorService.js';
// import { getCourses, createCourse, updateCourse, deleteCourse } from '../services/courseService.js'
// import { createHeader } from '../misc/domHelpers.js'

// export async function renderCourses() {
//     localStorage.setItem("current_page", "courses");

//     clearRoot();
//     const root = document.getElementById("root");

//     const header = createHeader();
//     const nav = createNavBar();    
//     root.append(header, nav);

//     const content = createElement("main", "main-content");
    
//     const pageTitle = createElement("h1", "page-title", "My Courses");
//     content.appendChild(pageTitle);

//     const sortRow = createElement("div", "sort-row");

//     const sortBySelect = createElement("select", "form-select");
//     sortBySelect.innerHTML = `
//         <option value="title">Name</option>
//         <option value="grade">Grade</option>
//         <option value="semester">Semester</option>
//         <option value="professor">Professor</option>
//     `;

//     const orderSelect = createElement("select", "form-select");
//     orderSelect.innerHTML = `
//         <option value="asc">Ascending</option>
//         <option value="desc">Descending</option>
//     `;

//     const savedSortBy = localStorage.getItem("sortBy") || "title";
//     const savedOrder = localStorage.getItem("order") || "asc";

//     sortBySelect.value = savedSortBy;
//     orderSelect.value = savedOrder;

//     sortRow.append(sortBySelect, orderSelect);
//     content.appendChild(sortRow);

//     const coursesGrid = createElement("div", "courses-grid");
//     content.appendChild(coursesGrid);

//     root.appendChild(content);

//     const fab = createElement("button", "fab", "+ Add Course");
//     fab.addEventListener("click", openAddCourseModal);
//     root.appendChild(fab);

//     showSkeletons(coursesGrid);

//     const renderSorted = setupSorting(coursesGrid, sortBySelect, orderSelect);
//     await renderSorted();
// }

// async function loadCourses(container) {
//     try {
//         const currentUser = await getCurrentUser();
//         const courses = await getCourses(currentUser.id);

//         if (!courses || courses.length === 0) {
//             container.innerHTML = "";
//             const emptyMsg = createElement("p", "empty-message", "No courses.");
//             container.appendChild(emptyMsg);
//             return [];
//         }

//         return courses;

//     } catch (error) {
//         if (handleAuthError(error)) return [];
//         container.innerHTML = "";
//         console.error("Failed to load courses:", error);
//         const errorMsg = createElement("p", "error-message", "Error fetching the courses.");
//         container.appendChild(errorMsg);
//         return [];
//     }
// }

// function setupSorting(container, sortBySelect, orderSelect) {
//     async function renderSorted() {
//         const sortBy = sortBySelect.value;
//         const order = orderSelect.value;

//         localStorage.setItem("sortBy", sortBy);
//         localStorage.setItem("order", order);

//         showSkeletons(container);

//         const courses = await loadCourses(container);
//         if (!courses.length) return;

//         const sorted = [...courses].sort((a, b) => {
//             let valA, valB;

//             switch (sortBy) {
//                 case "title":
//                     valA = a.title.toLowerCase();
//                     valB = b.title.toLowerCase();
//                     break;
//                 case "semester":
//                     valA = a.semester;
//                     valB = b.semester;
//                     break;
//                 case "grade":
//                     if (a.grade == null) return 1;
//                     if (b.grade == null) return -1;
//                     valA = a.grade;
//                     valB = b.grade;
//                     break;
//                 case "professor":
//                     valA = a.professorId;
//                     valB = b.professorId;
//                     break;
//             }

//             if (valA < valB) return order === "asc" ? -1 : 1;
//             if (valA > valB) return order === "asc" ? 1 : -1;
//             return 0;
//         });

//         const cards = await Promise.all(
//             sorted.map(course => createCourseCard(course))
//         );

//         container.innerHTML = "";
//         cards.forEach(card => container.appendChild(card));
//     }

//     sortBySelect.addEventListener("change", renderSorted);
//     orderSelect.addEventListener("change", renderSorted);

//     return renderSorted;
// }

// async function createCourseCard(course) {
//     const card = createElement("div", "course-card");

//     const titleWrapper = createElement("div", "course-title-wrapper");

//     const title = createElement("h3", "course-title", course.title);
    
//     const editBtn = createElement("button", "course-action-btn", "✏️");
//     const deleteBtn = createElement("button", "course-action-btn", "❌");

//     editBtn.addEventListener("click", (e) => {
//         e.stopPropagation();
//         openEditCourseModal(course);
//     });

//     deleteBtn.addEventListener("click", (e) => {
//         e.stopPropagation();
//         createConfirmModal("Are you sure you want to delete this course?", 
//             async () => {
//                 const hideOverlay = showLoadingOverlay();
//                 try{
//                     await deleteCourse(course.id);
//                     renderCourses();
//                 }
//                 finally{
//                     hideOverlay();
//                 }
//             }, null, "NOTE: If you delete this course, every task and note connected to it will also be deleted!"
//         )
//     });

//     titleWrapper.append(title, editBtn, deleteBtn);
    
//     let professorText = `Prof. ${course.professorId}`;
//     try {
//         const professor = await getProfessorById(course.professorId);
//         professorText = `Prof. ${professor.firstName} ${professor.lastName} | ${professor.office}`;
//     } catch {}

//     const professor = createElement("p", "course-professor", professorText);
//     const semester = createElement("p", "course-semester", `Semester: ${course.semester}`);
//     const description = createElement("p", "course-description", course.description);
    
//     const footer = createElement("div", "course-footer");

//     if (course.grade) {
//         const grade = createElement("span", "course-grade", `Grade: ${course.grade}`);
//         footer.appendChild(grade);
//     } else {
//         const passBtn = createElement("button", "btn-pass", "Pass the course");
//         passBtn.addEventListener("click", (e) => {
//             e.stopPropagation();
//             openPassExamModal(course);
//         });
//         footer.appendChild(passBtn);
//     }

//     card.append(titleWrapper, professor, semester, description, footer);

//     card.addEventListener("click", () => openEditCourseModal(course));
//     card.style.cursor = "pointer";

//     return card;
// }

// async function openAddCourseModal() {
//     const overlay = createElement("div", "modal-overlay");
    
//     const modalContent = createElement("div", "modal-content");
    
//     const header = createElement("div", "modal-header");
//     const modalTitle = createElement("h2", "modal-title", "Add New Course");
//     const closeBtn = createElement("button", "modal-close", "×");
//     closeBtn.addEventListener("click", () => overlay.remove());
//     header.append(modalTitle, closeBtn);
    
//     const form = createCourseForm();
    
//     modalContent.append(header, form);
//     overlay.appendChild(modalContent);
    
//     overlay.addEventListener("click", (e) => {
//         if (e.target === overlay) overlay.remove();
//     });
    
//     document.body.appendChild(overlay);
// }

// function createCourseForm(course = null) {
//     const form = createElement("form", "course-form");
//     form.addEventListener("submit", (e) => handleCourseSubmit(e, course));
    
//     const titleGroup = createElement("div", "form-group");
//     const titleLabel = createElement("label", "form-label", "Course Title");
//     const titleInput = createElement("input", "form-input");
//     titleInput.type = "text";
//     titleInput.name = "title";
//     titleInput.required = true;
//     if(course) titleInput.value = course.title;
//     titleGroup.append(titleLabel, titleInput);
    
//     const rowGroup = createElement("div", "form-row");
    
//     const semesterGroup = createElement("div", "form-group");
//     const semesterLabel = createElement("label", "form-label", "Semester");
//     const semesterInput = createElement("input", "form-input");
//     semesterInput.type = "number";
//     semesterInput.name = "semester";
//     semesterInput.min = "1";
//     semesterInput.max = "12";
//     semesterInput.required = true;
//     if(course) semesterInput.value = course.semester;
//     semesterGroup.append(semesterLabel, semesterInput);
    
//     rowGroup.append(semesterGroup);
    
//     const descGroup = createElement("div", "form-group");
//     const descLabel = createElement("label", "form-label", "Description");
//     const descTextarea = createElement("textarea", "form-textarea");
//     descTextarea.name = "description";
//     descTextarea.required = true;
//     if(course) descTextarea.value = course.description;
//     descGroup.append(descLabel, descTextarea);
    
//     const actions = createElement("div", "form-actions");
//     const submitText = course ? "Save changes" : "Create course"; 
//     const submitBtn = createElement("button", "btn-submit", submitText);
//     submitBtn.type = "submit";
//     const cancelBtn = createElement("button", "btn-cancel", "Cancel");
//     cancelBtn.type = "button";
//     cancelBtn.addEventListener("click", () => {
//         document.querySelector(".modal-overlay").remove();
//     });
//     actions.append(cancelBtn, submitBtn);
    
//     const professorGroup = createElement("div", "form-group");
//     const professorLabel = createElement("label", "form-label", "Professor");

//     const selectWrapper = createElement("div", "select-wrapper");
//     const loaderContainer = createElement("div", "select-loader");
//     loaderContainer.appendChild(createLoader());
//     selectWrapper.appendChild(loaderContainer);

//     const professorSelect = createElement("select", "form-select");
//     professorSelect.name = "professorId";
//     professorSelect.required = true;
//     professorSelect.style.display = "none";

//     const defaultOption = createElement("option", "", "Select a professor");
//     defaultOption.value = "";
//     defaultOption.disabled = true;
//     defaultOption.selected = true;
//     professorSelect.appendChild(defaultOption);

//     selectWrapper.appendChild(professorSelect);
//     professorGroup.append(professorLabel, selectWrapper);

//     form.append(titleGroup, rowGroup, professorGroup, descGroup, actions);

//     getCurrentUser()
//         .then(user => getProfessorsByUserId(user.id))
//         .then(professors => {
//             professors.forEach(prof => {
//                 const option = createElement("option", "", `${prof.firstName} ${prof.lastName} - ${prof.office}`);
//                 option.value = prof.id;
//                 if (course && prof.id === course.professorId) option.selected = true;
//                 professorSelect.appendChild(option);
//             });
//         })
//         .finally(() => {
//             loaderContainer.remove();
//             professorSelect.style.display = "";
//         });

//     return form;
// }

// async function handleCourseSubmit(e, course) {
//     e.preventDefault();
    
//     const hideOverlay = showLoadingOverlay();
//     const currentUser = await getCurrentUser();
//     const formData = new FormData(e.target);
//     const courseData = {
//         title: formData.get("title"),
//         semester: parseInt(formData.get("semester")),
//         description: formData.get("description"),
//         grade: course ? course.grade : null,
//         userId: currentUser.id,
//         professorId: parseInt(formData.get("professorId"))
//     };
    
//     try {
//         if(course) await updateCourse(course.id, courseData);
//         else await createCourse(courseData);
//         document.querySelector(".modal-overlay").remove();
//         hideOverlay();
//         renderCourses();
//     } catch (err) {
//         alert("Error creating the course: " + err.message);
//     }
// }

// async function openEditCourseModal(course) {
//     const overlay = createElement("div", "modal-overlay");
    
//     const modalContent = createElement("div", "modal-content");
    
//     const header = createElement("div", "modal-header");
//     const modalTitle = createElement("h2", "modal-title", "Edit Course");
//     const closeBtn = createElement("button", "modal-close", "×");
//     closeBtn.addEventListener("click", () => overlay.remove());
//     header.append(modalTitle, closeBtn);
    
//     const form = createCourseForm(course);
    
//     modalContent.append(header, form);
//     overlay.appendChild(modalContent);
    
//     overlay.addEventListener("click", (e) => {
//         if (e.target === overlay) overlay.remove();
//     });
    
//     document.body.appendChild(overlay);
// }

// async function openPassExamModal(course) {
//     const overlay = createElement("div", "modal-overlay");
    
//     const modalContent = createElement("div", "modal-content");
//     modalContent.style.maxWidth = "400px";
    
//     const header = createElement("div", "modal-header");
//     const modalTitle = createElement("h2", "modal-title", "Pass the course");
//     const closeBtn = createElement("button", "modal-close", "×");
//     closeBtn.addEventListener("click", () => overlay.remove());
//     header.append(modalTitle, closeBtn);
    
//     const form = createElement("form", "course-form");
//     form.addEventListener("submit", async (e) => {
//         e.preventDefault();

//         const hideOverlay = showLoadingOverlay();

//         const formData = new FormData(e.target);
//         const grade = parseInt(formData.get("grade"));

//         const currentUser = await getCurrentUser();
//         const courseData = {
//             title: course.title,
//             semester: course.semester,
//             description: course.description,
//             grade: grade,
//             userId: currentUser.id,
//             professorId: course.professorId
//         };

//         try {
//             await updateCourse(course.id,courseData);
//             overlay.remove();
//             hideOverlay();
//             renderCourses();
//         } catch (err) {
//             alert("Greška pri polaganju ispita: " + err.message);
//         }
//     });
    
//     const courseInfo = createElement("p", "modal-info", `Course: ${course.title}`);
    
//     const gradeGroup = createElement("div", "form-group");
//     const gradeLabel = createElement("label", "form-label", "Grade");
//     const gradeInput = createElement("input", "form-input");
//     gradeInput.type = "number";
//     gradeInput.name = "grade";
//     gradeInput.min = "6";
//     gradeInput.max = "10";
//     gradeInput.required = true;
//     gradeInput.placeholder = "6-10";
//     gradeGroup.append(gradeLabel, gradeInput);
    
//     const actions = createElement("div", "form-actions");
//     const submitBtn = createElement("button", "btn-submit", "Pass the course");
//     submitBtn.type = "submit";
//     const cancelBtn = createElement("button", "btn-cancel", "Cancel");
//     cancelBtn.type = "button";
//     cancelBtn.addEventListener("click", () => overlay.remove());
//     actions.append(cancelBtn, submitBtn);
    
//     form.append(courseInfo, gradeGroup, actions);
//     modalContent.append(header, form);
//     overlay.appendChild(modalContent);
    
//     overlay.addEventListener("click", (e) => {
//         if (e.target === overlay) overlay.remove();
//     });
    
//     document.body.appendChild(overlay);
// }
    
    import { createElement, clearRoot, showSkeletons, createNavBar, createConfirmModal,showLoadingOverlay, createLoader, handleAuthError } from '../misc/domHelpers.js';
    import { getCurrentUser } from '../services/userService.js';
    import { getProfessorById, getProfessorsByUserId } from '../services/professorService.js';
    import { getCourses, createCourse, updateCourse, deleteCourse } from '../services/courseService.js'
    import { createHeader } from '../misc/domHelpers.js'

    export async function renderCourses() {
        localStorage.setItem("current_page", "courses");

        clearRoot();
        const root = document.getElementById("root");

        const header = createHeader();
        const nav = createNavBar();    
        root.append(header, nav);

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

        showSkeletons(coursesGrid);

        const courses = await loadCourses(coursesGrid);

        if(courses.length != 0)
        {
            
            sortBySelect.value = "title";
            orderSelect.value = "asc";
            
            setupSorting(courses, coursesGrid, sortBySelect, orderSelect);
    
            sortBySelect.dispatchEvent(new Event("change"));

        }
    }

    async function loadCourses(container) {
        try {
            const currentUser = await getCurrentUser();
            const courses = await getCourses(currentUser.id);

            if (!courses || courses.length === 0) {
                container.innerHTML = "";
                const emptyMsg = createElement("p", "empty-message", "No courses.");
                container.appendChild(emptyMsg);
                return [];
            }

            return courses;

        } catch (error) {
            if (handleAuthError(error)) return [];
            container.innerHTML = "";
            console.error("Failed to load courses:", error);
            const errorMsg = createElement("p", "error-message", "Error fetching the courses.");
            container.appendChild(errorMsg);
            return [];
        }
    }

    function setupSorting(courses, container, sortBySelect, orderSelect) {
        async function renderSorted() 
        {
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

        const titleWrapper = createElement("div", "course-title-wrapper");

        const title = createElement("h3", "course-title", course.title);
        
        const editBtn = createElement("button", "course-action-btn", "✏️");
        const deleteBtn = createElement("button", "course-action-btn", "❌");
    
        editBtn.addEventListener("click", (e) => {
            e.stopPropagation();
            console.log("Edit course.");
            openEditCourseModal(course);
        });
    
        deleteBtn.addEventListener("click", (e) => {
            e.stopPropagation();
            console.log("Delete course.");
            createConfirmModal("Are you sure you want to delete this course?", 
                async () => {
                    const hideOverlay = showLoadingOverlay();
    
                    try{
                        await deleteCourse(course.id);
                        renderCourses();
                    }
                    finally{
                        hideOverlay();
                    }
                }, null, "NOTE: If you delete this course, every task and note connected to it will also be deleted!"
            )
        });

        titleWrapper.append(title, editBtn, deleteBtn);
        
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
            const grade = createElement("span", "course-grade", `Grade: ${course.grade}`);
            footer.appendChild(grade);
        } else {
            const passBtn = createElement("button", "btn-pass", "Pass the course");
            passBtn.addEventListener("click", (e) => {
                e.stopPropagation();
                openPassExamModal(course);
            });
            footer.appendChild(passBtn);
        }

        card.append(titleWrapper, professor, semester, description, footer);

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
        
        const form = createCourseForm();
        
        modalContent.append(header, form);
        overlay.appendChild(modalContent);
        
        overlay.addEventListener("click", (e) => {
            if (e.target === overlay) overlay.remove();
        });
        
        document.body.appendChild(overlay);
    }

    function createCourseForm(course = null) {
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
        
        rowGroup.append(semesterGroup);
        
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
        
        // Profesori
        const professorGroup = createElement("div", "form-group");
        const professorLabel = createElement("label", "form-label", "Professor");

        const selectWrapper = createElement("div", "select-wrapper");
        const loaderContainer = createElement("div", "select-loader");
        loaderContainer.appendChild(createLoader());
        selectWrapper.appendChild(loaderContainer);

        const professorSelect = createElement("select", "form-select");
        professorSelect.name = "professorId";
        professorSelect.required = true;
        professorSelect.style.display = "none";

        const defaultOption = createElement("option", "", "Select a professor");
        defaultOption.value = "";
        defaultOption.disabled = true;
        defaultOption.selected = true;
        professorSelect.appendChild(defaultOption);

        selectWrapper.appendChild(professorSelect);
        professorGroup.append(professorLabel, selectWrapper);

        form.append(titleGroup, rowGroup, professorGroup, descGroup, actions);

        getCurrentUser()
            .then(user => getProfessorsByUserId(user.id))
            .then(professors => {
                professors.forEach(prof => {
                    const option = createElement("option", "", `${prof.firstName} ${prof.lastName} - ${prof.office}`);
                    option.value = prof.id;
                    if (course && prof.id === course.professorId) {
                        option.selected = true;
                    }
                    professorSelect.appendChild(option);
                });
            })
            .catch(err => console.error("Failed to load professors:", err))
            .finally(() => {
                loaderContainer.remove();
                professorSelect.style.display = "";
            });

        return form;
    }

    async function handleCourseSubmit(e, course) {
        e.preventDefault();
        
        const hideOverlay = showLoadingOverlay();
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
            hideOverlay();
            renderCourses();
        } catch (err) {
            alert("Error creating the course: " + err.message);
        }
    }

    async function handleCourseRemove(courseId)
    {
        createConfirmModal(
            "Are you sure you want to delete this course?",
            async () => {
                const hideOverlay = showLoadingOverlay();
                try{
                    await deleteCourse(courseId);
                }
                finally
                {
                    hideOverlay();
                }
                clearRoot();
                renderCourses();
            }
        )
    }

    async function openEditCourseModal(course) {
        const overlay = createElement("div", "modal-overlay");
        
        const modalContent = createElement("div", "modal-content");
        
        const header = createElement("div", "modal-header");
        const modalTitle = createElement("h2", "modal-title", "Edit Course");
        const closeBtn = createElement("button", "modal-close", "×");
        closeBtn.addEventListener("click", () => overlay.remove());
        header.append(modalTitle, closeBtn);
        
        const form = createCourseForm(course);
        
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
        const modalTitle = createElement("h2", "modal-title", "Pass the course");
        const closeBtn = createElement("button", "modal-close", "×");
        closeBtn.addEventListener("click", () => overlay.remove());
        header.append(modalTitle, closeBtn);
        
        const form = createElement("form", "course-form");
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            const hideOverlay = showLoadingOverlay();

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
                hideOverlay();
                renderCourses();
            } catch (err) {
                alert("Error passing exam: " + err.message);
            }
        });
        
        const courseInfo = createElement("p", "modal-info", `Course: ${course.title}`);
        courseInfo.style.marginBottom = "20px";
        courseInfo.style.fontSize = "16px";
        
        const gradeGroup = createElement("div", "form-group");
        const gradeLabel = createElement("label", "form-label", "Grade");
        const gradeInput = createElement("input", "form-input");
        gradeInput.type = "number";
        gradeInput.name = "grade";
        gradeInput.min = "6";
        gradeInput.max = "10";
        gradeInput.required = true;
        gradeInput.placeholder = "6-10";
        gradeGroup.append(gradeLabel, gradeInput);
        
        const actions = createElement("div", "form-actions");
        const submitBtn = createElement("button", "btn-submit", "Pass the course");
        submitBtn.type = "submit";
        const cancelBtn = createElement("button", "btn-cancel", "Cancel");
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
