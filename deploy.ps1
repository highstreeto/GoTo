scoop install zip

if (Test-Path dist.zip) { Remove-Item dist.zip }

New-Item -ItemType Directory dist/
Copy-Item Dockerfile-eb dist/Dockerfile

dotnet publish GoTo.Service -o ../dist/app

Push-Location GoTo.Client

ng build --prod
Copy-Item -Force dist/* ../dist/app/wwwroot/

Pop-Location

Push-Location dist/
# Use zip instead of Compress-Archive so path sep. are correct
zip -r ../dist.zip *
Pop-Location

Remove-Item -Recurse dist/
eb deploy
