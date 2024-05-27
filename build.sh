dotnet build mod

KSP=~/KSP_osx/GameData/HypergolicSystems

rm -rf $KSP
rm -f ~/KSP_osx/KSP.log

mkdir $KSP
mkdir $KSP/Plugins

cp ./mod/bin/Debug/net45/mod.dll $KSP/Plugins/Hgs.Mod.dll
cp -rv ./mod/config/* $KSP/
