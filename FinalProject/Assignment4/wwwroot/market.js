const API_URL = "http://localhost:5119/api/MonteCarloPricer/market"; 

document.addEventListener("DOMContentLoaded", () => {
    loadMarkets();

    document.getElementById("addMarketForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        await addMarket();
    });
});

async function loadMarkets() {
    const response = await fetch(API_URL);
    const markets = await response.json();

    const tableBody = document.querySelector("#marketTable tbody");
    tableBody.innerHTML = "";

    markets.forEach(m => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${m.id}</td>
	    <td>${m.exchangeid}</td>
            <td>${m.name}</td>
            <td>${m.type}</td>
        `;

        tableBody.appendChild(row);
    });
}

async function addMarket() {
   const  eid = document.getElementById("exchangeid").value;
    const name = document.getElementById("name").value;
    const type = document.getElementById("type").value;

    const body = {
	exchangeid :eid,
        name: name,
        type: type
    };

    const response = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        alert("Failed to add market");
        return;
    }

    // Clear form
    document.getElementById("exchangeid").value ="";
    document.getElementById("name").value = "";
    document.getElementById("type").value = "";

    // Refresh table
    loadMarkets();
}

