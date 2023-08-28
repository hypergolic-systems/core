dotnet build mod

KSP=~/KSP_osx/GameData/HypergolicSystems

rm -rf $KSP

mkdir $KSP
mkdir $KSP/Plugins

cp ./core/bin/Debug/net45/core.dll $KSP/Plugins/Hgs.Core.dll
cp ./mod/bin/Debug/net45/mod.dll $KSP/Plugins/Hgs.Mod.dll
cp -rv ./mod/config/* $KSP/