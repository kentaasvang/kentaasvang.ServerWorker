# ServerWorker

Runs on remote server and handles versioning and deployment of web applications.

## Description

ServiceWorker will interate over services specified in appsettings.json, and if a new version is found in **Published**-directory, it will be moved into a folder with the new version number and symlinked to the public folder.
