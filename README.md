# SubFinder
A .net console app to automatically find subs for TV shows

It will scan specified folders and look for missing subs files.  
It will try to get the best sub (base on team name) if possible or will download all available subs otherwise (so you can pick the synced one)

*Just a rough start that only implements download from addic7ed website*


Works with .net core 3.1

## Build it

To make an executable ready for linux arm64 just run the command

> dotnet publish -c Release -f netcoreapp3.1 -r linux-arm64

## Use it

On first launch it will create a settings.xml file and exit.  
Just configure this file based on your environment and you're good to go.

### Tips
1. You can omit a subfolder by creating a .no_sub_dl file in it
2. You can launch it with "--silent" so it won't pause on error message (for background tasks)
3. Run it periodically from cron and you can forget it 👌
