# escape=`
FROM microsoft/windowsservercore:ltsc2016 AS installer
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ENV 7ZIP_VERSION="1604"

RUN Invoke-WebRequest "http://7-zip.org/a/7z$($env:7ZIP_VERSION)-x64.msi" -OutFile '7z.msi' -UseBasicParsing; `
    Start-Process msiexec.exe -ArgumentList '/i', '7z.msi', '/quiet', '/norestart' -NoNewWindow -Wait; `
    Remove-Item 7z.msi

ARG PROMETHEUS_VERSION="2.3.1"
ARG PROMETHEUS_SHA256="10ed7d04c34d73f29c07abce1b01fbb885380ab2af26b605d2163601356bf50a"

RUN [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; `
    Write-Host "Downloading Prometheus version: $env:PROMETHEUS_VERSION"; `
    Invoke-WebRequest "https://github.com/prometheus/prometheus/releases/download/v$($env:PROMETHEUS_VERSION)/prometheus-$($env:PROMETHEUS_VERSION).windows-amd64.tar.gz" -OutFile 'prometheus.tar.gz' -UseBasicParsing; `
    if ((Get-FileHash prometheus.tar.gz -Algorithm sha256).Hash.ToLower() -ne $env:PROMETHEUS_SHA256) {exit 1}; `
    & 'C:\Program Files\7-Zip\7z.exe' x prometheus.tar.gz; `
    & 'C:\Program Files\7-Zip\7z.exe' x prometheus.tar; `
    Rename-Item -Path "C:\prometheus-$($env:PROMETHEUS_VERSION).windows-amd64" -NewName 'C:\prometheus'

# Prometheus
FROM microsoft/nanoserver:sac2016

COPY --from=installer /prometheus/prometheus.exe      /bin/prometheus.exe
COPY --from=installer /prometheus/promtool.exe        /bin/promtool.exe
COPY --from=installer /prometheus/prometheus.yml      /etc/prometheus/prometheus.yml
COPY --from=installer /prometheus/console_libraries/  /etc/prometheus/
COPY --from=installer /prometheus/consoles/           /etc/prometheus/
COPY prometheus.yml /etc/prometheus/prometheus.yml

EXPOSE     9090
VOLUME     C:\prometheus

ENTRYPOINT ["C:\\bin\\prometheus.exe", `
            "--storage.tsdb.path=/prometheus", `
            "--web.console.libraries=/etc/prometheus/console_libraries", `
            "--web.console.templates=/etc/prometheus/consoles" ]

CMD ["--config.file=/etc/prometheus/prometheus.yml"]