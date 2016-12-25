param (
	[Parameter(Mandatory = $true)]
	[string]$assemblyInfoPath,
	[Parameter(Mandatory = $true)]
	[string]$packageName,
	[Parameter(Mandatory = $true)]
	[string]$branchName,
	[Parameter(Mandatory = $true)]
	[string]$repoNuGet
)

# Get AssemblyInfo version
$assemblyInfoVersion = Select-String -Path $assemblyInfoPath -Pattern 'AssemblyInformationalVersion\("([0-9]*).([0-9]*).([0-9]*)(.*?)"\)'  | % { $_.Matches }
$assemblyInfoVersion_major = $assemblyInfoVersion.Groups[1].Value
$assemblyInfoVersion_minor = $assemblyInfoVersion.Groups[2].Value
$assemblyInfoVersion_patch = $assemblyInfoVersion.Groups[3].Value

# Get all available package versions
$packageVersions = src\.nuget\NuGet.exe list $packageName -prerelease -source $repoNuGet -allversions

# Get the matching version
$filteredVersions = $packageVersions | Select-String -Pattern "$($assemblyInfoVersion_major).$($assemblyInfoVersion_minor).$($assemblyInfoVersion_patch)-[bB]eta([0-9]*)" -AllMatches | % { $_.Matches }

if($branchName -eq "master")
{
     $nextVersion = $assemblyInfoVersion_major + "." + $assemblyInfoVersion_minor + "." + $assemblyInfoVersion_patch
}
else 
{
    if($filteredVersions.Length -eq 0) {
	
	    $nextVersion = $assemblyInfoVersion_major + "." + $assemblyInfoVersion_minor + "." + $assemblyInfoVersion_patch + "-Beta01"
	
    } 
	else 
	{
	    # Get last version available
	    $lastVersion = $filteredVersions | Sort-Object -Property Groups[1] -desc | Select-Object -first 1 | % { $_.Value }
	
	    # Get next beta available
	    $semanticVersion    = $lastVersion | Select-String -Pattern "([0-9]*).([0-9]*).([0-9]*)-[bB]eta([0-9]*)" | % { $_.Matches }
	    $nextBeta = ([int]$semanticVersion.Groups[4].Value + 1).ToString("00")

	    # Set the next version
	    $nextVersion = $assemblyInfoVersion_major + "." + $assemblyInfoVersion_minor + "." + $assemblyInfoVersion_patch + "-Beta" + $nextBeta
    }
}

# Write next version on console
"Next version is " + $nextVersion

# Replace AssemblyInfo.cs with the newer version ($nextVersion)
[IO.File]::WriteAllLines($assemblyInfoPath, ((Get-Content $assemblyInfoPath) | ForEach-Object { $_ -replace 'AssemblyInformationalVersion\("([0-9]*).([0-9]*).([0-9]*)(.*?)"\)', "AssemblyInformationalVersion(""$nextVersion"")" }))