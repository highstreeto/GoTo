if (Test-Path dist/) {
    Remove-Item -Recurse dist/
}

dotnet publish .\GoTo.Service -o ../dist

Push-Location .\GoTo.Client

npm run build
Copy-Item -Force dist/* ../dist/wwwroot/

Pop-Location