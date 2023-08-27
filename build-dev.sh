dotnet build dev

KSP=~/KSP_osx/GameData/HypergolicSystems

rm -rf $KSP

mkdir $KSP
mkdir $KSP/Plugins

cp ./dev/bin/Debug/net45/dev.dll $KSP/Plugins/HgsDebug.dll