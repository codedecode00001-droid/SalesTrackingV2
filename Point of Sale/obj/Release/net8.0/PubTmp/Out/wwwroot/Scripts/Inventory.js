document.addEventListener("DOMContentLoaded", function () {
    loadCategoryInv();
    loadInventory();

    document
        .getElementById("categoryDropdownInv")
        .addEventListener("change", onCategoryChange);

    document
        .getElementById("productDropdownInv")
        .addEventListener("change", loadInvListByProd);
});

async function onCategoryChange() {
    await loadProductInv();      // populate product dropdown
    await loadInvListByCat();    // load table by category
}

function loadCategoryInv() {
    fetch('/Inventory?handler=Category')
        .then(response => response.json())
        .then(data => {
            // console.log(data); // [{id:1, name:"A"}, ...]
            bindCategoryDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindCategoryDropdown(categories) {
    const ddl = document.getElementById("categoryDropdownInv");
    ddl.innerHTML = '<option value="">-- Select Category --</option>';

    categories.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });
}

function loadProductInv() {
    const catValue = document.getElementById("categoryDropdownInv").value;
    fetch(`/Inventory?handler=GetProd&catId=${encodeURIComponent(catValue)}`)
        .then(response => response.json())
        .then(data => {
            // console.log(data); // [{id:1, name:"A"}, ...]
            bindProductDropdown(data);
        })
        .catch(error => console.error('Error:', error));
}

function bindProductDropdown(product) {
    const ddl = document.getElementById("productDropdownInv");
    ddl.innerHTML = '<option value="">-- Select Product --</option>';

    product.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.text = c.name;
        ddl.appendChild(option);
    });

    // Make a select editable
    document.getElementById("productDropdownInv").disabled = false;
}


async function loadInvListByCat() {
    const catId = document.getElementById("categoryDropdownInv").value;

    if (!catId) {
        loadInventory();
        return;
    }

    const res = await fetch(`/Inventory?handler=GetInvListCat&catId=${encodeURIComponent(catId)}`);
    const list = await res.json();

    console.log(list);

    renderInventoryTable(list);
}

async function loadInvListByProd() {
    const catId = document.getElementById("categoryDropdownInv").value;
    const prodId = document.getElementById("productDropdownInv").value;

    if (!catId || !prodId) {
        loadInvListByCat();
        return;
    }

    const res = await fetch(
        `/Inventory?handler=GetInvListCatProd&catId=${encodeURIComponent(catId)}&prodId=${encodeURIComponent(prodId)}`
    );

    const list = await res.json();
    renderInventoryTable(list);
}

async function loadInventory() {
    const res = await fetch('/Inventory?handler=List');
    const list = await res.json();

    console.log(list);

    renderInventoryTable(list);
}

let inventoryTable;
let currentInventory = [];

document.addEventListener("DOMContentLoaded", () => {
    inventoryTable = new DataTable('#example', {
        responsive: true,
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Export Excel',
                title: 'Inventory',
                exportOptions: {
                    columns: [1, 2, 3, 4] // product, price, stock, date_expired
                }
            }
        ]
    });
});

function renderInventoryTable(list) {
    currentInventory = list
    inventoryTable.clear();

    list.forEach(o => {
        inventoryTable.row.add([
            o.category_name,
            o.product_name,
            o.price,
            o.stock,
            new Date(o.date_expired).toLocaleString('en-US'),
            o.status,
            o.description
        ]);
    });

    inventoryTable.draw();
}

function exportInventoryToExcel(list) {
    const data = list.map(o => ({
        "Product Name": o.product_name,
        "Price": o.price,
        "Stock": o.stock,
        "Date Expired": new Date(o.date_expired).toLocaleDateString('en-US')
    }));

    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = XLSX.utils.book_new();

    XLSX.utils.book_append_sheet(workbook, worksheet, "Inventory");

    XLSX.writeFile(workbook, "Inventory.xlsx");
}


