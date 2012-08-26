$currentPath = Split-Path $MyInvocation.MyCommand.Path

& "$currentPath\nuget" install NUnit.Runners -Version 2.6.1

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")