[CmdletBinding(SupportsShouldProcess=$true)]
param(
[Parameter(HelpMessage="The current configuration.  This script will only run in DEBUG")]
[string] $configuration,
[Parameter(HelpMessage="The project name. ")]
[string] $projectName,
[Parameter(HelpMessage="Local Nuget Gallery.  Pass null to skip this step")]
[string] $nugetPath

)

if( $configuration -eq "DEBUG"  -or $configuration -eq "debug") {
	if( [string]::IsNullOrEmpty($nugetPath) -eq $false) {
		Write-Host "Deploying Package To Local Nuget Share $nugetPath"

		$path = $env:userprofile
		$path = [io.path]::combine($path,".nuget", "packages", $projectName)
		if( Test-Path $path) {
			Write-Host "Deleting cached Nuget Folder $path"
			Remove-Item $path -Recurse
		}

		dotnet pack --no-build --no-restore --configuration $configuration -o $nugetPath
	}
} else {
	Write-Host "Configuration Is Not Debug.  PostCompile step will be skipped."
}
