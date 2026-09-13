# Épületfűtési rendszerek hőtechnikai modellezése és 3D vizualizációja
## Rövid leírás
A dolgozat célja egy szimulációs szoftver elkészítése, amellyel modellezhető épületek hőtechnikája, és ezzel hőmérsékleti és veszteségi adatok deríthetők fel.
A program egy asztali alkalmazás, amely input mezőkből, menükből és egy központi 3D megjelenítőből áll. A 3D megjelenítő egy lecsupaszított épületet mutat kezdetben.
Az épület alakját előre definiált lehetőségek közül lehet kiválasztani. Ezután a felhasználónak lehetősége lesz megadni az építőanyag és a szigetelés, illetve a vastagság tulajdonságait,
majd drag-and-drop technikával hőtermelőket és hőleadókat helyezhet el az épület 3D-s modelljében. A hőtermelőkre és hőleadókra való kattintás hatására felugró ablakokban megadhatók lesznek
az azokra jellemző specifikus adatok, mint például: hőtechnikai mérések (sugárzási veszteség mérés), az anyag minőségi jellemzői, hőhordozó típus. A hőtermelők és hőleadók összeköthetőek lesznek 
vonalakkal, amelyek a fűtési csöveket reprezentálják, ezeknek is megadható anyag minőség illetve szigetelés minőség. A felhasználó által megadott adatok elmenthetőek, és később betölthetőek lesznek.
Ezek után a szimuláció elindításával a 3D térben vizualizálódik a hő áramlása, ezzel megmutatva hol veszíti el az épület a fűtésből eredő hőjét. Az épület bizonyos pontjaira kattintva a 
3D modellben láthatóvá válnak a pontszerű adatok is, mint például a pillanatnyi hőmérséklet.

## Használt technológia
A szimulációhoz a hővezetési egyenlet megoldását közelítjük majd diszkrét 3D rácson. A program C# nyelven kerül megvalósításra, Unity keretrendszer használatával.
- C# programozási nyelv
- Unity ver. 6000.3.8f1
- Github

## Használható tanulási anyag
- https://www.wiley.com/en-us/shop/general-introductory-mechanical-engineering/fundamentals-of-heat-and-mass-transfer-8th-edition-p-9781119353881
- https://www.visualslope.com/Library/FDM-for-heat-transfering.pdf
