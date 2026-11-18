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

`GetTollFee(vehicle, dates)` utgår från att alla passager är från samma dag, eftersom dagstaket bara funkar per dag. Blandar man dagar kastas ett exception istället för att räkna fel i tysthet. Behövs stöd för flera dagar längre fram är det enkelt att lägga till - gruppera per dag och summera - men vi kör på en dag tills vidare.

## Struktur

```
source/
├── IVehicle.cs            interface för allt som kan tullas - varje fordon säger själv om det är skattefritt
├── Car.cs / Motorbike.cs  de två fordonstyper vi har idag
├── ITollCalculator.cs     kontraktet för att räkna ut en avgift
├── TollCalculator.cs      själva räknelogiken - intervallfönster + dagstak
├── IHolidayProvider.cs    kontraktet för "är detta datum skattefritt"
├── HolidayProvider.cs     helger, helgdagar, dagen-före-helgdag, juli
└── TollFeeSchedule.cs     konstanter + avgiftsschemat per timme

TollFeeCalculator/         class library-projektet (länkar in filerna ovan)
tests/                     xUnit-testprojekt
```

`TollCalculator` tar emot en `IHolidayProvider` via konstruktorn (defaultar till den riktiga `HolidayProvider` om man inte anger något annat). Mest för att tester ska kunna byta ut kalendern mot en påhittad variant och testa intervall-/tak-logiken isolerat - se `AlwaysChargeableHolidayProvider` i testfilen för ett exempel.

## Köra den

```bash
dotnet test
```

Alla tester ska vara gröna.

## Hur vi hamnade här

Arbetet kördes med en TDD-approach (test-driven development): testerna skrevs först, mot kraven i `Krav.txt`, så att det tydligt syntes svart på vitt vad i den befintliga koden som faktiskt var fel, en handfull röda tester som facit innan någon fix gjordes. Därefter implementerades fixarna tills de blev gröna, och varje efterföljande refaktorering gjordes med hela testsviten som skyddsnät.

Startpunkten var den befintliga `TollCalculator.cs`, som hade ett par riktiga buggar gömda i sig: 60-minutersfönstret jämförde millisekund-komponenten istället för faktisk förfluten tid, mitt-på-dagen-avgiften krävde minut ≥30 för varje timme mellan 9 och 14 (den skulle bara gällt timme 8), och juli var bara skattefritt ett hårdkodat år. De fixades först, tillsammans med "dagen före helgdag"-regeln som ett tidigare försök på samma övning missade helt.

Därifrån blev det mer av ett allmänt städ-pass: bröt ut helgdags-/datumlogiken och avgiftsschemat till egna klasser, bytte en lång if-kedja mot switch-expressions, tog bort ett dött fordons-enum (Tractor/Emergency/Diplomat/...) som ingen klass implementerade (kravet säger bara att "vissa fordon" är undantagna, och idag är det motorcyklar), och införde `ITollCalculator`/`IHolidayProvider` så att beräknarens beroenden går att byta ut - i första hand för tester idag, kanske för andra städers regler längre fram.

Efter det blev det en andra genomgång av hela koden, som hittade några saker till. Intervall-logiken kunde fortfarande ta för mycket betalt när tre eller fler passager hamnade inom samma timme, och helgdagslistan hade tappat trettondagen, första maj, midsommar och alla helgons dag på vägen - samtidigt som annandag pingst låg kvar trots att den inte varit helgdag sen 2005. Båda fixades på samma sätt som tidigare, rött test först och sen fixen. Utöver det sorteras passagerna nu innan beräkningen, passager från olika dagar och ett saknat fordon ger ett exception istället för ett tyst felaktigt belopp, och varje fordon säger nu själv om det är skattefritt (`IsTollFree`) istället för att beräknaren jämför strängar..

Commit-historiken har varje steg för sig om du vill ha hela resan, inte bara facit.
