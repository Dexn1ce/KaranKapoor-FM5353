const BASE_URL = "http://localhost:5119/api/MonteCarloPricer";

async function loadTrades() {
    const response = await fetch(`${BASE_URL}/trade`);
    const trades = await response.json();

    const table = document.querySelector("#tradeTable tbody");
    table.innerHTML = "";

    trades.forEach(t => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${t.tradeId}</td>
            <td>${t.underlyingId}</td>
            <td>${t.marketId}</td>
            <td>${t.direction}</td>
            <td>${t.quantity}</td>
            <td>${t.tradePrice}</td>
            <td>${new Date(t.tradeTime).toLocaleString()}</td>
        `;

        table.appendChild(row);
    });
}

// Add Trade
document.getElementById("addTradeForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const trade = {
        underlyingid: parseInt(document.getElementById("underlyingId").value),
        marketid: parseInt(document.getElementById("marketId").value),
        direction: document.getElementById("direction").value,
        quantity: parseFloat(document.getElementById("quantity").value),
        tradeprice: parseFloat(document.getElementById("tradePrice").value),
        tradetime: new Date().toISOString()  // auto timestamp
    };

    const response = await fetch(`${BASE_URL}/trade`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(trade)
    });

    if (!response.ok) {
        const err = await response.text();
        alert("Failed to place trade: " + err);
        return;
    }

    loadTrades();
});

// Load trades on page start
loadTrades();

