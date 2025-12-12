const BASE_URL = "http://localhost:5119/api/MonteCarloPricer";

async function loadPrices() {
    const underlyingId = document.getElementById("underlyingIdInput").value;

    if (!underlyingId) {
        alert("Please enter an Underlying ID");
        return;
    }

    const response = await fetch(`${BASE_URL}/underlying/${underlyingId}/prices`);

    const underlying = await response.json();
    const prices = underlying.prices || [];
    
    console.log("API Response:", underlying);

    const tableBody = document.querySelector("#priceTable tbody");
    tableBody.innerHTML = "";

    prices.forEach(p => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${p.historicalpriceid}</td>
            <td>${p.underlyingid}</td>
            <td>${new Date(p.priceTime).toLocaleString()}</td>
            <td>${p.lastPrice}</td>
        `;
        tableBody.appendChild(row);
    });
}

document.getElementById("addPriceForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const underlyingId = document.getElementById("underlyingid").value;
    const priceTime = document.getElementById("priceTime").value;
    const lastPrice = document.getElementById("lastPrice").value;

    if (!underlyingId) {
        alert("Enter Underlying ID before adding prices.");
        return;
    }

    const body = {
        pricetime: priceTime,
        lastprice: parseFloat(lastPrice)
    };

    const response = await fetch(`${BASE_URL}/underlying/${underlyingId}/prices`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        alert("Failed to add historical price.");
        return;
    }

    // reload table
    loadPrices();
});

