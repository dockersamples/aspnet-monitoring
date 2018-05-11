Write-Output '*** Building App'

& docker-compose $config `
    -f ..\docker-compose.yml `
    -f ..\docker-compose-build.yml `
    build