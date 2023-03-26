dotnet build hgs

KSP=~/KSP_osx/GameData/HypergolSystems

rm -rf $KSP

mkdir $KSP
mkdir $KSP/Plugins

cp ./hgs/bin/Debug/net45/hgs.dll $KSP/Plugins/HypergolSystems.dll
cp -rv ./hgs/src/config/* $KSP/