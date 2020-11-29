#!/bin/bash
if [ "$OSTYPE" == "msys" -o "$OSTYPE" == "cygwin" ] ; then
	echo -e "\e[37m[Building executable...]\e[2m"
	dotnet publish ./src/computorv1.csproj -r win-x86 -c Release -o ./ -p:PublishSingleFile=True --self-contained True --nologo
	echo -e "\e[1;92m[Build complete!]\e[0m"
elif [[ "$OSTYPE" == "linux-gnu"* ]] ; then
	echo -e "\e[37m[Building executable...]\e[2m"
	dotnet publish ./src/computorv1.csproj -r linux-x64 -c Release -o ./ -p:PublishSingleFile=True --self-contained True --nologo
	echo -e "\e[1;92m[Build complete!]\e[0m"
elif [[ "$OSTYPE" == "darwin"* ]] ; then
	echo -e "\033[37m[Building executable...]\033[2m"
	dotnet publish ./src/computorv1.csproj -r osx-x64 -c Release -o ./ -p:PublishSingleFile=True --self-contained True --nologo
	echo -e "\033[1;92m[Build complete!]\033[0m"
else
	echo "Can't recognize OSTYPE. Try to build manually or download already build executable!"
fi
