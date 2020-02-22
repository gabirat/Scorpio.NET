Write-Host 'Starting...'

cd ..\src\Scorpio.Web
Write-Host 'Creating production release of Scorpio.Web...'
npm run build
cd ..\
Write-Host 'Build is available in src\Scorpio.Web\build'
Write-Host 'Cleaning Scorpio.Api/wwwroot...'
Remove-Item -recurse Scorpio.Api\wwwroot\* -exclude .gitkeep -Verbose

Write-Host 'Copying artifacts to Scorpio.Api\wwwroot...'
[string]$sourceDirectory  = ".\Scorpio.Web\build\*"
[string]$destinationDirectory = ".\Scorpio.Api\wwwroot"
Copy-item -Force -Recurse -Verbose $sourceDirectory -Destination $destinationDirectory

Write-Host 'Files copied!'