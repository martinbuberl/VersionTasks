$currentPath = Split-Path $MyInvocation.MyCommand.Path

& "$currentPath\nuget" install NUnit.Runners
& "$currentPath\nuget" install MSBuildTasks

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")