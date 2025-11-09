# PWCli - Playwright Test Framework

## PowerShell Runner

```powershell
.\Commands\run-tests.ps1 -Help
.\Commands\run-tests.ps1
.\Commands\run-tests.ps1 -Scenario Sample
.\Commands\run-tests.ps1 -Case MSTest
.\Commands\run-tests.ps1 -Case NUnit
.\Commands\run-tests.ps1 -Case xUnit
.\Commands\run-tests.ps1 -Scenario Sample -Case xUnit
```

## dotnet CLI

```bash
dotnet test --settings Core\Settings\playwright.runsettings
dotnet test --settings Core\Settings\playwright.runsettings --filter "FullyQualifiedName~Sample"
dotnet test --settings Core\Settings\playwright.runsettings --filter "DisplayName~MSTest"
dotnet test --settings Core\Settings\playwright.runsettings --filter "DisplayName~NUnit"
dotnet test --settings Core\Settings\playwright.runsettings --filter "DisplayName~xUnit"
dotnet test --settings Core\Settings\playwright.runsettings --filter "FullyQualifiedName~Sample&DisplayName~xUnit"
```

## Execution Modes

```bash
dotnet test --settings Core\Settings\playwright.runsettings
$env:TEST_MODE="PARALLEL"; dotnet test --settings Core\Settings\playwright.runsettings
$env:HEADED="0"; dotnet test --settings Core\Settings\playwright.runsettings
$env:BROWSER_SLOWMO="1000"; dotnet test --settings Core\Settings\playwright.runsettings
```

## Test Cases
- MSTest
- NUnit  
- xUnit

## Scenarios
- Sample