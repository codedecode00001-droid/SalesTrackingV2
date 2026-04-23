document.addEventListener("DOMContentLoaded", async () => {
    await loadOrders();
});

// Fetch Orders
async function loadOrders() {
    const res = await fetch('/Return?handler=List');

    if (!res.ok) {
        console.error("Failed to load orders");
        return;
    }

    const orders = await res.json();

    const container = document.getElementById("ordersContainer");
    container.innerHTML = "";

    orders.forEach((order, orderIndex) => {
        let total = 0;

        console.log(order);

        let productRows = order.products.map(p => {
            let subtotal = p.price * p.qty;
            total += subtotal;

            return `
                <tr
                  data-order-id="${p.order_no}"
                  data-prod-id="${p.prod_id}"
                  data-price="${p.price}"
                  data-qty="${p.qty}"
                >
                    <td>${p.product_name}</td>
                    <td class="text-center">₱${p.price}</td>
                    <td class="text-center">
                        <input type="number"
                               class="form-control form-control-sm text-center qty"
                               style="width:80px;margin:auto;"
                               value="${p.qty}"
                               min="1"
                               data-order="${orderIndex}"
                               data-price="${p.price}" />
                    </td>
                    <td class="text-center subtotal">₱${subtotal}</td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-outline-primary me-1" onclick="editRow(this)">
                            <i class="fa-solid fa-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger" onclick="removeRow(this)">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        }).join("");

        container.innerHTML += `
            <div class="card shadow-sm mb-4">
                <div class="card-header d-flex justify-content-between">
                    <strong>${order.order_id}</strong>
                    <span>Total: ₱<span id="total-${orderIndex}">${total}</span></span>
                </div>

                <div class="card-body p-0">
                    <table class="table table-hover mb-0 align-middle">
                        <thead class="table-light">
                            <tr>
                                <th>Product</th>
                                <th class="text-center">Price</th>
                                <th class="text-center">Qty</th>
                                <th class="text-center">Subtotal</th>
                                <th class="text-center">Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${productRows}
                        </tbody>
                    </table>
                </div>
            </div>
        `;
    });
}

// Live qty change
document.addEventListener("input", function (e) {
    if (e.target.classList.contains("qty")) {
        let input = e.target;
        let row = input.closest("tr");

        let price = parseFloat(input.dataset.price);
        let qty = parseInt(input.value) || 0;

        // Update subtotal
        row.querySelector(".subtotal").innerText = "₱" + (price * qty);

        // Update total
        let orderIndex = input.dataset.order;
        updateTotal(orderIndex);
    }
});

// Update total per order
function updateTotal(orderIndex) {

    let inputs = document.querySelectorAll(`input[data-order='${orderIndex}']`);
    let total = 0;

    inputs.forEach(input => {
        let price = parseFloat(input.dataset.price);
        let qty = parseInt(input.value) || 0;
        total += price * qty;
    });

    document.getElementById("total-" + orderIndex).innerText = total;
}

// Delete Item
async function removeRow(btn) {
    if (!btn) {
        console.error("Button is undefined");
        return;
    }

    const row = btn.closest("tr");
    if (!row) {
        console.error("Row not found");
        return;
    }

    const order_no = row.dataset.orderId || "";
    const prod_id = Number(row.dataset.prodId);
    const price = Number(row.dataset.price);

    // ✅ Get latest qty from input (NOT dataset)
    const qtyInput = row.querySelector(".qty");
    if (!qtyInput) {
        console.error("Qty input not found");
        return;
    }

    const qty = Number(qtyInput.value);

    // Validation
    if (!order_no || isNaN(prod_id) || isNaN(price) || isNaN(qty)) {
        showMessage("Invalid row data!", "danger");
        return;
    }

    try {
        const tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');

        const res = await fetch('/Return?handler=DeleteItem', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(tokenEl && { "RequestVerificationToken": tokenEl.value })
            },
            body: JSON.stringify({
                order_no,
                prod_id,
                price,
                qty
            })
        });

        if (!res.ok) {
            throw new Error(`HTTP ${res.status}`);
        }

        const result = await res.json();

        //showMessage(result.message, result.success ? "success" : "danger");

        // ✅ Only update UI if backend succeeded
        if (result.success) {
            const orderIndex = qtyInput.dataset.order;

            row.remove();

            if (orderIndex !== undefined) {
                updateTotal(orderIndex);
            }
        }

    } catch (err) {
        console.error(err);
        showMessage("Delete failed. Check console.", "danger");
    }
}

// Edit/ Update Item
async function editRow(btn) {
    if (!btn) return;

    const row = btn.closest("tr");
    if (!row) {
        console.error("Row not found");
        return;
    }

    const order_no = row.dataset.orderId || "";
    const prod_id = Number(row.dataset.prodId);
    const price = Number(row.dataset.price);

    // ✅ GET UPDATED QTY FROM INPUT (THIS IS THE FIX)
    const qtyInput = row.querySelector(".qty");
    if (!qtyInput) {
        console.error("Qty input not found");
        return;
    }

    const qty = Number(qtyInput.value);

    // Validation
    if (!order_no || isNaN(prod_id) || isNaN(price) || isNaN(qty)) {
        showMessage("Invalid row data!", "error");
        return;
    }

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        const res = await fetch('/Return?handler=UpdateItem', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                ...(token && { "RequestVerificationToken": token })
            },
            body: JSON.stringify({
                order_no,
                prod_id,
                price,
                qty
            })
        });

        if (!res.ok) {
            throw new Error(`HTTP ${res.status}`);
        }

        const result = await res.json();

        //showMessage(result.message, result.success ? "success" : "info");

        // ✅ Reset styles first
        row.classList.remove("table-danger", "table-warning");

        if (!result.success) {
            row.classList.add("table-danger");
        } else {
            row.dataset.qty = qty;

            const subtotalCell = row.querySelector(".subtotal");
            if (subtotalCell) {
                subtotalCell.textContent = `₱${price * qty}`;
            }

            row.classList.add("table-warning");

            const qtyInput = row.querySelector(".qty");
            const orderIndex = qtyInput?.dataset.order;

            if (orderIndex !== undefined) {
                updateTotal(orderIndex);
            }
        }

        // ✅ Optional: auto-clear highlight
        setTimeout(() => {
            row.classList.remove("table-danger", "table-warning");
        }, 2000);

    } catch (err) {
        console.error(err);
        showMessage("Update failed. Check console.", "error");
    }
}