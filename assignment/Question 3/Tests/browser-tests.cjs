"use strict";
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { pathToFileURL } = require("node:url");
const { chromium } = require(process.env.PLAYWRIGHT_MODULE || "playwright");

const base = path.resolve(__dirname, "..");
const artifacts = path.join(__dirname, "artifacts");
let testsPassed = 0;

async function test(name, action) {
    await action();
    testsPassed += 1;
    console.log("PASS: " + name);
}

async function fillForm(page, name = "René", surname = "O’Connor") {
    await page.getByLabel("First name", { exact: true }).fill(name);
    await page.getByLabel("Surname", { exact: true }).fill(surname);
    await page.getByLabel("Email address", { exact: true }).fill("rene@example.com");
    await page.getByLabel("Preferred sport").selectOption("Netball");
    await page.getByLabel("Membership type").selectOption("Player");
}

(async function () {
    await fs.mkdir(artifacts, { recursive: true });
    const launchOptions = { headless: true, args: ["--no-sandbox"] };
    if (process.env.CHROMIUM_EXECUTABLE) launchOptions.executablePath = process.env.CHROMIUM_EXECUTABLE;
    const browser = await chromium.launch(launchOptions);
    try {
        const context = await browser.newContext({ viewport: { width: 1280, height: 1000 }, acceptDownloads: true });
        const page = await context.newPage();
        const errors = [];
        const externalRequests = [];
        const dialogs = [];
        page.on("pageerror", error => errors.push(error.message));
        page.on("console", message => { if (message.type() === "error") errors.push(message.text()); });
        page.on("request", request => { if (/^https?:/i.test(request.url())) externalRequests.push(request.url()); });
        page.on("dialog", async dialog => { dialogs.push(dialog.message()); await dialog.dismiss(); });
        const url = pathToFileURL(path.join(base, "index.html")).href;
        await page.goto(url);

        await test("Local HTML, CSS, script, logo and navigation load", async () => {
            assert.match(await page.title(), /Membership registration/);
            assert.equal(await page.getByRole("heading", { level: 1 }).textContent(), "Membership registration");
            assert.equal(await page.locator(".brand img").evaluate(image => image.complete && image.naturalWidth > 0), true);
            assert.equal(await page.locator(".header").evaluate(header => getComputedStyle(header).backgroundColor), "rgb(204, 0, 32)");
            assert.equal(await page.getByRole("button", { name: "Review details", exact: true }).isEnabled(), true);
            assert.equal(await page.locator("nav a").evaluateAll(links => links.every(link => document.querySelector(link.getAttribute("href")))), true);
            await page.screenshot({ path: path.join(artifacts, "desktop.png"), fullPage: true });
        });

        await test("Empty required fields block review", async () => {
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("form").evaluate(form => form.checkValidity()), false);
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
        });

        await test("Whitespace-only names are rejected", async () => {
            await fillForm(page, "   ");
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#first-name").evaluate(input => input.validity.customError), true);
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
        });

        await test("Invalid email addresses block review", async () => {
            await fillForm(page);
            await page.getByLabel("Email address", { exact: true }).fill("not-an-email");
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#email").evaluate(input => input.validity.typeMismatch), true);
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
        });

        await test("Valid details are trimmed and shown in a focused review", async () => {
            await fillForm(page, "  René  ", "  O’Connor  ");
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#registration-summary").isVisible(), true);
            assert.equal(await page.locator("#summary-name").textContent(), "René O’Connor");
            assert.equal(await page.locator("#summary-email").textContent(), "rene@example.com");
            assert.equal(await page.locator("#summary-sport").textContent(), "Netball");
            assert.equal(await page.locator("#summary-membership").textContent(), "Player");
            assert.equal(await page.evaluate(() => document.activeElement.id), "registration-summary");
        });

        await test("Download contains the reviewed Unicode details and submission notice", async () => {
            const [download] = await Promise.all([
                page.waitForEvent("download"),
                page.getByRole("button", { name: "Download summary", exact: true }).click()
            ]);
            assert.equal(download.suggestedFilename(), "club-registration.txt");
            assert.equal(await download.failure(), null);
            const text = await fs.readFile(await download.path(), "utf8");
            assert.match(text, /Name: René O’Connor/);
            assert.match(text, /Email: rene@example.com/);
            assert.match(text, /Sport: Netball/);
            assert.match(text, /Membership: Player/);
            assert.match(text, /No application has been sent/);
            assert.match(text, /\r\n/);
            await fs.writeFile(path.join(artifacts, "sample-registration.txt"), text);
        });

        await test("Changing a selection invalidates the old downloadable review", async () => {
            await page.getByLabel("Preferred sport").selectOption("Rugby");
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
            assert.equal(await page.locator("#download-button").isDisabled(), true);
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#summary-sport").textContent(), "Rugby");
        });

        await test("Edit preserves values and returns keyboard focus", async () => {
            await page.getByRole("button", { name: "Edit details", exact: true }).click();
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
            assert.equal(await page.getByLabel("First name", { exact: true }).inputValue(), "René");
            assert.equal(await page.evaluate(() => document.activeElement.id), "first-name");
        });

        await test("Entered markup is displayed as text", async () => {
            await fillForm(page, "<img src=x onerror=alert(1)>", "A & B");
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#summary-name").textContent(), "<img src=x onerror=alert(1)> A & B");
            assert.equal(await page.locator("#summary-name img").count(), 0);
            assert.deepEqual(dialogs, []);
        });

        await test("Clear resets every field and hides the summary", async () => {
            await page.getByRole("button", { name: "Clear form", exact: true }).click();
            for (const selector of ["#first-name", "#surname", "#email", "#sport", "#membership"]) {
                assert.equal(await page.locator(selector).inputValue(), "");
            }
            assert.equal(await page.locator("#registration-summary").isHidden(), true);
            assert.equal(await page.locator("#download-button").isDisabled(), true);
            assert.equal(await page.locator("#download-status").textContent(), "");
        });

        await test("Enter submits a valid form from the keyboard", async () => {
            await fillForm(page);
            await page.getByLabel("Email address", { exact: true }).press("Enter");
            assert.equal(await page.locator("#registration-summary").isVisible(), true);
        });

        await test("Desktop, tablet and narrow mobile layouts do not overflow", async () => {
            await fillForm(page, "A".repeat(60), "B".repeat(60));
            await page.getByLabel("Email address", { exact: true }).fill("longname".repeat(8) + "@example.com");
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            for (const width of [320, 375, 768, 1280]) {
                await page.setViewportSize({ width, height: 900 });
                assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth), true, "Overflow at " + width);
            }
            await page.setViewportSize({ width: 375, height: 900 });
            await fillForm(page);
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            await page.screenshot({ path: path.join(artifacts, "mobile-review.png"), fullPage: true });
            await page.getByRole("button", { name: "Clear form", exact: true }).click();
            await page.screenshot({ path: path.join(artifacts, "mobile.png"), fullPage: true });
        });

        await test("The page works offline without storage or network requests", async () => {
            await context.setOffline(true);
            await page.goto(url);
            await fillForm(page);
            await page.getByRole("button", { name: "Review details", exact: true }).click();
            assert.equal(await page.locator("#registration-summary").isVisible(), true);
            assert.equal(await page.evaluate(() => localStorage.length + sessionStorage.length), 0);
            assert.deepEqual(externalRequests, []);
        });

        await test("Without JavaScript, the limitation is visible and review stays disabled", async () => {
            const noScript = await browser.newContext({ javaScriptEnabled: false });
            const fallback = await noScript.newPage();
            await fallback.goto(url);
            assert.equal(await fallback.locator("noscript .notice").isVisible(), true);
            assert.equal(await fallback.getByRole("button", { name: "Review details", exact: true }).isDisabled(), true);
            await noScript.close();
        });

        await test("No JavaScript errors or unexpected dialogs", async () => {
            assert.deepEqual(errors, []);
            assert.deepEqual(dialogs, []);
        });
        await context.close();
        console.log("PASS: " + testsPassed + " browser scenarios. Screenshots and a sample download are in Tests/artifacts.");
    } finally {
        await browser.close();
    }
})().catch(error => { console.error(error); process.exitCode = 1; });
