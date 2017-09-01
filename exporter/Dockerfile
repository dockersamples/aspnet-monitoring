# escape=`
FROM sixeyed/msbuild:netfx-4.5.2-10.0.14393.1480 AS builder

WORKDIR C:\src
COPY src\DockerSamples.AspNetExporter.App\packages.config .
RUN nuget restore packages.config -PackagesDirectory .\packages

COPY src C:\src
RUN msbuild .\DockerSamples.AspNetExporter.App\DockerSamples.AspNetExporter.App.csproj /p:OutputPath=c:\out

# app image
FROM microsoft/windowsservercore:10.0.14393.1480

WORKDIR C:\aspnet-exporter
COPY --from=builder C:\out\ .

ENV COLLECTOR_CONFIG_PATH="aspnet-collectors.json" `
    METRICS_PORT="50505"

ENTRYPOINT ["aspnet-exporter.exe"]