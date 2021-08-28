Write-Output 'Hello World'


#Write-Error "TERRIBLE_ERROR"
#throw "TERRIBLE_ERROR"

$loops = 3

For ($i=0; $i -le $loops; $i++) {
		Write-Output "$i / $loops"
        Sleep 1
    }