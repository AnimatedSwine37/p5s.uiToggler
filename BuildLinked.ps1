# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p5s.uiToggler/*" -Force -Recurse
dotnet publish "./p5s.uiToggler.csproj" -c Release -o "$env:RELOADEDIIMODS/p5s.uiToggler" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location