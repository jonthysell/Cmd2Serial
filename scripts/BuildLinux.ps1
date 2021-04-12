param()

[string] $Product = "Cmd2Serial"
[string] $Target = "Linux"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-target:Publish -p:RuntimeIdentifier=linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target
