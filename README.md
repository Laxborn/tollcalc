# Toll Fee Calculator

En avgiftsberäknare för Göteborgs trängselskatt. Skön och rättfram - inget krångel, bara en bil som kör igenom en betalstation och en avgift som ska räknas ut rätt.

## Bakgrund

Det här är ett kodtest. Uppgiften: en applikation för att hantera biltullarna i Göteborg med tillhörande avgiftsberäkning för fordonen. Ingen deadline i sig, men ett resultat inom 7-10 dagar var riktmärket - och lösningen skulle läggas upp på egen GitHub så commit-historiken går att följa hela vägen, inte bara slutresultatet.

Utgångskoden hade inga direkta syntaxfel, men gott om utrymme för förbättring - strukturmässigt och rent allmänt. Så det är precis det den här readmen (och commit-historiken) försöker visa: vilka förändringar som gjordes, och varför.

## Reglerna

- Varje passage genom en betalstation kostar 8, 13 eller 18 kr beroende på tid på dygnet
- 06:00-06:29 → 8 kr, 06:30-06:59 → 13 kr, 07:00-07:59 → 18 kr, 08:00-08:29 → 13 kr, 08:30-14:59 → 8 kr, 15:00-15:29 → 13 kr, 15:30-16:59 → 18 kr, 17:00-17:59 → 13 kr, 18:00-18:29 → 8 kr, resten av dygnet → 0 kr
- Lördagar, söndagar, hela juli, helgdagar och dagen före en helgdag är skattefria
- Motorcyklar är undantagna
- Flera passager inom 60 minuter räknas bara en gång, till den högsta avgiften bland dem
- Max 60 kr per dag och fordon

## Struktur

```
source/
├── Vehicle.cs             interface för allt som kan tullas
├── Car.cs / Motorbike.cs  de två fordonstyper vi har idag
├── ITollCalculator.cs     kontraktet för att räkna ut en avgift
├── TollCalculator.cs      själva räknelogiken - intervallfönster + dagstak
├── IHolidayProvider.cs    kontraktet för "är detta datum skattefritt"
├── HolidayProvider.cs     helger, helgdagar, dagen-före-helgdag, juli
└── TollConfiguration.cs   konstanter + avgiftsschemat per timme

TollFeeCalculator/         class library-projektet (länkar in filerna ovan)
tests/                     xUnit-testprojekt
```

`TollCalculator` tar emot en `IHolidayProvider` via konstruktorn (defaultar till den riktiga `HolidayProvider` om man inte anger något annat). Mest för att tester ska kunna byta ut kalendern mot en påhittad variant och testa intervall-/tak-logiken isolerat - se `AlwaysChargeableHolidayProvider` i testfilen för ett exempel.

## Köra den

```bash
dotnet test
```

55 tester, alla gröna.

## Hur vi hamnade här

Startpunkten var den befintliga `TollCalculator.cs`, som hade ett par riktiga buggar gömda i sig: 60-minutersfönstret jämförde millisekund-komponenten istället för faktisk förfluten tid, mitt-på-dagen-avgiften krävde minut ≥30 för varje timme mellan 9 och 14 (den skulle bara gällt timme 8), och juli var bara skattefritt ett hårdkodat år. De fixades först, tillsammans med "dagen före helgdag"-regeln som ett tidigare försök på samma övning missade helt.

Därifrån blev det mer av ett allmänt städ-pass: bröt ut helgdags-/datumlogiken och avgiftsschemat till egna klasser, bytte en lång if-kedja mot switch-expressions, tog bort ett dött fordons-enum (Tractor/Emergency/Diplomat/...) som varken kravlistan efterfrågade eller någon klass implementerade, och införde `ITollCalculator`/`IHolidayProvider` så att beräknarens beroenden går att byta ut - i första hand för tester idag, kanske för andra städers regler längre fram.

Commit-historiken har varje steg för sig om du vill ha hela resan, inte bara facit.
