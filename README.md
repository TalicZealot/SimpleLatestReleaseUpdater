# Simple Latest Release Updater

A small app that fetches the latest release of a GitHub repository, downloads and extracts it to a selected location, overwriting existing files.

## Setup
Download the release and extract it in your application release then configure for your purposes.

## Configuration
Edit the relevant keys in config.xml
```xml
    <add key="Username" value="GitHub username" />
    <add key="Repository" value="GitHub repositry name" />
    <add key="UpdateNamePrefix" value="Prefix for the release zip file, used to update" />
    <add key="InstallPath" value="Relative path where the archive gets extracted to" />
    <add key="DownloadPath" value="./temp/update.zip Where the update zip gets downloaded to, leave as default" />
```
