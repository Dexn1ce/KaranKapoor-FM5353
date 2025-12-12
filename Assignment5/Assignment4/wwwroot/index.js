const BASE_URL = "http://localhost:5119/api/MonteCarloPricer";

// --- Dynamic Optional Fields ---
document.getElementById("optionType").addEventListener("change", updateOptionalFields);

function updateOptionalFields() {
    const opt = document.getElementById("optionType").value;
    const optional = document.getElementById("optionalFields");
    optional.innerHTML = "";

    if (opt === "barrier") {
        optional.innerHTML = `
            <label>Barrier Level:</label>
            <input type="number" id="barrier" step="0.01">
            <label>Barrier Type:</label>
            <input type="text" id="barrierType">
        `;
    }

    if (opt === "digital") {
        optional.innerHTML = `
            <label>Payout Amount:</label>
            <input type="number" id="payout" step="0.01">
        `;
    }
}

// --- Simulation Request ---
document.getElementById("pricingForm").addEventListener("submit", async (e) => {
    e.preventDefault();

	    const params = new URLSearchParams();

    params.append("OptionType", document.getElementById("optionType").value);
    params.append("S", document.getElementById("spot").value);
    params.append("K", document.getElementById("strike").value);
    params.append("sig", document.getElementById("sigma").value);
    params.append("r", document.getElementById("rate").value);


    params.append("optiontyp", document.getElementById("optiontype").value);
    params.append("b", document.getElementById("costCarry").value);
    params.append("T", document.getElementById("maturity").value);
    params.append("N", document.getElementById("simN").value);
    params.append("M", document.getElementById("simM").value);

    // Boolean parameters
    params.append("useAnt", document.getElementById("useAnt").value);
    params.append("useCont", document.getElementById("useCont").value);
    params.append("isParallel", document.getElementById("isParallel").value);

    // Optional: barrier fields
    if (document.getElementById("barrier")) {
        params.append("barrier", document.getElementById("barrier").value);
        params.append("barrierType", document.getElementById("barrierType").value);
    }

    // Optional: digital payout fields
    if (document.getElementById("payout")) {
        params.append("payout", document.getElementById("payout").value);
    }

    // Pricing URL (GET request)
    const priceUrl = `${BASE_URL}/price?${params.toString()}`;
    const greeksUrl = `${BASE_URL}/price_greeks?${params.toString()}`;

    console.log("PRICE URL:", priceUrl);

    // Call API
    const greekResponse = await fetch(greeksUrl);


    const greekData = await greekResponse.json();

    // Update UI
    document.getElementById("res_price").innerText = greekData.price ?? "—";

    document.getElementById("res_delta").innerText = greekData.delta ?? "—";
    document.getElementById("res_vega").innerText = greekData.vega ?? "—";
    document.getElementById("res_rho").innerText = greekData.rho ?? "—";
    document.getElementById("res_theta").innerText = greekData.theta ?? "—";


});

