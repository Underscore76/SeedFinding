build:
	dotnet build --property WarningLevel=0 --nologo -v q /clp:ErrorsOnly -c Release

build-debug:
	dotnet build --property WarningLevel=0 --nologo -v q /clp:ErrorsOnly -c Debug

run: build
	dotnet run --no-build -c Release

debug: build-debug
	dotnet run --no-build -c Debug

nettrace:
	dotnet-trace collect --name SeedFinding --format speedscope