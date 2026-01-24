# build_and_deploy.ps1

# This script automates the build and deployment process for the FieldManagementSystem User service.
# It performs the following steps:
# 1. Publishes the .NET User service project to a local 'Builds' directory.
# 2. Builds a new Docker image from the published artifacts.
# 3. Restarts the services defined in the docker-compose.yaml file to use the new image.

# --- Configuration ---
$ProjectName = "FieldManagementSystem.Services.User"
$ProjectFile = "..\Services\User\FieldManagementSystem.Services.User\FieldManagementSystem.User.csproj"
$Dockerfile = "..\Services\User\FieldManagementSystem.Services.User\Dockerfile"
$BuildOutput = ".\Builds\$ProjectName"
$ImageName = "fieldmanagementsystem-user:latest"
$ComposeFile = ".\docker-compose.yaml"
$ComposeInfraFile = ".\docker-compose-infra.yaml"

# --- 1. Publish .NET Project ---
Write-Host "Step 1: Publishing .NET project..."
if (Test-Path $BuildOutput) {
    Remove-Item -Recurse -Force $BuildOutput
}
dotnet publish $ProjectFile -c Release -o $BuildOutput
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish .NET project. Aborting."
    exit 1
}
Write-Host ".NET project published successfully to $BuildOutput"
Write-Host ""

# --- 1.5. Run Tests ---
Write-Host "Step 1.5: Running .NET tests..."
dotnet test ..\Services\FieldManagementSystem.sln
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed. Aborting."
    exit 1
}
Write-Host ".NET tests passed successfully."
Write-Host ""

# --- 2. Build Docker Image ---
Write-Host "Step 2: Building Docker image..."
docker build -t $ImageName -f $Dockerfile $BuildOutput
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build Docker image. Aborting."
    exit 1
}
Write-Host "Docker image '$ImageName' built successfully."
Write-Host ""

# --- 3. Rerun Docker Compose ---
Write-Host "Step 3: Rerunning docker-compose..."

# Remove the shared network if it exists to ensure a clean state
$NetworkName = "fsm_shared_network"
Write-Host "Attempting to remove existing Docker network '$NetworkName' if it exists..."
docker network rm $NetworkName -f | Out-Null # -f to force removal, Out-Null to suppress output if network doesn't exist
# We don't check $LASTEXITCODE here as 'rm' will fail if network doesn't exist, which is fine.



# Bring down services from both compose files to ensure clean restart
Write-Host "Bringing down existing Docker Compose services..."
docker-compose -f $ComposeFile -f $ComposeInfraFile down --remove-orphans
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Failed to bring down Docker Compose services. Continuing with up command."
    # We use Write-Warning here as 'down' might fail if services aren't running, but we still want to try 'up'.
}

# Bring up infrastructure services first
Write-Host "Bringing up infrastructure services (Seq)..."
docker-compose -f $ComposeInfraFile up -d
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to bring up infrastructure services. Aborting."
    exit 1
}

# Bring up the user service
Write-Host "Bringing up user-service..."
docker-compose -f $ComposeFile up -d --force-recreate user-service
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to bring up user-service. Aborting."
    exit 1
}

Write-Host "Docker Compose services have been updated."
Write-Host ""
Write-Host "Deployment complete."
