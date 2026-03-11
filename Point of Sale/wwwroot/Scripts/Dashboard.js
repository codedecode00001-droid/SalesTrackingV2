function init() {
    loadSales();
    loadStock();
    loadExpired();
    dashboardData();
}

document.addEventListener("DOMContentLoaded", init);

async function loadSales() {
    const res = await fetch('/Dashboard?handler=ListSales');
    const listsales = await res.json();

    let html = "";
    listsales.forEach(s => {
        html += `
        <tr>
            <td>${s.order_id}</td>
            <td>${new Date(s.datetime).toLocaleString('en-US', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
                hour12: true
            }) }</td>
            <td>${s.users}</td>
            <td style="text-align: right;">${s.total_amount}</td>
        </tr>
        `;
    });
    document.getElementById("listofSalesTable").innerHTML = html;

    // Initialize AFTER data is added
    new DataTable('#example', {
        responsive: true
    });
}

async function loadStock() {
    const res = await fetch('/Dashboard?handler=ListStock');
    const liststock = await res.json();

    let html = "";
    liststock.forEach(c => {
        html += `
        <tr>
            <td>${c.category_name}</td>
            <td>${c.product_name}</td>
            <td>${c.stock}</td>
        </tr>
        `;
    });
    document.getElementById("listofStocksTable").innerHTML = html;

    // Initialize AFTER data is added
    new DataTable('#example1', {
        responsive: true
    });
}

async function loadExpired() {
    const res = await fetch('/Dashboard?handler=ListExpired');
    const listexpired = await res.json();

    let html = "";
    listexpired.forEach(c => {
        html += `
        <tr>
            <td>${c.category_name}</td>
            <td>${c.product_name}</td>
            <td>${new Date(c.date_expired).toLocaleDateString('en-US')}</td>
        </tr>
        `;
    });
    document.getElementById("listofExpiredTable").innerHTML = html;

    // Initialize AFTER data is added
    new DataTable('#example2', {
        responsive: true
    });
}

async function dashboardData() {
    const res = await fetch(`/Dashboard?handler=Details`);
    const pDetails = await res.json();

    document.getElementById("totalproducts").textContent = pDetails.total_product ?? 0;;
    document.getElementById("totalcategory").textContent = pDetails.total_category ?? 0;;
    document.getElementById("no_sales").textContent = pDetails.no_sales ?? 0;;
    document.getElementById("totalrevenue").textContent = pDetails.total_revenue ?? 0;;

    //// If the data is naka List<>
    //if (pDetails.length > 0) {
    //    const d = pDetails[0];

    //    document.getElementById("totalproducts").textContent = d.total_product ?? 0;
    //    document.getElementById("totalcategory").textContent = d.total_category ?? 0;
    //    document.getElementById("no_sales").textContent = d.no_sales ?? 0;
    //    document.getElementById("totalrevenue").textContent = d.total_revenue ?? 0;
    //}
    //// End if the data is naka List<>
}