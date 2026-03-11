function generatePin() {

    let pin = Math.floor(1000 + Math.random() * 9000);
    document.getElementById("pinCode").value = pin;

}

// auto generate on page load
window.onload = function () {
    generatePin();
};

document.addEventListener("DOMContentLoaded", function () {
    'use strict';

    const form = document.getElementById('userForm');
    if (!form) return;

    form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            event.preventDefault(); // stop real submit
            saveUser();          // your existing function
        }

        form.classList.add('was-validated');
    }, false);
});

document.addEventListener("DOMContentLoaded", async () => {
    dataTable = new DataTable('#example', {
        responsive: true
    });

    await loadUsers();
});


// Get List of Product
async function loadUsers() {
    const res = await fetch('/Pin?handler=List');

    if (!res.ok) {
        console.error("Failed to load users");
        return;
    }

    const list = await res.json();

    console.log(list);

    dataTable.clear();

    list.forEach(u => {
        dataTable.row.add([
            u.first_name ?? "",
            u.middle_name ?? "",
            u.last_name ?? "",
            u.position ?? "",
            u.status ?? "",
            `
            <button class="btn btn-warning btn-sm" onclick="editUser(${u.id})">
                <i class="fa-solid fa-pencil"></i>
            </button>
            <button class="btn btn-danger btn-sm" onclick="deleteUser(${u.id})">
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
async function saveUser() {
    const selected = document.querySelector('input[name="choice"]:checked');
    //console.log(selected.value);

    const pin_code = document.getElementById("pinCode").value;
   
    const data = {
        id: parseInt(document.getElementById("Id").value),
        first_name: document.getElementById("fname").value,
        middle_name: document.getElementById("mname").value,
        last_name: document.getElementById("lname").value,
        position: document.getElementById("positionDropdown").value,
        status: selected ? selected.value : null,
        pin_code: pin_code === "" ? null : parseInt(pin_code),
    };

    console.log(data);

    const isInsert = data.id === 0;

    const url = isInsert
        ? "/Pin?handler=Insert"
        : "/Pin?handler=Update";

    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    let result = {};
    if (res.headers.get("content-type")?.includes("application/json")) {
        result = await res.json();
    }

    showMessage(result.message ?? "User saved successfully", "success");

    await loadUsers();
    resetUserForm();
}

// --------------------------------------
// LOAD DATA FOR EDITING
// --------------------------------------
async function editUser(id) {
    const res = await fetch(`/Pin?handler=Get&Id=${id}`);
    const p = await res.json();

    document.getElementById("Id").value = p.id;
    document.getElementById("pinCode").value = p.pin_code;
    document.getElementById("fname").value = p.first_name;
    document.getElementById("mname").value = p.middle_name;
    document.getElementById("lname").value = p.last_name;
    document.getElementById("positionDropdown").value = p.position;
   
    // Check status based on selected data
    if (p.status === true || p.status === 1 || p.status === "Active") {
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

async function deleteUser(id) {
    confirmDelete(async () => {

        const res = await fetch('/Pin?handler=Delete', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id })
        });

        const result = await res.json();
        showMessage(result.message, result.success ? "success" : "danger");

        loadUsers();
    });
}

// --------------------------------------
// RESET FORM
// --------------------------------------
function resetUserForm() {
    const form = document.getElementById("userForm");
    form.reset();
    form.classList.remove("was-validated");
    document.getElementById("Id").value = 0;
}