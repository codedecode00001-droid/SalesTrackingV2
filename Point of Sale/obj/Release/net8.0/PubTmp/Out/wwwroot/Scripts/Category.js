// Load list on page load
let dataTable;

document.addEventListener("DOMContentLoaded", async () => {
    dataTable = new DataTable('#example', {
        responsive: true
    });

    await loadCategories();
});

document.addEventListener("DOMContentLoaded", function () {
    'use strict';

    const form = document.getElementById('categoryForm');
    if (!form) return;

    form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            event.preventDefault(); // stop real submit
            saveCategory();         // call your existing save function
        }

        form.classList.add('was-validated');
    }, false);
});


// Get List of Category
async function loadCategories() {
    const res = await fetch('/Category?handler=List');

    if (!res.ok) {
        console.error("Failed to load category");
        return;
    }

    const list = await res.json();

    dataTable.clear();

    list.forEach(c => {
        dataTable.row.add([
            c.category_name ?? "",
            c.description ?? "",
            c.status ?? "",
            c.date_created
                ? new Date(c.date_created).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                })
                : "",
            `
             <button class="btn btn-warning btn-sm" onclick="editCategory(${c.cat_id})">
                <i class="fa-solid fa-pencil"></i>
            </button>
             <button class="btn btn-danger btn-sm" onclick="deleteCategory(${c.cat_id})">
               <i class="fa-solid fa-trash"></i>
            </button>
            `
        ]);
    });

    dataTable.draw(false); // keeps pagination
}

// --------------------------------------
// INSERT or UPDATE
// --------------------------------------
async function saveCategory() {
    const selected = document.querySelector('input[name="choice"]:checked');

    const data = {
        cat_id: parseInt(document.getElementById("catId").value),
        category_name: document.getElementById("categoryName").value,
        status: selected ? selected.value : null,
        description: document.getElementById("description").value
    };

    const url = data.cat_id === 0
        ? "/Category?handler=Insert"
        : "/Category?handler=Update";

    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    let result = {};
    if (res.headers.get("content-type")?.includes("application/json")) {
        result = await res.json();
    }

    showMessage(result.message ?? "Saved successfully", "success");


    await loadCategories();
    resetCategoryForm();
}

// --------------------------------------
// LOAD DATA FOR EDITING
// --------------------------------------
async function editCategory(cat_id) {
    const res = await fetch(`/Category?handler=Get&catid=${cat_id}`);
    const c = await res.json();

    document.getElementById("catId").value = c.cat_id;
    document.getElementById("categoryName").value = c.category_name;
    document.getElementById("description").value = c.description;

    // Check status based on selected data
    if (c.status === true || c.status === 1 || c.status === "Available") {
        document.getElementById("option1").checked = true;
        document.getElementById("option2").checked = false;
    } else {
        document.getElementById("option2").checked = true;
        document.getElementById("option1").checked = false;
    }

   // document.getElementById("formTitle").innerText = "Edit Category";
}

// --------------------------------------
// DELETE
// --------------------------------------

let deleteCallback = null;

function confirmDelete(callback) {
    deleteCallback = callback;

    const modal = new bootstrap.Modal(
        document.getElementById('confirmDeleteModal')
    );
    modal.show();
}

document.getElementById('confirmDeleteBtn')
    .addEventListener('click', function () {
        if (deleteCallback) deleteCallback();

        const modalEl = document.getElementById('confirmDeleteModal');
        bootstrap.Modal.getInstance(modalEl).hide();
    });


async function deleteCategory(cat_id) {
    confirmDelete(async () => {

        const res = await fetch('/Category?handler=Delete', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ cat_id })
        });

        const result = await res.json();
        showMessage(result.message, result.success ? "success" : "danger");

        loadCategories();
    });
}


// --------------------------------------
// RESET FORM
// --------------------------------------
function resetCategoryForm() {
    const form = document.getElementById("categoryForm");
    form.reset();
    form.classList.remove("was-validated");
    document.getElementById("catId").value = 0;
}


