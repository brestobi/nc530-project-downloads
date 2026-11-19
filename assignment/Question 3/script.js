"use strict";

const form = document.getElementById("registration-form");
const firstName = document.getElementById("first-name");
const surname = document.getElementById("surname");
const summary = document.getElementById("registration-summary");
const downloadButton = document.getElementById("download-button");
const downloadStatus = document.getElementById("download-status");
let reviewedDetails = null;

function clearReview() {
    reviewedDetails = null;
    summary.hidden = true;
    downloadButton.disabled = true;
    downloadStatus.textContent = "";
}

function validateName(input, message) {
    input.setCustomValidity(input.value.trim() ? "" : message);
}

form.addEventListener("submit", function (event) {
    event.preventDefault();
    firstName.value = firstName.value.trim();
    surname.value = surname.value.trim();
    validateName(firstName, "Enter your first name.");
    validateName(surname, "Enter your surname.");
    if (!form.reportValidity()) return;

    reviewedDetails = {
        name: firstName.value + " " + surname.value,
        email: document.getElementById("email").value.trim(),
        sport: document.getElementById("sport").value,
        membership: document.getElementById("membership").value
    };
    // textContent keeps entered text as text, including names with < or >.
    document.getElementById("summary-name").textContent = reviewedDetails.name;
    document.getElementById("summary-email").textContent = reviewedDetails.email;
    document.getElementById("summary-sport").textContent = reviewedDetails.sport;
    document.getElementById("summary-membership").textContent = reviewedDetails.membership;
    downloadButton.disabled = false;
    summary.hidden = false;
    summary.focus();
});

form.addEventListener("input", function (event) {
    if (event.target === firstName || event.target === surname) {
        event.target.setCustomValidity("");
    }
    clearReview();
});
form.addEventListener("change", clearReview);

form.addEventListener("reset", function () {
    firstName.setCustomValidity("");
    surname.setCustomValidity("");
    clearReview();
    firstName.focus();
});

document.getElementById("edit-button").addEventListener("click", function () {
    clearReview();
    firstName.focus();
});

downloadButton.addEventListener("click", function () {
    if (!reviewedDetails) return;
    const text = [
        "COMMUNITY SPORTS CLUB",
        "Registration summary",
        "",
        "Name: " + reviewedDetails.name,
        "Email: " + reviewedDetails.email,
        "Sport: " + reviewedDetails.sport,
        "Membership: " + reviewedDetails.membership,
        "",
        "No application has been sent. Give this summary to the club to complete registration.",
        ""
    ].join("\r\n");
    const blob = new Blob([text], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = "club-registration.txt";
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
    downloadStatus.textContent = "Save the summary and give it to the club to complete your registration.";
});

// No storage or network request is made; reviewing and downloading happen locally.
document.getElementById("review-button").disabled = false;
