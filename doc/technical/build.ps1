Write-Host "Building technical documentation"

docker run -v ${pwd}:/data pandoc/latex -t latex -o technical-doc.pdf index.md

Write-Host -ForegroundColor Green -NoNewline "Done: "
Write-Host "Building technical documentation"