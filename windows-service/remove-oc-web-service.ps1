$ErrorActionPreference = 'Stop'

$serviceName = 'OcWebDashboard'

$currentIdentity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($currentIdentity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
  throw 'Please run this script in an elevated PowerShell window.'
}

if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
  sc.exe stop $serviceName | Out-Null
  Start-Sleep -Seconds 2
  sc.exe delete $serviceName | Out-Null
  Write-Host "Removed service: $serviceName"
} else {
  Write-Host "Service not found: $serviceName"
}
