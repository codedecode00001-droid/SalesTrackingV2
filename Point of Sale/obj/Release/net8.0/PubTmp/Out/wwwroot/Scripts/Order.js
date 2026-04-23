document.addEventListener("DOMContentLoaded", function () {
    'use strict';

    const form = document.getElementById('orderForm');
    if (!form) return;

    form.addEventListener('submit', function (event) {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            event.preventDefault(); // stop real submit
            saveTempOrder();        // call your function
        }

        form.classList.add('was-validated');
    }, false);
});

document.addEventListener("DOMContentLoaded", function () {
    loadCategoryOrder();
    loadUnits();

    document
        .getElementById("categoryDropdownOrder")
        .addEventListener("change", loadProductOrder);

    document
        .getElementById("productDropdownOrder")
        .addEventListener("change", loadPriceStock);

    document
        .getElementById("qty")
        .addEventListener("input", loadTotalPrice);
});

function loadCategoryOrder() {
    fetch('/Orders?handler=Category')
        .then(response => response.json())
        .then(data => {
            // console.log(data); // [{id:1, name:"A"}, ...]
            bindCategoryDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindCategoryDropdown(categories) {
    const ddl = document.getElementById("categoryDropdownOrder");
    ddl.innerHTML = '<option value="">-- Select Category --</option>';

    categories.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });
}

function loadProductOrder() {
    const catValue = document.getElementById("categoryDropdownOrder").value;
    fetch(`/Orders?handler=GetProd&catId=${encodeURIComponent(catValue)}`)
        .then(response => response.json())
        .then(data => {
            // console.log(data); // [{id:1, name:"A"}, ...]
            bindProductDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindProductDropdown(product) {
    const ddl = document.getElementById("productDropdownOrder");
    ddl.innerHTML = '<option value="">-- Select Product --</option>';

    product.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });
}

function loadUnits() {
    fetch('/Orders?handler=GetUnits')
        .then(response => response.json())
        .then(data => {
            // console.log(data); // [{id:1, name:"A"}, ...]
            bindUnitsDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindUnitsDropdown(units) {
    const ddl = document.getElementById("unitsDropdown");
    ddl.innerHTML = '<option value="">-- Select Units --</option>';

    units.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });
}

async function loadPriceStock() {
    const prodValue = document.getElementById("productDropdownOrder").value;
    const res = await fetch(`/Orders?handler=GetPriceStock&prodId=${encodeURIComponent(prodValue)}`);
    const o = await res.json();

    document.getElementById("price").value = o.price;
    document.getElementById("stock").value = o.stock;
}

async function loadTotalPrice() {
    const price = parseFloat(document.getElementById("price").value) || 0;
    const qtyInput = document.getElementById("qty");
    const qty = parseInt(qtyInput.value) || 0;
    const stock = parseInt(document.getElementById("stock").value) || 0;
    const editId = parseInt(document.getElementById("oid").value) || 0;

    if (qty <= 0) {
        qtyInput.setCustomValidity("Quantity must be at least 1");
        return;
    }

    if (editId === 0 && qty > stock) {
        qtyInput.setCustomValidity("Quantity exceeds stock");
        showMessage("The stock does not match the quantity!", "danger");
        document.getElementById("totalPrice").value = "";
        return;
    }

    // VALID
    qtyInput.setCustomValidity("");
    document.getElementById("msg").innerText = "";

    const total = price * qty;
    document.getElementById("totalPrice").value = total.toFixed(2);
}

let dataTable;

document.addEventListener("DOMContentLoaded", async () => {
    dataTable = new DataTable('#example', {
        responsive: true,
        autoWidth: false
    });
    await loadOrders();
});

async function loadOrders() {
    const res = await fetch('/Orders?handler=List');
    const list = await res.json();

    let html = "";
    list.forEach(o => {
        html += `
       <tr
          data-id="${o.id}"
          data-cat-id="${o.cat_id}"
          data-prod-id="${o.prod_id}"
          data-qty="${o.qty}"
        >
            <td>${o.product_name}</td>
            <td>${o.price}</td>
            <td>${o.qty}</td>
            <td>${o.description}</td>
            <td>${o.unit_name}</td>
            <td>${o.total_price}</td>
            <td style="text-align:center">
                <button class="btn btn-warning btn-sm" onclick="editOrder(${o.id})">
                    <i class="fa-solid fa-pencil"></i>
                </button>
                <button class="btn btn-danger btn-sm" onclick="deleteOrder(this, ${o.id})">
                   <i class="fa-solid fa-trash"></i>
                </button>
            </td>
        </tr>
        `;
    });

    const tableElement = document.getElementById("cartTable");
    tableElement.innerHTML = html;

    dataTable.clear();
    dataTable.rows.add($('#cartTable tr'));
    dataTable.draw(false);
}

// --------------------------------------
// Compute Total Amount
// --------------------------------------
function computeTotalRevenue() {
    let total = 0;

    reportTable
        .column(5, { search: 'applied' }) // Total Price column
        .data()
        .each(value => {
            if (value !== '') {
                total += parseFloat(value.toString().replace(/[₱,]/g, '')) || 0;
            }
        });

    document.getElementById('totalAmount').innerText =
        `₱ ${total.toLocaleString()}`;
}


// --------------------------------------
// INSERT or UPDATE Temp Table
// --------------------------------------
async function saveTempOrder() {

    const catValue = document.getElementById("categoryDropdownOrder").value;
    const prodValue = document.getElementById("productDropdownOrder").value;
    const units = document.getElementById("unitsDropdown").value;
    const priceValue = document.getElementById("price").value;
    const qty = document.getElementById("qty").value;
    const stock = document.getElementById("stock").value;
    const total_price = document.getElementById("totalPrice").value;

    const data = {
        id: parseInt(document.getElementById("oid").value),
        cat_id: catValue === "" ? null : parseInt(catValue),
        prod_id: prodValue === "" ? null : parseInt(prodValue),
        price: priceValue === "" ? null : parseInt(priceValue),
        qty: qty === "" ? null : parseInt(qty),
        stock: stock === "" ? null : parseInt(stock),
        units: units === "" ? null : parseInt(units),
        description: document.getElementById("description").value,
        total_price: total_price === "" ? null : parseInt(total_price)
    };

    console.log(data);

    const url = data.id === 0
        ? "/Orders?handler=AddCart"
        : "/Orders?handler=UpdateCart";

    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    const result = await res.json();

    showMessage(result.message, "success");
 
    loadOrders();
    resetFormOrder();

    computeTotalRevenue();
}
// --------------------------------------
// END INSERT or UPDATE Temp Table
// --------------------------------------

// --------------------------------------
// INSERT Order Main Table
// --------------------------------------
async function saveOrder() {

    if (document.querySelectorAll('#cartTable tr').length === 0) {
        alert('Cart is empty!');
        return;
    }

    showLoading();

    try {
        const res = await fetch("/Orders?handler=SaveOrder", {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        });

        const result = await res.json();

        showMessage(result.message, "success");
        
        if (result.success) {
            await loadOrders();
        }
    }
    catch (err) {
        console.error(err);
        showMessage("Failed to save order.", "danger");
    }
    finally {
        hideLoading();
    }
}
// --------------------------------------
// END INSERT Order Main Table
// --------------------------------------

//function formatForDateInput(dateStr) {
//    const d = new Date(dateStr);
//    const yyyy = d.getFullYear();
//    const mm = String(d.getMonth() + 1).padStart(2, '0');
//    const dd = String(d.getDate()).padStart(2, '0');
//    return `${yyyy}-${mm}-${dd}`;
//}

// --------------------------------------
// LOAD DATA FOR EDITING
// --------------------------------------
async function editOrder(id) {
    const res = await fetch(`/Orders?handler=Get&Id=${id}`);
    const p = await res.json();

    document.getElementById("oid").value = p.id;
    document.getElementById("categoryDropdownOrder").value = p.cat_id;
    document.getElementById("btnAddCart").innerText = "Update Order";

    loadProductOrder()

    setTimeout(function () {
        document.getElementById("productDropdownOrder").value = p.prod_id;
        document.getElementById("price").value = p.price;
        document.getElementById("qty").value = p.qty;
        document.getElementById("stock").value = p.stock;
        document.getElementById("unitsDropdown").value = p.units;
        document.getElementById("description").value = p.description;
        document.getElementById("totalPrice").value = p.total_price;
    }, 1000); 
}


// --------------------------------------
// BEEP SOUND AFTER SCAN QR
// --------------------------------------
//function playBeep() {
//    const audio = new Audio("https://actions.google.com/sounds/v1/alarms/beep_short.ogg");
//    audio.play();
//}
function playBeep() {
    const audioCtx = new (window.AudioContext || window.webkitAudioContext)();
    const oscillator = audioCtx.createOscillator();
    const gainNode = audioCtx.createGain();

    oscillator.type = "sine";
    oscillator.frequency.setValueAtTime(1000, audioCtx.currentTime); // beep pitch
    oscillator.connect(gainNode);
    gainNode.connect(audioCtx.destination);

    oscillator.start();

    gainNode.gain.exponentialRampToValueAtTime(
        0.00001,
        audioCtx.currentTime + 0.15 // duration
    );

    oscillator.stop(audioCtx.currentTime + 0.15);
}

// --------------------------------------
// LOAD DATA FOR SCAN QR
// --------------------------------------
let currentCameraIndex = 0;
let cameras = [];
let html5QrCode;
let isScanning = false;

async function openScanner() {
    const modal = new bootstrap.Modal(document.getElementById("qrModal"));
    modal.show();
    await startScanner();
}

async function startScanner() {

    html5QrCode = new Html5Qrcode("qr-reader");

    cameras = await Html5Qrcode.getCameras();

    if (!cameras.length) {
        alert("No camera found");
        return;
    }

    startCamera(cameras[currentCameraIndex].id);
}

async function startCamera(cameraId) {

    isScanning = false; // Reset scanning flag for new camera

    await html5QrCode.start(
        cameraId,
        { fps: 10, qrbox: 250 },
        (decodedText, decodedResult) => { // <-- two parameters
            onScanSuccess(decodedText, decodedResult);
        },
        (errorMessage) => {
            // optional: handle scan failure for each frame
            // console.log("Scan error:", errorMessage);
        }
    );
}

async function switchCamera() {

    if (!cameras.length) return;

    try {
        await html5QrCode.stop();
    } catch (e) {
        console.warn("Camera stop error", e);
    }

    currentCameraIndex = (currentCameraIndex + 1) % cameras.length;

    startCamera(cameras[currentCameraIndex].id);
}

async function onScanSuccess(decodedText, decodedResult) {

    if (isScanning) return;

    isScanning = true;

    console.log("Scanned Text:", decodedText);
    console.log("Full Result:", decodedResult);

    // call your order fill function
    await fillOrderForm(decodedText);

    try {
        await html5QrCode.stop();
    } catch (e) {
        console.warn("Stop camera error", e);
    }

    bootstrap.Modal.getInstance(document.getElementById("qrModal")).hide();

    isScanning = false;
}

async function fillOrderForm(prodId) {

    try {
        const response = await fetch(`/Orders?handler=OrderProd&Id=${prodId}`);

        if (!response.ok) {
            alert("Product not found.");
            return;
        }

        const p = await response.json();

        console.log(p);

        if (!p) {
            alert("No data returned.");
            return;
        }


        // ✅ AUTO FILL FORM
        document.getElementById("categoryDropdownOrder").value = p.cat_id;

        await loadProductOrder(); // ✅ better to await instead of setTimeout

        setTimeout(function () {
            document.getElementById("productDropdownOrder").value = p.prod_id;
            document.getElementById("price").value = p.price;
            document.getElementById("stock").value = p.stock;
            document.getElementById("description").value = p.description;
        }, 1000); 

        playBeep();

    } catch (err) {
        console.error("Error loading product:", err);
        alert("Error loading product.");
    }
}

// --------------------------------------
// CLOSE CAMERA
// --------------------------------------
const qrModal = document.getElementById('qrModal');

qrModal.addEventListener('hidden.bs.modal', async function () {

    if (html5QrCode) {
        try {
            await html5QrCode.stop();
            await html5QrCode.clear();
            console.log("Camera stopped");
        } catch (err) {
            console.log("Scanner already stopped");
        }
    }

});

// --------------------------------------
// DELETE
// --------------------------------------
//let deleteCallback = null;

//function confirmDelete(callback) {
//    deleteCallback = callback;

//    const modal = new bootstrap.Modal(
//        document.getElementById('confirmDeleteModal')
//    );
//    modal.show();
//}

//document.getElementById('confirmDeleteBtn')
//    .addEventListener('click', function () {
//        if (deleteCallback) deleteCallback();

//        const modalEl = document.getElementById('confirmDeleteModal');
//        bootstrap.Modal.getInstance(modalEl).hide();
//    });

async function deleteOrder(btn, id) {
    const row = btn.closest("tr");
    const prodId = parseInt(row.dataset.prodId);
    const qty = parseInt(row.dataset.qty);

    const res = await fetch('/Orders?handler=DeleteItemCart', {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id, prodId, qty })
    });

    const result = await res.json();
    showMessage(result.message, result.success ? "success" : "danger");

    loadOrders();
    computeTotalRevenue();

    //confirmDelete(async () => {

    //    const row = btn.closest("tr");
    //    const prodId = parseInt(row.dataset.prodId);
    //    const qty = parseInt(row.dataset.qty);

    //    const res = await fetch('/Orders?handler=DeleteItemCart', {
    //        method: "POST",
    //        headers: { "Content-Type": "application/json" },
    //        body: JSON.stringify({ id, prodId, qty })
    //    });

    //    const result = await res.json();
    //    showMessage(result.message, result.success ? "success" : "danger");

    //    loadOrders();
    //});
}


// --------------------------------------
// RESET FORM
// --------------------------------------
function resetFormOrder() {
    const form = document.getElementById("orderForm");
    form.reset();
    form.classList.remove("was-validated");

    document.getElementById("oid").value = 0;
    document.getElementById("price").value = "";
    document.getElementById("stock").value = "";
    document.getElementById("totalPrice").value = "";
}