# xycat

![.NET](https://github.com/JonathanWheelhouse/xycat/workflows/.NET%20Core/badge.svg)

## xcat

Weakly encrypt a directory's contents; store in a file.

## ycat

Decrypt a weakly encrypted file into its subdirectories and files in a specified directory.

## msbuild

See build.proj

`dotnet msbuild build.proj -t:clean,build,test -v:n`

## dotnet

`cd xycat`

`dotnet clean -r linux-x64`

`dotnet build -r linux-x64`

`dotnet test xycat.test/xycat.test.csproj -l "console;verbosity=detailed" --runtime linux-x64 --no-build`


`dotnet publish xcat -r linux-x64  -o publish/linux-x64   -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true --self-contained true`
`dotnet publish ycat -r linux-x64 -c Release -o bin/ycat  -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true --self-contained true`

`dotnet publish -c Release -r linux-x64`
