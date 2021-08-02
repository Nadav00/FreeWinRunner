Write-Output 'Hello World'

$loops = 1000;

For ($i=0; $i -le $loops; $i++) {
		Write-Output "$i / $loops"
        Sleep 1
    }