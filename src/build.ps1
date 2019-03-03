properties {
    $rootNow = Resolve-Path .
    $deployMode = "Release"
    $releaseDir = "$rootNow/build/$deployMode"
}

# default task
Task Default -depends Build

Task Clean -Description "clean last build result" {
    Remove-Item $releaseDir -Force -Recurse -ErrorAction SilentlyContinue
}

Task Init -depends Clean -Description "init some data" {
    New-Item $releaseDir -ItemType Directory
}

Task Nuget -depends Init -Description "nuget restore" {
    Exec {
        dotnet restore
    }
}

Task Build -depends Nuget -Description "build sln" {
    Exec {
        dotnet build -c $deployMode
    }   
}

