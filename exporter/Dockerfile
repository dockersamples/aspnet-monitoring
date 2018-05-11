# escape=`
FROM microsoft/dotnet-framework-build:4.7.1-windowsservercore-ltsc2016  AS builder

WORKDIR C:\src
COPY src\DockerSamples.AspNetExporter.App\packages.config .
RUN nuget restore packages.config -PackagesDirectory .\packages

COPY src C:\src
RUN msbuild .\DockerSamples.AspNetExporter.App\DockerSamples.AspNetExporter.App.csproj /p:OutputPath=c:\out

# app image
FROM microsoft/windowsservercore:ltsc2016

WORKDIR C:\aspnet-exporter
COPY --from=builder C:\out\ .

ENV COLLECTOR_CONFIG_PATH="aspnet-collectors.json" `
    METRICS_PORT="50505"

ENTRYPOINT ["aspnet-exporter.exe"]