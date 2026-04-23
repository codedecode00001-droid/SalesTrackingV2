document.addEventListener("DOMContentLoaded", function () {
    loadSalesReport();
});

document.getElementById('filterForm').addEventListener('submit', async function (e) {
    e.preventDefault(); // stop page refresh
    await loadSalesDataChange();;   // reuse your existing function
});

async function loadSalesDataChange() {
    const dateFrom = document.getElementById("dateFrom").value;
    const dateTo = document.getElementById("dateTo").value;

    // If dateTo is empty, reload full report
    if (!dateTo) {
        await loadSalesReport();
        return;
    }

    const res = await fetch(
        `/Report?handler=GetSalesList&from=${encodeURIComponent(dateFrom)}&to=${encodeURIComponent(dateTo)}`
    );

    if (!res.ok) {
        console.error("Failed to fetch sales data");
        return;
    }

    const list = await res.json();

    console.log(list);

    renderSalesTable(list);
}

async function loadSalesReport() {
    const res = await fetch('/Report?handler=List');
    const list = await res.json();

    console.log(list);

    renderSalesTable(list);
}

let reportTable;
let currentSales = [];

document.addEventListener("DOMContentLoaded", () => {
    reportTable = new DataTable('#example', {
        responsive: true,
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Export Excel',
                title: 'Sales',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5] // order no, product name, price, amount, total sales
                }
            }
        ]
    });
});

function renderSalesTable(list) {
    currentSales = list
    reportTable.clear();

    list.forEach(o => {

        const orders = o.orderTrans.split(', '); // Get all same order no
        const products = o.product_name.split(', ');
        const prices = o.price.split(', ');
        const amounts = o.amount.split(', ');
        const dates = o.date.split(', ');

        products.forEach((product, index) => {
            reportTable.row.add([
                dates[index],
                orders[index],
               // index === 0 ? o.orderTrans : '',       // Order No only first row
                product,                             // Product
                prices[index],                       // Price
                amounts[index],                      // Amount
                index === 0 ? o.total_sales : ''     // Total Sales only first row
            ]);
        });
    });

    reportTable.draw();
    computeTotalRevenue();
}

function computeTotalRevenue() {
    let total = 0;

    reportTable
        .column(5, { search: 'applied' }) // Total Sales column
        .data()
        .each(value => {
            if (value !== '') {
                total += parseFloat(value.toString().replace(/[₱,]/g, '')) || 0;
            }
        });

    document.getElementById('totalRevenue').innerText =
        `₱ ${total.toLocaleString()}`;
}

document.getElementById("exportExcel").addEventListener("click", exportCurrentSalesToExcel);

// For not undefined the total revenue
function exportCurrentSalesToExcel() {
    const dateFrom = document.getElementById("dateFrom").value;
    const dateTo = document.getElementById("dateTo").value;
    const frequency = document.getElementById("frequencies")?.value || 'All';
    const createdBy = 'Admin';

    // Compute total dynamically
    let totalRevenue = 0;
    reportTable
        .column(5, { search: 'applied' })
        .data()
        .each(value => {
            if (value !== '') {
                totalRevenue += parseFloat(value.toString().replace(/[₱,]/g, '')) || 0;
            }
        });

    exportSalesToExcel(currentSales, dateFrom, dateTo, frequency, createdBy, totalRevenue);
}
// End

// To Total Revenue Export Excel Files
function exportSalesToExcel(list, dateFrom, dateTo, frequency, createdBy, totalRevenue) {
    const excelData = [];

    list.forEach(o => {
        const orders = o.orderTrans.split(', ');
        const products = o.product_name.split(', ');
        const prices = o.price.split(', ');
        const amounts = o.amount.split(', ');
        const dates = o.date.split(', ');

        products.forEach((product, index) => {
            excelData.push({
                "Date": dates[index],
                "Order No.": orders[index],
                "Product Name": product,
                "Price": parseFloat(prices[index].replace(/[₱,]/g, '')) || 0,
                "Amount": parseFloat(amounts[index].replace(/[₱,]/g, '')) || 0,
                "Total Sales": index === 0 ? parseFloat(o.total_sales) : 0
            });
        });
    });

    const workbook = XLSX.utils.book_new();
    const worksheet = XLSX.utils.json_to_sheet(excelData, { skipHeader: true });

    worksheet['!merges'] = [
        { s: { r: 0, c: 0 }, e: { r: 0, c: 5 } }, // A1:F1 Store name
        { s: { r: 1, c: 0 }, e: { r: 1, c: 3 } }, // A2:D2 Date range
        { s: { r: 1, c: 4 }, e: { r: 1, c: 5 } }, // E2:F2 Frequency
        { s: { r: 2, c: 0 }, e: { r: 2, c: 3 } }, // A3:D3 Created By
        { s: { r: 2, c: 4 }, e: { r: 2, c: 5 } }  // E3:F3 Total Revenue
    ];

    XLSX.utils.sheet_add_aoa(worksheet, [
        ["Sari Sari Store Company"],
        [`Date: ${dateFrom || '-'} ${ dateTo || '-'}`, , , , `Frequencies: ${frequency || 'All'}`],
        [`Created By: ${createdBy}`, , , , `Total Revenue: ₱${totalRevenue.toLocaleString()}`],
        [] // empty row before headers
    ], { origin: "A1" });

    XLSX.utils.sheet_add_aoa(worksheet, [
        ["Date", "Order No.", "Product Name", "Price", "Amount", "Total Sales"]
    ], { origin: "A4" });

    XLSX.utils.sheet_add_json(worksheet, excelData, { skipHeader: true, origin: "A5" });

    const range = XLSX.utils.decode_range(worksheet['!ref']);

    // Style header rows (bold + center) and Store name normal bold
    for (let R = 0; R <= 0; ++R) { // Row 0 (Store Name)
        for (let C = 0; C <= 5; ++C) {
            const cellRef = XLSX.utils.encode_cell({ r: R, c: C });
            const cell = worksheet[cellRef];
            if (!cell) continue;
            cell.s = {
                font: { bold: true },
                alignment: { horizontal: "center", vertical: "center" }
            };
        }
    }

    // Rows 1 and 2 - Date, Frequency, Created By, Total Revenue - bold & italic + alignments
    for (let R = 1; R <= 2; ++R) {
        for (let C = 0; C <= 5; ++C) {
            const cellRef = XLSX.utils.encode_cell({ r: R, c: C });
            const cell = worksheet[cellRef];
            if (!cell) continue;

            // Bold & Italic font
            cell.s = cell.s || {};
            cell.s.font = {
                bold: true,
                italic: true,
            };

            // Alignment:
            // Left align for columns A-D (0-3)
            // Right align for columns E-F (4-5)
            cell.s.alignment = {
                horizontal: (C <= 3) ? "left" : "right",
                vertical: "center"
            };
        }
    }

    // Column headers (row 5) - bold + center
    for (let C = 0; C <= 5; ++C) {
        const cellRef = XLSX.utils.encode_cell({ r: 4, c: C });
        const cell = worksheet[cellRef];
        if (!cell) continue;
        cell.s = {
            font: { bold: true },
            alignment: { horizontal: "center", vertical: "center" }
        };
    }

    // Peso currency format for Price, Amount, Total Sales columns (D=3, E=4, F=5) starting from row 6
    for (let R = 5; R <= range.e.r; ++R) {
        [3, 4, 5].forEach(C => {
            const cellRef = XLSX.utils.encode_cell({ r: R, c: C });
            const cell = worksheet[cellRef];
            if (cell) {
                cell.z = '"₱"#,##0.00';
            }
        });
    }

    // Set column widths
    worksheet['!cols'] = [
        { wch: 12 },  // Date
        { wch: 15 },  // Order No.
        { wch: 20 },  // Product Name
        { wch: 12 },  // Price
        { wch: 12 },  // Amount
        { wch: 15 }   // Total Sales
    ];

    // Freeze header row (A6 = first row of table)
    worksheet['!freeze'] = { ySplit: 5 };

        // Highlight Total Revenue row (optional)
        //const totalRowIndex = range.e.r + 1; // row after last data
        //XLSX.utils.sheet_add_aoa(worksheet, [
        //    [`TOTAL REVENUE: ₱${totalRevenue.toLocaleString()}`]
        //], { origin: `A${totalRowIndex + 1}` });
        //worksheet['!merges'].push({ s: { r: totalRowIndex, c: 0 }, e: { r: totalRowIndex, c: 5 } });

    XLSX.utils.book_append_sheet(workbook, worksheet, "Sales");
    XLSX.writeFile(workbook, "Sales.xlsx");
}