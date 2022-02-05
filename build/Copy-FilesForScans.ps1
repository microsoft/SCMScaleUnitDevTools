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

Copy-Item $nugetOutput\* -Destination $DestinationDir -Recurse -Verbose

# Unable to find private pdbs, so we need to exclude this file from scanning.
Remove-Item $DestinationDir\Microsoft.DiaSymReader.Native.amd64.dll

