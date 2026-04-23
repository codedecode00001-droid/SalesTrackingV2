document.addEventListener("DOMContentLoaded", function () {
    loadCategory();
});

document.addEventListener("DOMContentLoaded", function () {
    'use strict';

    const form = document.getElementById('productForm');
    if (!form) return;

    form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            event.preventDefault(); // stop real submit
            saveProduct();          // your existing function
        }

        form.classList.add('was-validated');
    }, false);
});

function loadCategory() {
    fetch('/Product?handler=Category')
        .then(response => response.json())
        .then(data => {
           // console.log(data); // [{id:1, name:"A"}, ...]
            bindCategoryDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindCategoryDropdown(categories) {
    const ddl = document.getElementById("categoryDropdown");
    ddl.innerHTML = '<option value="">-- Select Category --</option>';

    categories.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });
}

let dataTable;

document.addEventListener("DOMContentLoaded", async () => {
    dataTable = new DataTable('#example', {
        responsive: true
    });

    await loadProducts();
});


// Get List of Product
async function loadProducts() {
    const res = await fetch('/Product?handler=List');

    if (!res.ok) {
        console.error("Failed to load products");
        return;
    }

    const list = await res.json();

    dataTable.clear();

    list.forEach(p => {
        dataTable.row.add([
            p.category_name ?? "",
            p.product_name ?? "",
            p.price ?? 0,
            p.stock ?? 0,
            p.date_expired
                ? new Date(p.date_expired).toLocaleDateString('en-US')
                : "",
            p.status ?? "",
            p.description ?? "",
            p.date_created ?? "",
            `
            <button class="btn btn-warning btn-sm" onclick="editProduct(${p.prod_id})">
                <i class="fa-solid fa-pencil"></i>
            </button>
            <button class="btn btn-danger btn-sm" onclick="deleteProduct(${p.prod_id})">
                <i class="fa-solid fa-trash"></i>
            </button>
            <button class="btn btn-primary btn-sm" onclick="qrProduct(${p.prod_id})">
                <i class="fa-solid fa-qrcode"></i>
            </button>           
            `
        ]);
    });

    dataTable.draw(false); // keeps pagination
}

// --------------------------------------
// INSERT or UPDATE
// --------------------------------------
async function saveProduct() {
    const selected = document.querySelector('input[name="choice"]:checked');
    //console.log(selected.value);

    const catValue = document.getElementById("categoryDropdown").value;
    const priceValue = document.getElementById("price").value;
    const stockValue = document.getElementById("stock").value;

    const data = {
        prod_id: parseInt(document.getElementById("prodId").value),
        cat_id: catValue === "" ? null : parseInt(catValue),
        product_name: document.getElementById("product").value,
        price: priceValue === "" ? null : parseInt(priceValue),
        stock: stockValue === "" ? null : parseInt(stockValue),
        date_expired: document.getElementById("expiredDate").value,
        status: selected ? selected.value : null,
        description: document.getElementById("description").value
    };

    console.log(data); 

    const isInsert = data.prod_id === 0;

    const url = isInsert
        ? "/Product?handler=Insert"
        : "/Product?handler=Update";

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

    //// ✅ Generate QR after save
    //if (isInsert && result.prod_id) {
    //    generateAndPrintQR({ ...data, prod_id: result.prod_id });
    //}else if (!isInsert && result.prod_id) {
    //    // Ask user before generating QR on edit
    //    const confirmed = await askGenerateQR({ ...data, prod_id: result.prod_id });
    //    if (confirmed) {
    //        generateAndPrintQR({ ...data, prod_id: result.prod_id });
    //    }
    //}

    await loadProducts();
    resetProductForm();
}

// Function to ask confirmation using Bootstrap modal
function askGenerateQR(product) {
    return new Promise((resolve) => {
        const modalEl = document.getElementById('qrConfirmModal');
        const productNameSpan = document.getElementById('qrProductName');
        productNameSpan.textContent = product.product_name;

        const confirmBtn = document.getElementById('confirmQRBtn');

        // Show modal
        const bsModal = new bootstrap.Modal(modalEl);
        bsModal.show();

        // On confirm
        confirmBtn.onclick = () => {
            bsModal.hide();
            resolve(true);
        };

        // On modal close without confirming
        modalEl.addEventListener('hidden.bs.modal', () => {
            resolve(false);
        }, { once: true });
    });
}

let selectedProductId = null;

// --------------------------------------
// Generate QR Code
// --------------------------------------
async function qrProduct(prod_id) {
    //const res = await fetch(`/Product?handler=Get&prodId=${prod_id}`);
    //const qrProduct = await res.json();

    //generateAndPrintQR({ ...qrProduct });
    selectedProductId = prod_id;

    const modal = new bootstrap.Modal(document.getElementById('qrQtyModal'));
    modal.show();
}

// --------------------------------------
// Confirmation for how many QR Code create
// --------------------------------------
async function confirmQRPrint() {

    const qty = document.getElementById("qrQty").value;

    const res = await fetch(`/Product?handler=Get&prodId=${selectedProductId}`);
    const qrProduct = await res.json();

    generateAndPrintQR(qrProduct, qty);

    bootstrap.Modal.getInstance(document.getElementById('qrQtyModal')).hide();
}

// --------------------------------------
// LOAD QR CODE
// --------------------------------------
function generateAndPrintQR(product, qty) {
    const qrDiv = document.getElementById("qrcode");
    qrDiv.innerHTML = "";

    const qrData = product.prod_id.toString();

    new QRCode(qrDiv, {
        text: qrData,
        width: 80,
        height: 80
    });

    setTimeout(() => {

        const qrImage =
            qrDiv.querySelector("img")?.src ||
            qrDiv.querySelector("canvas")?.toDataURL();

        if (!qrImage) {
            console.error("QR image not generated");
            return;
        }

        // [Optional]
        /* <div class="qrCode">${product.prod_id}</div>*/
        //.qrCode{
        //    font - size: 11px;
        //    letter - spacing: 2px;
        //}

        let labels = "";

        for (let i = 0; i < qty; i++) {
            labels += `
                <div class="label">
                    <div class="product-name">${product.product_name}</div>
                    <img src="${qrImage}" />
                    <div class="price">₱ ${product.price ?? ""}</div>
                </div>
            `;
        }

        const printWindow = window.open("", "_blank");

        printWindow.document.write(`
            <html>
            <head>
            <title>Print QR Code</title>

            <style>
               @media print {
                    @page {
                        size: A4;
                        margin: 10mm;
                    }
                }

                body{
                    font-family: Arial;
                }

                .container{
                    display:grid;
                    grid-template-columns: repeat(5, 1fr); /* 5 stickers per row */
                    gap:10px;
                }

                .label{
                    border:1px dashed #ccc;
                    padding:5px;
                    text-align:center;
                    break-inside: avoid;
                }

                .product-name{
                    font-size:10px;
                    font-weight:bold;
                }

                img{
                    width:70px;
                    height:70px;
                }

                .price{
                    font-size:10px;
                } 
            </style>

            </head>

            <body>

            <div class="container">
                ${labels}
            </div>

            </body>
            </html>
        `);

        printWindow.onload = function () {
            printWindow.focus();
            printWindow.print();
            printWindow.close();
        };

    }, 300);
}


// --------------------------------------
// LOAD FOR FORMAT DATE
// --------------------------------------
function formatForDateInput(dateStr) {
    const d = new Date(dateStr);
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
}

// --------------------------------------
// LOAD DATA FOR EDITING
// --------------------------------------
async function editProduct(prod_id) {
    const res = await fetch(`/Product?handler=Get&prodId=${prod_id}`);
    const p = await res.json();

    document.getElementById("prodId").value = p.prod_id;
    document.getElementById("categoryDropdown").value = p.cat_id;
    document.getElementById("product").value = p.product_name;
    document.getElementById("price").value = p.price;
    document.getElementById("stock").value = p.stock;
    document.getElementById("expiredDate").value = formatForDateInput(p.date_expired);
    document.getElementById("description").value = p.description;

    // Check status based on selected data
    if (p.status === true || p.status === 1 || p.status === "Available") {
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

async function deleteProduct(prod_id) {
    confirmDelete(async () => {

        const res = await fetch('/Product?handler=Delete', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ prod_id })
        });

        const result = await res.json();
        showMessage(result.message, result.success ? "success" : "danger");

        loadProducts();
    });
}


// --------------------------------------
// RESET FORM
// --------------------------------------
function resetProductForm() {
    const form = document.getElementById("productForm");
    form.reset();
    form.classList.remove("was-validated");
    document.getElementById("prodId").value = 0;
}