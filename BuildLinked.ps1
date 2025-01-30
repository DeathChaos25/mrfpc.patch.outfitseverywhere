# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/mrfpc.patch.outfitseverywhere/*" -Force -Recurse
dotnet publish "./mrfpc.patch.outfitseverywhere.csproj" -c Release -o "$env:RELOADEDIIMODS/mrfpc.patch.outfitseverywhere" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location