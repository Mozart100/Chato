# Define variables
$ContainerName = "seq"
$SeqLogsPath = "C:\MyDevelopment\Chatto\SeqLogs"

# Stop and remove the existing container if it exists
if ($(docker ps -a -q -f name=$ContainerName)) {
    Write-Host "Stopping and removing existing container: $ContainerName"
    docker stop $ContainerName
    docker rm $ContainerName
}

# Remove the SeqLogs directory if it exists
if (Test-Path $SeqLogsPath) {
    Write-Host "Removing directory: $SeqLogsPath"
    Remove-Item -Path $SeqLogsPath -Recurse -Force
}

# Recreate the Seq container
Write-Host "Starting new Seq container..."
docker run -d --restart unless-stopped --name $ContainerName -e ACCEPT_EULA=Y -v $SeqLogsPath:/data -p 8090:80 datalust/seq:latest

Write-Host "Seq container is running at http://localhost:8090"
