# escape=`
FROM microsoft/dotnet-framework:4.7.2-sdk-windowsservercore-ltsc2019 AS builder

WORKDIR C:\src\DockerSamples.SimpleWebsite.Web
COPY src\DockerSamples.SimpleWebsite.Web\packages.config .
RUN nuget restore packages.config -PackagesDirectory ..\packages

COPY src C:\src
RUN msbuild DockerSamples.SimpleWebsite.Web.csproj /p:OutputPath=c:\out /p:Configuration=Release

# app image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ENV APP_ROOT=C:\web-app

WORKDIR ${APP_ROOT}
RUN Remove-Website -Name 'Default Web Site';`
    New-Website -Name 'web-app' -Port 80 -PhysicalPath $env:APP_ROOT

WORKDIR C:\aspnet-exporter
COPY --from=dockersamples/aspnet-monitoring-exporter:4.7.2-windowsservercore-ltsc2019 C:\aspnet-exporter .
ENV COLLECTOR_CONFIG_PATH="w3svc-collectors.json"

ENTRYPOINT ["powershell"]

CMD Start-Service W3SVC; `
    Invoke-WebRequest http://localhost -UseBasicParsing | Out-Null; `
    Start-Process -NoNewWindow C:\aspnet-exporter\aspnet-exporter.exe; `
    C:\ServiceMonitor.exe w3svc

COPY --from=builder C:\out\_PublishedWebsites\DockerSamples.SimpleWebsite.Web C:\web-app