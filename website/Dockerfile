# escape=`
FROM microsoft/dotnet-framework:4.7.2-sdk AS build

WORKDIR C:\src\DockerSamples.SimpleWebsite.Web
COPY src\DockerSamples.SimpleWebsite.Web\packages.config .
RUN nuget restore packages.config -PackagesDirectory ..\packages

COPY src C:\src
RUN msbuild DockerSamples.SimpleWebsite.Web.csproj /p:OutputPath=c:\out\web\SimpleWebsite /p:Configuration=Release

# perf counter exporter
FROM dockersamples/aspnet-monitoring-exporter AS exporter

# app image
FROM microsoft/aspnet:4.7.2-windowsservercore-ltsc2016
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Remove-Website -Name 'Default Web Site';`
    New-Item -Path 'C:\web-app' -Type Directory; `
    New-Website -Name 'web-app' -Port 80 -PhysicalPath 'C:\web-app'

WORKDIR C:\aspnet-exporter
COPY --from=exporter C:\aspnet-exporter .
ENV COLLECTOR_CONFIG_PATH="w3svc-collectors.json"

ENTRYPOINT ["powershell"]

CMD Start-Service W3SVC; `
    Invoke-WebRequest http://localhost:8090 -UseBasicParsing | Out-Null; `
    Start-Process -NoNewWindow C:\aspnet-exporter\aspnet-exporter.exe; `
    C:\ServiceMonitor.exe w3svc

COPY --from=builder C:\out\web\SimpleWebsite\_PublishedWebsites\DockerSamples.SimpleWebsite.Web C:\web-app