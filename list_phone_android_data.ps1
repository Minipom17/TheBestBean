$ErrorActionPreference = 'Continue'
$shell = New-Object -ComObject Shell.Application
$thisPc = $shell.NameSpace('shell:MyComputerFolder')
$phone = $null
foreach ($item in $thisPc.Items()) {
    Write-Host ("PC ITEM: " + $item.Name)
    if ($item.Name -match 'S23|Alex|Samsung|Galaxy|Android') { $phone = $item }
}
if (-not $phone) { Write-Host 'NO PHONE FOUND'; exit 0 }

function Get-ChildItems($folderItem) {
    try {
        $ns = $shell.NameSpace($folderItem.Path)
        if (-not $ns) { return @() }
        return @($ns.Items())
    } catch {
        Write-Host ("ERR listing: " + $_.Exception.Message)
        return @()
    }
}

Write-Host ("PHONE: " + $phone.Name)
$storage = $null
foreach ($s in (Get-ChildItems $phone)) {
    Write-Host ("STORAGE: " + $s.Name)
    if ($s.Name -eq 'Internal storage') { $storage = $s }
}
if (-not $storage) { Write-Host 'NO STORAGE'; exit 0 }
Write-Host ("USING STORAGE: " + $storage.Name)

$android = $null
foreach ($f in (Get-ChildItems $storage)) {
    if ($f.Name -eq 'Android') { $android = $f }
}
if (-not $android) {
    Write-Host 'NO Android FOLDER'
    foreach ($f in (Get-ChildItems $storage)) { Write-Host ("ROOT: " + $f.Name) }
    exit 0
}

$data = $null
foreach ($f in (Get-ChildItems $android)) {
    Write-Host ("ANDROID: " + $f.Name)
    if ($f.Name -eq 'data') { $data = $f }
}
if (-not $data) { Write-Host 'NO data FOLDER'; exit 0 }

$all = @(Get-ChildItems $data)
Write-Host ("folder count: " + $all.Count)
Write-Host '--- MATCHES ---'
foreach ($f in $all) {
    $n = [string]$f.Name
    if ($n -match 'santoker|roast|coffee|bean|santo|hong|com\.santoker') {
        Write-Host ("HIT: " + $n)
    }
}
Write-Host '--- READABLE NAMES ---'
foreach ($f in $all) {
    $n = [string]$f.Name
    if ($n -like 'com.*') { Write-Host $n }
}
