Write-Output '*** Pushing Images'

$services = 'signup-app', 'signup-save-handler', 'signup-index-handler', 'signup-homepage'

foreach ($service in $services) {

    & docker-compose $config `
        -f ..\docker-compose.yml `
        -f ..\docker-compose-build.yml `
        push $service
}