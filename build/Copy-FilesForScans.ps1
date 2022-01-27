param
(
    [Parameter(Mandatory = $true)]
    [string]$PackagesDir,

    [Parameter(Mandatory = $true)]
    [string]$PackagesFile,

    [Parameter(Mandatory = $true)]
    [string]$DestinationDir
)

[xml]$xml = Get-Content $PackagesFile
$nodes = Select-Xml "//package[@id='Microsoft.Dynamics.CloudAndEdge.ScaleUnitDevTools.Pdbs']" $xml
$packagePath = Join-Path $PackagesDir "$($nodes.node.id).$($nodes.node.version)"
$nugetOutput = Join-Path $packagePath -ChildPath "content\SCMScaleUnitDevToolsPdbs"
if (!(Test-Path $nugetOutput))
{
    throw "$nugetOutput does not exist. Make sure the package has been successfully restored."
}

Get-ChildItem $nugetOutput -Recurse | Copy-Item -Destination {Join-Path $DestinationDir $_.FullName.Substring($SourceDir.length)} -Verbose

