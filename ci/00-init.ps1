Write-Output '*** Debug info'

& docker $config version
& docker-compose $config version

Write-Output '*** Pulling build & base images'

@('microsoft/windowsservercore:ltsc2016', `
  'microsoft/nanoserver:sac2016', `
  'microsoft/dotnet-framework:4.7.2-sdk', `
  'microsoft/aspnet:4.7.2-windowsservercore-ltsc2016') `
| foreach { & docker $config image pull $_ }