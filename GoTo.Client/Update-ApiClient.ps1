$swaggerJson = "/local/GoTo.Service/swagger.json"
$type = "typescript-angular"
$outputDir = "/local/GoTo.Client/src/api-client"

Remove-Item -Recurse src\api-client
docker run --rm -v $pwd\..:/local swaggerapi/swagger-codegen-cli generate --input-spec $swaggerJson -l $type -o $outputDir
