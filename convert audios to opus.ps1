New-Item -Path ".\audios_optimized" -Type Directory > $null
Get-ChildItem -Path .\audios\  -Include *.mp3 -Recurse | ForEach-Object -Process {
    New-Item ".\audios_optimized\$($_.Directory.Name)" -Type Directory > $null
    ffmpeg -i $_.FullName -acodec libopus -hide_banner -loglevel panic -ac 2 -ar 48000 ".\audios_optimized\$($_.Directory.Name)\$($_.Name.Replace(".mp3",".opus"))"
}
