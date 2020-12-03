$day = (Get-Date).Day

&"curl.exe" "https://adventofcode.com/2020/day/$day/input" -H "cookie: session=$env:AOC_SESSION" -o "Day$($day.ToString('00')).input.txt"
 