# escape=`
FROM microsoft/windowsservercore:ltsc2016
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

EXPOSE 3000

ARG GRAFANA_VERSION="5.2.1"
ARG GRAFANA_SHA256="b2a1b29556bc9d949b40ba9f975ea1ccd43471904fc6311e65bdfb42979a2571"

RUN Invoke-WebRequest "https://s3-us-west-2.amazonaws.com/grafana-releases/release/grafana-$($env:GRAFANA_VERSION).windows-amd64.zip"  -OutFile grafana.zip -UseBasicParsing; `
    if ((Get-FileHash grafana.zip -Algorithm sha256).Hash.ToLower() -ne $env:GRAFANA_SHA256) {exit 1}; `
    Expand-Archive grafana.zip -DestinationPath C:\; `
    Move-Item "grafana-$($env:GRAFANA_VERSION)" grafana; `
    Remove-Item grafana.zip

WORKDIR C:\grafana\bin
CMD ["grafana-server.exe"]