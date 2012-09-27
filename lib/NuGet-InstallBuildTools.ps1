$currentPath = Split-Path $MyInvocation.MyCommand.Path

& "$currentPath\nuget" update -self
& "$currentPath\nuget" install MSBuildTasks
& "$currentPath\nuget" install NUnit.Runners

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")