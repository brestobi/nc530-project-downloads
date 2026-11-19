# NC530 project downloads

A simple static download page for the Visual Studio 2010 edition of the three NC530 Computer Programming Level 4 Paper 2 projects.

## Contents

- `dist/index.html`: the single download page, with embedded CSS and no JavaScript dependencies.
- `dist/downloads/`: the full assignment ZIP and individual question downloads.
- `assignment/`: editable Visual Basic and website sources, tests and instructions.
- `.github/workflows/pages.yml`: publishes `dist/` to GitHub Pages whenever `main` changes.

The website can also be hosted by any static file host. There is no build command; use `dist` as the publish directory.

## GitHub Pages

Repository: https://github.com/brestobi/nc530-project-downloads

Custom domain configured in GitHub Pages: `me.illdoit.space`.

DNS for `illdoit.space` is managed by Cloudflare. The required subdomain record is:

| Type | Name | Target | Proxy | TTL |
| --- | --- | --- | --- | --- |
| CNAME | me | brestobi.github.io | DNS only | Auto |

The target excludes the repository name. The domain needs this DNS record before it can resolve and GitHub can issue its HTTPS certificate. After issuance, enable Enforce HTTPS in repository Settings > Pages. The workflow deploys a static artifact, so GitHub Pages stores the custom-domain setting in repository configuration rather than a CNAME source file.

In repository Settings > Pages, select **GitHub Actions** as the source if it is not already selected. The included workflow publishes the site and reports its URL in the deployment summary. It can also be started manually from the Actions tab.

## Local preview

```sh
python3 -m http.server 8080 --directory dist
```

Visit `http://localhost:8080` to preview the page.

## Assignment projects

Download the full ZIP, extract it into a new folder, and open `Assignment VS2010.sln` in Visual Studio 2010. The desktop applications target .NET Framework 4.0. Question 2 needs a 32-bit Jet or Access provider. For Question 3, open its `index.html` in a modern browser.

The question paper is still missing. The calculation rules and Question 3 page content are provisional; check each question's README before submission.
