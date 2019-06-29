Write-Host "Building slides"

if (!(Test-Path "reveal.js")) {
    Write-Host "Downloading reveal.js 3.8.0"

    Start-BitsTransfer -Source "https://github.com/hakimel/reveal.js/archive/3.8.0.zip" -Destination reveal.js.zip
    Expand-Archive reveal.js.zip -DestinationPath .
    Remove-Item reveal.js.zip
    Move-Item reveal.js-* reveal.js

    Write-Host -ForegroundColor Green -NoNewline "Done: "
    Write-Host "Downloading reveal.js 3.8.0"
    New-Item reveal.js/lib/js/head.min.js
}

docker run -v ${pwd}:/data pandoc/latex --slide-level=2 -s --self-contained -t revealjs -o slides.html index.md

Write-Host -ForegroundColor Green -NoNewline "Done: "
Write-Host "Building slides"