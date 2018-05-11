Write-Output '*** Pushing Images'

$images = "dockersamples/aspnet-monitoring-exporter", `
          "dockersamples/aspnet-monitoring-website", `
          "dockersamples/aspnet-monitoring-prometheus"

foreach ($image in $images) {

    $versionedImage = $image + ":$env:BUILD_NUMBER"
    & docker $config image push $versionedImage

    & docker $config image tag $versionedImage $image
    & docker $config image push $image
}