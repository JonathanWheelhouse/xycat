# xycat

## xcat

Weakly encrypt a directory's contents; store in a file.

## ycat

Decrypt a weakly encrypted file into its subdirectories and files in a specified directory.

## dotnet

`cd xycat`

`dotnet clean -r linux-x64`

`dotnet build -r linux-x64`

`dotnet test xycat.test/xycat.test.csproj -l "console;verbosity=detailed" --runtime linux-x64 --no-build`



`dotnet publish ycat/ycat.csproj -c Release -o bin/ycat -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=true -r linux-x64 --self-contained true`

`dotnet publish -c Release -r linux-x64`
