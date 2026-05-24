# Dijkstra Shortest Path Finder 🛣️

*[🇹🇷 Türkçe sürüm için aşağı kaydırın (Turkish version below)](#türkçe-sürüm)*

This project is a C# (.NET) console application that uses **Dijkstra's Algorithm** to find the **shortest highway route** between 81 cities in Turkey and the districts of Izmir.

## 🚀 Features

- **Interactive User Menu:** Enter the starting and destination points (city or district) to view the shortest path via the console.
- **Route Extraction:** Not only calculates the distance but also lists the step-by-step route (e.g., `Izmir -> Manisa -> Usak -> ...`) required to reach the destination.
- **Real Distance vs. Dijkstra Comparison:** Compare the direct distance (read from Excel) with the Dijkstra route distance traveled via neighboring cities.
- **Smart Text Detection:** Seamlessly detects city names regardless of case sensitivity or Turkish characters (`İ`, `ı`).

## 📁 Project Structure & Data Sources

During runtime, the project dynamically reads map and distance information from external files located in the `assets/` folder:

- `ilmesafe.xlsx`: Direct highway distances between Turkey's cities.
- `plakalar_komsu.txt`: Geographical neighborhood relationships of cities (based on plate numbers).
- `ilcemesafe.xlsx`: Distances between the districts of Izmir.
- `ilceler_komsu.txt`: Geographical neighborhood relationships of Izmir districts.

*Note: The application uses these files to dynamically build a Graph structure. Because it doesn't use fixed-size arrays, it will continue to work without code changes even if new data is added to the Excel files.*

## 🛠️ Setup and Execution

To run the project, you must have the [.NET SDK](https://dotnet.microsoft.com/download) installed on your system. The project uses the `ClosedXML` library to read Excel files.

1. Clone or download the project to your computer.
2. Navigate to the project directory (where the `Project_1.csproj` file is located) via Terminal or Command Prompt.
3. Run the following command to build and execute the project:

```bash
dotnet run
```

4. Select the desired operation from the menu (e.g., press 1 for Turkey General).
5. Enter your starting and destination points. You can type `liste` on the input screen to see all available cities/districts.

## 🧠 Algorithm Details

At the core of the project's architecture lies the famous **Dijkstra's Algorithm** (the `DijkstraPath` method within the `CityGraph` class in `Program.cs`).
- Each city or district is modeled as a **"Node"**, and the roads between neighboring points are modeled as **"Edges"**.
- The distance between two non-neighboring points is considered "Infinity" (`int.MaxValue`), ensuring that the algorithm only uses valid/open highway connections on the map.
- During execution, the algorithm remembers the traversed path using a `previous` array, and once the route is successfully completed, it backtracks to build the route text.

---

<h1 id="türkçe-sürüm">Dijkstra En Kısa Yol Bulucu 🛣️ (Türkçe)</h1>

*[🇬🇧 Scroll up for English version](#dijkstra-shortest-path-finder-️)*

Bu proje, Türkiye'nin 81 ili ve İzmir'in ilçeleri arasındaki **en kısa karayolu rotasını** bulmak için geliştirilmiş, **Dijkstra Algoritması** kullanan bir C# (.NET) konsol uygulamasıdır.

## 🚀 Özellikler

- **Etkileşimli Kullanıcı Menüsü:** Başlangıç ve hedef noktalarını (il veya ilçe) girerek aralarındaki en kısa yolu konsol üzerinden görebilirsiniz.
- **Güzergah (Rota) Çıkarımı:** Sadece iki nokta arasındaki mesafeyi bulmakla kalmaz, aynı zamanda hedefe ulaşmak için hangi il/ilçelerden geçmeniz gerektiğini adım adım (`İzmir -> Manisa -> Uşak -> ...`) listeler.
- **Gerçek Mesafe vs. Dijkstra Kıyaslaması:** Rotanın harita üzerindeki (Excel'den okunan) doğrudan mesafesi ile komşu iller üzerinden gidilen Dijkstra rota mesafesini karşılaştırarak analiz yapabilirsiniz.
- **Akıllı Metin Algılama:** Büyük/küçük harf veya Türkçe karakter (`İ`, `ı`) fark etmeksizin girdiğiniz şehir isimleri sorunsuz algılanır.

## 📁 Proje Yapısı ve Veri Kaynakları

Proje, çalışırken harita ve mesafe bilgilerini `assets/` klasörü içerisindeki dış dosyalardan dinamik olarak okur:

- `ilmesafe.xlsx`: Türkiye'nin illeri arasındaki doğrudan karayolu mesafeleri.
- `plakalar_komsu.txt`: İllerin coğrafi komşuluk ilişkileri (Plaka numaraları bazlı).
- `ilcemesafe.xlsx`: İzmir'in ilçeleri arasındaki mesafeler.
- `ilceler_komsu.txt`: İzmir ilçelerinin coğrafi komşuluk ilişkileri.

*Not: Uygulama bu dosyaları kullanarak dinamik bir Graph (düğüm/kenar ağı) yapısı oluşturur. Sabit boyutlu diziler kullanılmadığı için Excel'e yeni bir veri eklendiğinde uygulama kod değişmesine gerek kalmadan çalışmaya devam eder.*

## 🛠️ Kurulum ve Çalıştırma

Projenin çalışması için sisteminizde [.NET SDK](https://dotnet.microsoft.com/download) kurulu olması gerekmektedir. Proje, Excel dosyalarını okuyabilmek için `ClosedXML` kütüphanesini kullanır.

1. Projeyi bilgisayarınıza klonlayın veya indirin.
2. Terminal (veya Komut İstemcisi) üzerinden projenin bulunduğu dizine (`Project_1.csproj` dosyasının olduğu yere) gidin.
3. Projeyi derlemek ve çalıştırmak için aşağıdaki komutu kullanın:

```bash
dotnet run
```

4. Ekrana gelen menüden yapmak istediğiniz işlemi (örneğin 1'e basarak Türkiye Geneli) seçin. 
5. Başlangıç ve hedef noktalarını girin. Mevcut tüm şehir/ilçeleri görmek için girdi ekranında `liste` yazabilirsiniz.

## 🧠 Algoritma Detayları

Proje mimarisinin merkezinde meşhur **Dijkstra Algoritması** (`Program.cs` içerisindeki `CityGraph` sınıfı -> `DijkstraPath` metodu) yer almaktadır. 
- Her bir il veya ilçe bir **"Düğüm (Node)"**; komşu olan noktalar arasındaki yollar ise **"Kenar (Edge)"** olarak modellenmiştir. 
- Komşu olmayan iki nokta arasındaki uzaklık "Sonsuz" (`int.MaxValue`) kabul edilerek, algoritmanın harita üzerinde sadece geçerli/açık karayolu bağlantılarını kullanması güvence altına alınmıştır.
- Algoritma çalışırken `previous` dizisi kullanılarak gidilen yol hafızada tutulur ve rota başarıyla tamamlandığında geriye dönük (backtrack) olarak güzergah metni inşa edilir.

---

> Bu proje C# ve Algoritma Analizi çalışmaları kapsamında geliştirilmiştir.
