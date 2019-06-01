New-Item -ItemType Directory dist/
Copy-Item Dockerfile-eb dist/Dockerfile

dotnet publish GoTo.Service -o ../dist/app

Push-Location GoTo.Client

npm run build
Copy-Item -Force dist/* ../dist/app/wwwroot/

Pop-Location

Compress-Archive dist/* -DestinationPath dist.zip
Remove-Item -Recurse dist/