const API_URL = "http://localhost:5119/api/MonteCarloPricer/exchange";  

document.addEventListener("DOMContentLoaded", () => {
    loadExchanges();

    document.getElementById("addExchangeForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        await addExchange();
    });
});

async function loadExchanges() {
    const response = await fetch(API_URL);
    const exchanges = await response.json();

    const tableBody = document.querySelector("#exchangeTable tbody");
    tableBody.innerHTML = "";

    exchanges.forEach(ex => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${ex.id}</td>
            <td>${ex.name}</td>
            <td>${ex.country}</td>
            <td>${new Date(ex.createDate).toLocaleString()}</td>

            <td>${new Date(ex.updateDate).toLocaleString()}</td>
        `;

        tableBody.appendChild(row);
    });
}

async function addExchange() {
    const name = document.getElementById("name").value;
    const country = document.getElementById("country").value;

    const body = {
        name: name,
        country: country
    };

    const response = await fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        alert("Error adding exchange");
        return;
    }

    // Clear form
    document.getElementById("name").value = "";
    document.getElementById("country").value = "";

    // Reload table
    loadExchanges();
}

