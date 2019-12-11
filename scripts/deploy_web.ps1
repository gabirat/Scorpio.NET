#TODO
#  napisac ten skrypt, ktory przyjmuje parametry
roverIp - wiadomo (potrzebne do polaczenia sie przez ssh)

ktory wykona kolejno: 
w katalogu src/Scorpio.Web komende 'npm run build' -> wtedy pewstanie katalog src/Scorpio.Web/build, ktory trzeba trzeba skopiowac do src/Scorpio.Api/wwwroot (utworzyc jesli nie istnieje)
w katalogu src/Scorpio.Api wykona komende 'dotnet publish -c Release -r linux-arm64' (opublikowany build powstanie w /src/Scorpio.Api/bin/Release/cos tam/publish)
caly powyzszy katalog publish zipowac
zipa wyslac przez SSH (SCP) do lazika do katalogu /home/jetson/scorpioweb i ustawic chmod 755 na pliku 'Scorpio.Api'

skrypt bedzie odpalany z windowsa
do wykonania tego bedzie potrzebne dotnet core 3.0 sdk, node 10.15.3