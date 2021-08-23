Write-Output 'Hello World'


#Write-Error "TERRIBLE_ERROR"
#throw "TERRIBLE_ERROR"

$loops = Get-Random -Maximum 5

$loops = $loops + 5

For ($i=0; $i -le $loops; $i++) {
		Write-Output "$i / $loops"
        Sleep 1
    }