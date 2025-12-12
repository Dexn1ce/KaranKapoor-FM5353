const API_URL = "http://localhost:5119/api/MonteCarloPricer/underlying";

document.addEventListener("DOMContentLoaded", () => {
    loadUnderlyings();

    document.getElementById("addUnderlyingForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        await addUnderlying();
    });
});

async function loadUnderlyings() {
    const response = await fetch(API_URL);
    const underlyings = await response.json();

    const tableBody = document.querySelector("#underlyingTable tbody");
    tableBody.innerHTML = "";

    underlyings.forEach(u => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${u.underlyingid}</td>
            <td>${u.symbol}</td>
            <td>${u.name}</td>
            <td>${u.assettype}</td>
        `;

        tableBody.appendChild(row);
    });
}

async function addUnderlying() {
    const symbol = document.getElementById("symbol").value;
    const name = document.getElementById("name").value;


    const assettyp = document.getElementById("assettype").value;

    const body = {
        symbol: symbol,
        name: name,
	assettyp : assettyp
    };

    const response = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        alert("Failed to add underlying");
        return;
    }

    // Clear input
    document.getElementById("symbol").value = "";
    document.getElementById("name").value = "";


    document.getElementById("assettyp").value = "";

    // Reload table
    loadUnderlyings();
}

