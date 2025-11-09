param([string]$Scenario = "", [string]$Case = "", [switch]$Help)
if ($Help) { "Usage: .\run-tests.ps1 [-Scenario Sample] [-Case MSTest|NUnit|xUnit] [-Help]"; exit }
# Validate inputs
if ($Case -and $Case -notin @("MSTest","NUnit","xUnit")) { Write-Host "❌ Invalid Case: '$Case'. Use: MSTest, NUnit, or xUnit" -ForegroundColor Red; exit }
if ($Scenario -and $Scenario -ne "Sample") { Write-Host "❌ Invalid Scenario: '$Scenario'. Use: Sample" -ForegroundColor Red; exit }
$filter = @(); if($Scenario) { $filter += "FullyQualifiedName~$Scenario" }; if($Case) { $filter += "DisplayName~$Case" }
$cmd = "dotnet test --settings playwright.runsettings --logger `"console;verbosity=quiet`""; if($filter) { $cmd += " --filter `"$($filter -join '&')`"" }
Invoke-Expression $cmd