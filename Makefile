build:
	dotnet build --property WarningLevel=0 --nologo -v q /clp:ErrorsOnly -c Release

run: build
	dotnet run --no-build -c Release