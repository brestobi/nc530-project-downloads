# Question 3: Registration web page

A complete local HTML/CSS/JavaScript page has been built around the supplied stylesheet. The question paper has not yet been supplied. **The sports-club subject, field labels, options, page copy and download behaviour are provisional choices, not requirements recovered from the original files.**

## Open the page

Extract the completed ZIP fully and open `index.html` in a modern browser such as Edge, Chrome or Firefox. Keep `style.css`, `script.js` and `club-mark.svg` in the same folder as `index.html`.

No installation, server, internet access or build command is needed to use the page. The browser needs JavaScript enabled to review details and download a summary. Without JavaScript, the page explains the limitation and leaves the review button disabled.

## What is implemented

- A red club header and local SVG mark, dark navigation, centred registration form and footer.
- Real navigation targets for About, Registration and Help, plus a keyboard skip link.
- First name, surname, email address, preferred sport and membership type fields with associated labels.
- Required-field checks, native email validation, length limits and whitespace-only name validation. Names with accents or apostrophes are supported.
- **Review details** shows a focused summary of the entered information.
- **Download summary** saves `club-registration.txt`, including the reviewed fields and an explicit notice that no application has been sent.
- **Edit details** returns focus to the first-name field without clearing entries.
- **Clear form** resets all fields, validation messages and reviewed details.
- Editing any field invalidates the previous review so an outdated summary cannot be downloaded.
- Responsive layouts for desktop and mobile, visible focus styling and readable button hover states.

All processing happens in the browser. The page makes no network requests, sends no applications and uses no localStorage, sessionStorage or database. Downloading is an explicit action by the person using the form. A receiving organisation would need to arrange its own registration process; this page does not imply an actual club affiliation.

## Files

| File | Purpose |
| --- | --- |
| `index.html` | Page structure, navigation, form, review section and help |
| `style.css` | Supplied visual direction extended for the finished page |
| `script.js` | Validation, review, reset and text-file download |
| `club-mark.svg` | Original local vector mark; no external image dependency |
| `Tests/browser-tests.cjs` | Repeatable Playwright browser scenarios |
| `Tests/package.json` | Development-only browser-test dependency |
| `Tests/artifacts/desktop.png` | Desktop screenshot |
| `Tests/artifacts/mobile.png` | Mobile screenshot |
| `Tests/artifacts/mobile-review.png` | Mobile review screenshot |
| `Tests/artifacts/sample-registration.txt` | Sample download produced by the browser test with fictitious test details |
| `Question 3 - original.zip` | Local backup of the supplied stylesheet, excluded from the completed ZIP |

The original stylesheet's `.container`, `.header`, `.nav`, `.content`, `.form-header` and `.form-container` concepts are retained. Changes include consistent border-box sizing to prevent full-width controls from overflowing, responsive field rows, a complete header declaration, and readable button hover colours. The original white-on-white button hover has been replaced.

## Verification

JavaScript syntax checking passed. All **15 Chromium browser scenarios** passed:

1. Local HTML, CSS, JavaScript, logo and navigation load.
2. Empty required fields block review.
3. Whitespace-only names are rejected.
4. Invalid email addresses block review.
5. Valid input is trimmed and displayed with focus moved to the summary.
6. The actual downloaded text file contains the reviewed Unicode data and submission notice.
7. Changing a selection invalidates the old downloadable review.
8. Editing preserves form values and returns keyboard focus.
9. Entered markup is rendered as text and cannot create elements or execute a script.
10. Clearing resets every field, review and download status.
11. Enter submits a valid form from the keyboard.
12. Layouts at widths of 320, 375, 768 and 1280 pixels do not overflow, including long names and email text.
13. The page works offline without storage or network requests.
14. The no-JavaScript fallback is visible and review remains disabled.
15. No JavaScript errors or unexpected browser dialogs occur.

Desktop and mobile screenshots were also inspected. Browser automation was performed in Chromium; Firefox, Safari and Edge were not independently exercised. These checks do not replace checking the question paper or a full accessibility audit.

## Repeat the browser checks

These commands are for development testing only. They are not needed to open or use the page.

With Node.js and npm installed, from the Question 3 folder:

```sh
cd Tests
npm install
npx playwright install chromium
npm test
```

The tests open the actual `index.html` using a local file URL and save screenshots and a sample text download in `Tests/artifacts`. An existing Playwright installation or Chromium executable can be selected with the `PLAYWRIGHT_MODULE` and `CHROMIUM_EXECUTABLE` environment variables.

## When the paper arrives

Confirm the required page topic, title, logo, navigation destinations, form fields and options, exact appearance, filenames and any required submission or scripting behaviour. Replace the provisional content and adjust the implementation before submitting the assignment. If the task requires only HTML/CSS, the JavaScript behaviour may be beyond its scope.
