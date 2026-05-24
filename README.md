# 2026ISI-2Sem-GrafuAlgoritmai

# 3-oji pratybų užduotis: Eulerio ciklo paieška

## Aprašymas

Šiame release įgyvendinta individuali 10 užduotis: Eulerio ciklo paieška neorientuotame grafe, kurio visų viršūnių laipsniai yra lyginiai.

Realizuoti ir palyginti du algoritmai:

| Algoritmas | Aprašymas |
|---|---|
| Fleury | Renkasi briauną, kuri nėra tiltas, jei yra kitas pasirinkimas. Tiltai tikrinami naudojant DFS. |
| Hierholzer | Eina per nepanaudotas briaunas ir formuoja Eulerio ciklą be tiltų tikrinimo. |

## Naudotos technologijos

| Parametras | Reikšmė |
|---|---|
| Kalba | C# |
| Grafo saugojimas | Gretimumo sąrašas |
| Testavimo kompiuteris | AMD Ryzen 7 8840HS, 16 GB RAM |
| OS | Windows 11 64-bit |

## Tyrimo parametrai

| Parametras | Reikšmės |
|---|---|
| Viršūnių skaičius V | 10, 50, 100, 200, 500, 1000 |
| Grafo laipsnis k | 2, 4, 6, 8 |
| Briaunų skaičius | E = V * k / 2 |
| Grafas | Neorientuotas, jungus, visų viršūnių laipsniai lyginiai |

## Rezultatai

### k = 2

| Viršūnės | Briaunos | Fleury ms | Hierholzer ms |
|---:|---:|---:|---:|
| 10 | 10 | 0.6276 | 0.0931 |
| 50 | 50 | 0.3078 | 0.0492 |
| 100 | 100 | 1.0244 | 0.0984 |
| 200 | 200 | 3.6815 | 0.1820 |
| 500 | 500 | 19.2929 | 0.4853 |
| 1000 | 1000 | 53.3930 | 1.1394 |

### k = 4

| Viršūnės | Briaunos | Fleury ms | Hierholzer ms |
|---:|---:|---:|---:|
| 10 | 20 | 0.1650 | 0.0203 |
| 50 | 100 | 2.5837 | 0.0922 |
| 100 | 200 | 10.0169 | 0.1870 |
| 200 | 400 | 30.0246 | 0.2744 |
| 500 | 1000 | 188.8974 | 0.6387 |
| 1000 | 2000 | 650.0320 | 1.3572 |

### k = 6

| Viršūnės | Briaunos | Fleury ms | Hierholzer ms |
|---:|---:|---:|---:|
| 10 | 30 | 0.2099 | 0.0196 |
| 50 | 150 | 4.3250 | 0.0987 |
| 100 | 300 | 16.1653 | 0.1909 |
| 200 | 600 | 62.0226 | 0.3481 |
| 500 | 1500 | 382.5504 | 1.0340 |
| 1000 | 3000 | 2099.3894 | 2.5046 |

### k = 8

| Viršūnės | Briaunos | Fleury ms | Hierholzer ms |
|---:|---:|---:|---:|
| 10 | 40 | 0.4828 | 0.0406 |
| 50 | 200 | 8.9554 | 0.1541 |
| 100 | 400 | 43.0840 | 0.3420 |
| 200 | 800 | 123.5266 | 0.5300 |
| 500 | 2000 | 786.5383 | 1.7872 |
| 1000 | 4000 | 3611.1285 | 3.5942 |

## Išvada

Pagal gautus rezultatus greitesnis yra Hierholzer algoritmas. Jis veikia efektyviau, nes netikrina tiltų ir tiesiogiai pereina per nepanaudotas briaunas.

Fleury algoritmas yra žymiai lėtesnis, nes kiekviename žingsnyje tikrina, ar pasirinkta briauna nėra tiltas. Tilto tikrinimui naudojama DFS paieška, todėl didėjant viršūnių ir briaunų skaičiui Fleury veikimo laikas auga daug sparčiau.

Rezultatai atitinka teorinę analizę: Hierholzer algoritmas yra artimas O(E), o Fleury algoritmas yra lėtesnis dėl pakartotinių DFS patikrinimų.
