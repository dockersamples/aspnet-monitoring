# escape=`
FROM mcr.microsoft.com/windows/servercore:ltsc2019 AS installer
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ARG 7ZIP_VERSION="1604"

RUN Invoke-WebRequest "http://7-zip.org/a/7z$($env:7ZIP_VERSION)-x64.msi" -OutFile '7z.msi' -UseBasicParsing; `
    Start-Process msiexec.exe -ArgumentList '/i', '7z.msi', '/quiet', '/norestart' -NoNewWindow -Wait

ARG PROMETHEUS_VERSION="2.3.1"
ARG PROMETHEUS_SHA256="10ed7d04c34d73f29c07abce1b01fbb885380ab2af26b605d2163601356bf50a"

RUN [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; `
    Write-Host "Downloading Prometheus version: $env:PROMETHEUS_VERSION"; `
    Invoke-WebRequest "https://github.com/prometheus/prometheus/releases/download/v$($env:PROMETHEUS_VERSION)/prometheus-$($env:PROMETHEUS_VERSION).windows-amd64.tar.gz" -OutFile 'prometheus.tar.gz' -UseBasicParsing; `
    if ((Get-FileHash prometheus.tar.gz -Algorithm sha256).Hash.ToLower() -ne $env:PROMETHEUS_SHA256) {exit 1}

RUN & 'C:\Program Files\7-Zip\7z.exe' x prometheus.tar.gz; `
    & 'C:\Program Files\7-Zip\7z.exe' x prometheus.tar; `
    Rename-Item -Path "C:\prometheus-$($env:PROMETHEUS_VERSION).windows-amd64" -NewName 'C:\prometheus'

# Prometheus
# Nano Server 1809 can't be used - `Failed to load netapi32.dll: The specified module could not be found.`
# FROM mcr.microsoft.com/windows/nanoserver:1809
FROM mcr.microsoft.com/windows/servercore:ltsc2019

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