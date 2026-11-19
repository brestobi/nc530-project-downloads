# NC530 project download page

A single static page serving the Visual Studio 2010 edition of the three assignment projects.

- `dist/index.html`: simple download page, with embedded CSS and no JavaScript or external assets.
- `dist/downloads/`: complete package and individual question ZIPs.
- `assignment/`: editable assignment sources, including `Assignment VS2010.sln`.
- `.openai/hosting.json`: Sites identity and static-directory configuration.

The public site is intended to let anyone with the link download the package. The missing question paper still needs to confirm the assignment requirements; each question includes its assumptions in its README.

To preview locally from this repository, run `python3 -m http.server 8080 --directory dist`, then visit the printed local address. No build or dependency installation is required.

Git tracks the website, download releases and editable assignment sources. Generated Visual Studio build output and IDE caches are ignored.
