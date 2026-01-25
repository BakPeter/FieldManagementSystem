# build_and_deploy.ps1

# This script automates the build and deployment process for the FieldManagementSystem User service.
# It performs the following steps:
# 1. Publishes the .NET User service project to a local 'Builds' directory.
# 2. Builds a new Docker image from the published artifacts.
# 3. Restarts the services defined in the docker-compose.yaml file to use the new image.

# --- Configuration ---
$ComposeFile = ".\docker-compose.yaml"
$ComposeInfraFile = ".\docker-compose-infra.yaml"
$NetworkName = "fsm_shared_network"
$BuildRoot = ".\Builds"

$projects = @(
    @{
        ProjectName = "FieldManagementSystem.Services.User"
        ProjectFile = "..\Services\User\FieldManagementSystem.Services.User\FieldManagementSystem.Services.User.csproj"
        Dockerfile = "..\Services\User\FieldManagementSystem.Services.User\Dockerfile"
        BuildOutput = Join-Path $BuildRoot "FieldManagementSystem.Services.User"
        ImageName = "fieldmanagementsystem-services-user:latest"
    },
    @{
        ProjectName = "FieldManagementSystem.Services.Field"
        ProjectFile = "..\Services\Field\FieldManagementSystem.Services.Field\FieldManagementSystem.Services.Field.csproj"
        Dockerfile = "..\Services\Field\FieldManagementSystem.Services.Field\Dockerfile"
        BuildOutput = Join-Path $BuildRoot "FieldManagementSystem.Services.Field"
        ImageName = "fieldmanagementsystem-services-field:latest"
    },
    @{
        ProjectName = "FieldManagementSystem.Services.Controller"
        ProjectFile = "..\Services\Controller\FieldManagementSystem.Services.Controller\FieldManagementSystem.Services.Controller.csproj"
        Dockerfile = "..\Services\Controller\FieldManagementSystem.Services.Controller\Dockerfile"
        BuildOutput = Join-Path $BuildRoot "FieldManagementSystem.Services.Controller"
        ImageName = "fieldmanagementsystem-services-controller:latest"
    }
)

# --- 1. Publish .NET Project ---
Write-Host "Step 1: Publishing .NET project..."
if (Test-Path $BuildRoot) {
    Remove-Item -Recurse -Force $BuildRoot
}

foreach ($proj in $projects) {
    Write-Host "Publishing project: $($proj.ProjectName)"
    dotnet publish $proj.ProjectFile -c Release -o $proj.BuildOutput
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to publish project $($proj.ProjectName). Aborting."
        exit 1
    }
    Write-Host "Project $($proj.ProjectName) published successfully to $($proj.BuildOutput)"
    Write-Host ""
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to publish .NET project. Aborting."
    exit 1
}
Write-Host ""

# --- 1.5. Run Tests ---
Write-Host "Step 1.5: Running .NET tests..."
foreach ($proj in $projects) {
    Write-Host "Running tests for project: $($proj.ProjectName)"
    dotnet test ..\Services\FieldManagementSystem.sln --filter "FullyQualifiedName~$($proj.ProjectName)"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed for project $($proj.ProjectName). Aborting."
        exit 1
    }
    Write-Host "Tests passed successfully for project $($proj.ProjectName)"
    Write-Host ""
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed. Aborting."
    exit 1
}
Write-Host ".NET tests passed successfully."
Write-Host ""

# --- 2. Build Docker Image ---
Write-Host "Step 2: Building Docker image..."
foreach ($proj in $projects) {
    Write-Host "Building Docker image for project: $($proj.ProjectName)"
    docker build -t $proj.ImageName -f $proj.Dockerfile $proj.BuildOutput
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build Docker image for project $($proj.ProjectName). Aborting."
        exit 1
    }
    Write-Host "Docker image '$($proj.ImageName)' built successfully."
    Write-Host ""
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build Docker image. Aborting."
    exit 1
}
Write-Host ""

# --- 3. Rerun Docker Compose ---
Write-Host "Step 3: Rerunning docker-compose..."

# Remove the shared network if it exists to ensure a clean state
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

# Bring up services from main compose file
Write-Host "Bringing up services from main compose file..."
docker-compose -f $ComposeFile up -d --force-recreate
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to bring up services from main compose file. Aborting."
    exit 1
}

Write-Host "Docker Compose services have been updated."
Write-Host ""
Write-Host "Deployment complete."
