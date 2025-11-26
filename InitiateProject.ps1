# Ask for the new project name
$newProjectName = "Day_" + (Read-Host "Enter the name of the new project")
Write-Host "Creating new project '$newProjectName'"

# Define the source and destination paths, assuming this script is in the root of the AoC solution folder
$sourcePath = "Day_Template"
$destinationPath = "$newProjectName"

# Abort if the project already exists
if (Test-Path -Path $destinationPath -PathType Container) 
{ 
    Read-Host -Prompt "Folder already exists. Terminating script." 
    exit 
}

# Copy the folder
Copy-Item -Path $sourcePath -Destination $destinationPath -Recurse

# Rename the .csproj file in the new folder
$csprojFile = Get-ChildItem -Path $destinationPath -Filter *.csproj
$newCsprojFileName = "$newProjectName.csproj"
Rename-Item -Path $csprojFile.FullName -NewName $newCsprojFileName
$projectRelativePath = $destinationPath + "\" + $newCsprojFileName

# Add the new .csproj file to the Visual Studio solution
$solutionFilePath = Get-ChildItem -Filter *.sln
dotnet sln $solutionFilePath add $projectRelativePath

Read-Host -Prompt "Project '$newProjectName' has been created and added to the solution."
