$ErrorActionPreference = 'Stop'

$serviceName = 'OcWebDashboard'
$displayName = 'OC Web Dashboard'
$baseDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoDir = Split-Path -Parent $baseDir
$exePath = Join-Path $baseDir 'OcWebServiceHost.exe'
$sourcePath = Join-Path $baseDir 'OcWebServiceHost.cs'
$configPath = Join-Path $baseDir 'oc-web-service.json'
$cscPath = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'

$currentIdentity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal($currentIdentity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
  throw 'Please run this script in an elevated PowerShell window.'
}

if (-not (Test-Path $sourcePath)) {
  throw "Service source file not found: $sourcePath"
}

if (-not (Test-Path $configPath)) {
  throw "Service config file not found: $configPath"
}

if (-not (Test-Path $cscPath)) {
  throw "C# compiler not found: $cscPath"
}

& $cscPath /target:exe /out:$exePath /reference:System.ServiceProcess.dll /reference:System.Web.Extensions.dll $sourcePath

if (Get-Service -Name $serviceName -ErrorAction SilentlyContinue) {
  sc.exe stop $serviceName | Out-Null
  Start-Sleep -Seconds 2
  sc.exe delete $serviceName | Out-Null
  Start-Sleep -Seconds 2
}

sc.exe create $serviceName binPath= "`"$exePath`"" start= demand DisplayName= "$displayName" | Out-Null
sc.exe description $serviceName "Runs the opencode multi-auth dashboard on http://127.0.0.1:3434" | Out-Null

Write-Host "Installed service: $displayName ($serviceName)"
Write-Host "Start:   Start-Service $serviceName"
Write-Host "Stop:    Stop-Service $serviceName"
Write-Host "Status:  Get-Service $serviceName"
Write-Host "Logs:    $(Join-Path $baseDir 'oc-web-service.log')"
