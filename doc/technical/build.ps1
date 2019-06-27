$readerProcess = "FoxitReader"

Write-Host "Building technical documentation"

$proc = Get-Process $readerProcess -ErrorAction SilentlyContinue
if ($proc) {
    Write-Host "Closing $readerProcess so file can be overwritten"
    # try gracefully first
    $proc.CloseMainWindow() > $nil
    # kill after five seconds
    Start-Sleep 5
    if (!$proc.HasExited) {
        $proc | Stop-Process -Force
    }
}

docker run -v ${pwd}:/data pandoc/latex -t latex -o technical-doc.pdf index.md

Write-Host -ForegroundColor Green -NoNewline "Done: "
Write-Host "Building technical documentation"

Start-Process technical-doc.pdf