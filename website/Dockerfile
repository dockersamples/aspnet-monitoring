# escape=`
FROM sixeyed/msbuild:netfx-4.5.2-webdeploy-10.0.14393.1480 AS builder

WORKDIR C:\src\DockerSamples.SimpleWebsite.Web
COPY src\DockerSamples.SimpleWebsite.Web\packages.config .
RUN nuget restore packages.config -PackagesDirectory ..\packages

COPY src C:\src
RUN msbuild DockerSamples.SimpleWebsite.Web.csproj /p:OutputPath=c:\out\web\SimpleWebsite `
        /p:DeployOnBuild=true /p:VSToolsPath=C:\MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3\tools\VSToolsPath

# perf counter exporter
FROM dockersamples/aspnet-monitoring-exporter AS exporter

# app image
FROM microsoft/aspnet:windowsservercore-10.0.14393.1480
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Set-ItemProperty -path 'HKLM:\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters' -Name ServerPriorityTimeLimit -Value 0 -Type DWord

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