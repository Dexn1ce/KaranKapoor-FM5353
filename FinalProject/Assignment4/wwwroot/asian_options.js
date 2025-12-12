
const BASE_URL = "http://localhost:5119/api/MonteCarloPricer";

// Load All Options
async function loadOptions() {
    const response = await fetch(`${BASE_URL}/asian_option`);
    const options = await response.json();

    renderOptions(options);
}

// Load Option By ID
async function loadOptionById() {
    const id = document.getElementById("optionIdInput").value;

    if (!id) {
        alert("Please enter an Option ID");
        return;
    }

    const response = await fetch(`${BASE_URL}/asian_option`);
    const option = await response.json();

    // Convert single option → array
    renderOptions([option]);
}

// Render Table
function renderOptions(options) {
    const tableBody = document.querySelector("#optionsTable tbody");
    tableBody.innerHTML = "";

    options.forEach(o => {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${o.id}</td>
            <td>${o.underlyingid}</td>
            <td>${o.ratecurveid}</td>
            <td>${o.k}</td>
            <td>${o.expdate}</td>
            <td>${o.otyp}</td>
	    <td>${o.sig}</td>
	    <td>${o.b}</td>
        `;
        tableBody.appendChild(row);
    });
}

// Add New Option
document.getElementById("addOptionForm").addEventListener("submit", async (e) => {
    e.preventDefault();


    const option = {
        underlyingid: parseInt(document.getElementById("underlyingId").value),
        k: parseFloat(document.getElementById("strike").value),
        t: new Date(document.getElementById("expiry").value).toISOString(),
        optiontype: document.getElementById("optionType").value,
	b: document.getElementById("b").value,
	sig:document.getElementById("vol").value,

	ratecurveid:0,
	barrierlevel:0,
	barriertype:"none",
	payoutamount:0

    };

    const response = await fetch(`${BASE_URL}/asian_option`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(option)
    });

    if (!response.ok) {
        alert("Failed to add option");
        return;
    }

    loadOptions();
});



async function priceOption() {
    const id = document.getElementById("priceOptionId").value;
    const N = document.getElementById("simN").value;
    const M = document.getElementById("simM").value;
    const simDate = document.getElementById("simDate").value;
    const useAnt = document.getElementById("useAnt").checked;
    const useCont = document.getElementById("useCont").checked;
    const isParallel = document.getElementById("isParallel").checked;

    if (!id) {
        alert("Please enter an Option ID.");
        return;
    }

    // Convert simDate to ISO if provided
    let simDateParam = simDate ? new Date(simDate).toISOString() : "";

    const url =
        `http://localhost:5119/api/MonteCarloPricer/price_asian_options/${id}` +
        `?N=${N}` +
        `&M=${M}` +
        (simDateParam ? `&simDate=${encodeURIComponent(simDateParam)}` : "") +
        `&useAnt=${useAnt}` +
        `&useCont=${useCont}` +
        `&isParallel=${isParallel}`;

    console.log("Pricing URL:", url);

    document.getElementById("priceResult").innerText = "Running simulation...";

    try {
        const response = await fetch(url, {
            method: "GET"
        });

        if (!response.ok) {
            const msg = await response.text();
            document.getElementById("priceResult").innerText = "Error: " + msg;
            return;
        }

        const result = await response.json();

        // If backend returns a raw value or { price: X }
	    //
	document.getElementById("r_mean").innerText = result.mean.toFixed(6);
        document.getElementById("r_stderr").innerText = result.stderr.toFixed(6);
        document.getElementById("r_delta").innerText = result.delta.toFixed(6);
        document.getElementById("r_vega").innerText = result.vega.toFixed(6);
        document.getElementById("r_rho").innerText = result.rho.toFixed(6);
        document.getElementById("r_theta").innerText = result.theta.toFixed(6);
    } catch (err) {
        document.getElementById("priceResult").innerText =
            "Failed: " + err.message;
    }
}
