const BASE_URL = "http://localhost:5119/api/MonteCarloPricer";

async function loadRatePoints() {
    const curveDate = document.getElementById("curveIdInput").value;

    if (!curveDate) {
        alert("Enter a rate curve date.");
        return;
    }

    try {
        const response = await fetch(`${BASE_URL}/ratecurve/${curveDate}/ratepoints`);

        if (!response.ok) {
            alert("No rate curve found for this date. Try a different date.");
            return;
        }

        const points = await response.json();   // API returns an ARRAY
        console.log("Rate points:", points);

        if (!Array.isArray(points.ratePoints) || points.ratePoints.length === 0) {
            alert("No rate points available for this date.");
            document.querySelector("#ratePointsTable tbody").innerHTML = "";
            return;
        }

        const table = document.querySelector("#ratePointsTable tbody");
        table.innerHTML = "";

        points.ratePoints.forEach(p => {
            const row = document.createElement("tr");

            row.innerHTML = `
                <td>${p.ratePointId ?? p.ratepointid}</td>
                <td>${p.ratecurveid}</td>
                <td>${p.tenor}</td>
                <td>${p.rate}</td>
            `;

            table.appendChild(row);
        });

    } catch (err) {
        console.error("Error:", err);
        alert("Could not load rate points. Check API or try again.");
    }
}

// Add new rate point
/*document.getElementById("addRatePointForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const newPoint = {
        ratecurveid: parseInt(document.getElementById("newCurveId").value),
        tenor: parseFloat(document.getElementById("newTenor").value),
        rate: parseFloat(document.getElementById("newRate").value)
    };

    const response = await fetch(`${BASE_URL}/ratepoints`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newPoint)
    });

    if (!response.ok) {
        alert("Failed to add rate point");
        return;
    }

    alert("Rate point added successfully!");
    loadRatePoints();
});*/

