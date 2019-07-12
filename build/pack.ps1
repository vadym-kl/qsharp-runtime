# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

$ErrorActionPreference = 'Stop'

& "$PSScriptRoot/set-env.ps1"
$all_ok = $True

function Copy-One() {
    param(
        [string] $src,
        [string] $dst
    )

    If (Test-Path $src) { 
        copy $src $dst
    } else {
        Write-Host "##vso[task.logissue type=warning;]Missing file $src"
    }
}

Write-Host "##[info]Copy Native simulator xplat binaries"
Push-Location ../src/Simulation/Native
    If (-not (Test-Path 'osx')) { mkdir 'osx' }
    If (-not (Test-Path 'linux')) { mkdir 'linux' }

    $DROP= (Join-Path $Env:DROP_NATIVE "/src/Simulation/Native/build")
    If (Test-Path $DROP) {
        Copy-One (Join-Path $DROP "libMicrosoft.Quantum.Simulator.Runtime.dylib") (Join-Path "osx" "Microsoft.Quantum.Simulator.Runtime.dll") 
        Copy-One (Join-Path $DROP "libMicrosoft.Quantum.Simulator.Runtime.so") (Join-Path "linux" "Microsoft.Quantum.Simulator.Runtime.dll") 
    } else {
        Write-Host "##vso[task.logissue type=warning;]Missing drop folder with native dlls ($DROP)"
    }
Pop-Location
popd


function Pack-One() {
    Param($project, $include_references="")
    nuget pack $project `
        -OutputDirectory $Env:NUGET_OUTDIR `
        -Properties Configuration=$Env:BUILD_CONFIGURATION `
        -Version $Env:NUGET_VERSION `
        -Verbosity detailed `
        $include_references

    if  ($LastExitCode -ne 0) {
        Write-Host "##vso[task.logissue type=error;]Failed to pack $project"
        $script:all_ok = $False
    }
}


Write-Host "##[info]Using nuget to create packages"
Pack-One '../src/Simulation/CsharpGeneration/Microsoft.Quantum.CsharpGeneration.fsproj' '-IncludeReferencedProjects'
Pack-One '../src/Simulation/Simulators/Microsoft.Quantum.Simulators.csproj' '-IncludeReferencedProjects'
Pack-One '../src/ProjectTemplates/Microsoft.Quantum.ProjectTemplates.nuspec'
Pack-One '../src/Microsoft.Quantum.Development.Kit.nuspec'
Pack-One '../src/Xunit/Microsoft.Quantum.Xunit.csproj'

if (-not $all_ok) 
{
    throw "At least one project failed to pack. Check the logs."
}
