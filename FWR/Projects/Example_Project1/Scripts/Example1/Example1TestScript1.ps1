Write-Output 'Hello World'

$loops = 10;

For ($i=0; $i -le $loops; $i++) {
		Write-Output "$i / $loops"
        Sleep 1
    }