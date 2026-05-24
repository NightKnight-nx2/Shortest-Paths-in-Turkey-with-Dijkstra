using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

class Program
{
    static CityGraph TurkeyGraph;
    static CityGraph IzmirGraph;

    static void Main(string[] args)
    {
        Console.WriteLine("Veriler yükleniyor, lütfen bekleyin...");
        try
        {
            TurkeyGraph = new CityGraph("assets\\ilmesafe.xlsx", "assets\\plakalar_komsu.txt", true);
            IzmirGraph = new CityGraph("assets\\ilcemesafe.xlsx", "assets\\ilceler_komsu.txt", false);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Kritik Hata: Veriler yüklenirken bir sorun oluştu.");
            Console.WriteLine(ex.Message);
            Console.WriteLine("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
            return;
        }

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("    DİJKSTRA EN KISA YOL BULUCU (TÜRKİYE & İZMİR) ");
            Console.WriteLine("==================================================");
            Console.WriteLine("1. Türkiye Geneli İller Arası En Kısa Yol");
            Console.WriteLine("2. İzmir İlçeleri Arası En Kısa Yol");
            Console.WriteLine("3. Eski Analiz Raporunu Çalıştır");
            Console.WriteLine("4. Çıkış");
            Console.WriteLine("==================================================");
            Console.Write("Seçiminiz: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    InteractiveRouteFinder(TurkeyGraph, "Türkiye İlleri");
                    break;
                case "2":
                    InteractiveRouteFinder(IzmirGraph, "İzmir İlçeleri");
                    break;
                case "3":
                    RunLegacyAnalysis();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim. Devam etmek için Enter'a basın...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    static void InteractiveRouteFinder(CityGraph graph, string context)
    {
        Console.Clear();
        Console.WriteLine($"--- {context} Arası En Kısa Yol Bulucu ---");
        
        int startNode = SelectNode(graph, "Başlangıç");
        if (startNode == -1) return;

        int endNode = SelectNode(graph, "Hedef");
        if (endNode == -1) return;

        if (startNode == endNode)
        {
            Console.WriteLine("\nBaşlangıç ve hedef aynı olamaz! Devam etmek için Enter'a basın...");
            Console.ReadLine();
            return;
        }

        var result = graph.DijkstraPath(startNode, endNode);

        Console.WriteLine("\n================ SONUÇ ================");
        if (result.distance == int.MaxValue)
        {
            Console.WriteLine($"{graph.NodeNames[startNode]} ile {graph.NodeNames[endNode]} arasında herhangi bir karayolu bağlantısı bulunamadı.");
        }
        else
        {
            Console.WriteLine($"Güzergah: {string.Join(" -> ", result.path.Select(i => graph.NodeNames[i]))}");
            Console.WriteLine($"Toplam Mesafe (Dijkstra): {result.distance} km");
            Console.WriteLine($"Kuş Uçuşu / Direkt Mesafe : {graph.DirectDistances[startNode][endNode]} km");
        }
        Console.WriteLine("=======================================");
        Console.WriteLine("\nAna menüye dönmek için Enter'a basın...");
        Console.ReadLine();
    }

    static int SelectNode(CityGraph graph, string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} noktası girin (Listeyi görmek için 'liste', İptal için 'iptal'): ");
            string input = Console.ReadLine()?.Trim();
            
            if (string.Equals(input, "iptal", StringComparison.OrdinalIgnoreCase)) return -1;
            
            if (string.Equals(input, "liste", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n--- Nokta Listesi ---");
                for (int i = 0; i < graph.NodeCount; i++)
                {
                    Console.Write($"{graph.NodeNames[i], -20}");
                    if ((i + 1) % 4 == 0) Console.WriteLine();
                }
                Console.WriteLine("\n---------------------\n");
                continue;
            }

            int index = Array.FindIndex(graph.NodeNames, n => string.Equals(n, input, StringComparison.CurrentCultureIgnoreCase));
            if (index != -1)
            {
                return index;
            }
            
            Console.WriteLine("Bulunamadı! Lütfen listedeki isimlerden birini doğru yazın.\n");
        }
    }

    static void RunLegacyAnalysis()
    {
        Console.Clear();
        Console.WriteLine("--- ANALİZ RAPORU ÇALIŞTIRILIYOR ---");
        Console.WriteLine("Bu işlem biraz zaman alabilir...\n");

        Console.WriteLine("== TÜRKİYE İLLERİ ANALİZİ ==");
        AnalyzeGraph(TurkeyGraph);

        Console.WriteLine("\n== İZMİR İLÇELERİ ANALİZİ ==");
        AnalyzeGraph(IzmirGraph);

        Console.WriteLine("\nAnaliz tamamlandı. Ana menüye dönmek için Enter'a basın...");
        Console.ReadLine();
    }

    static void AnalyzeGraph(CityGraph graph)
    {
        List<(int node1, int node2, int roadDistance, int dijkstraDistance, int difference)> differences = new List<(int, int, int, int, int)>();

        for (int i = 0; i < graph.NodeCount; i++)
        {
            int[] dijkstraDistances = graph.DijkstraAllDistances(i);
            for (int j = i + 1; j < graph.NodeCount; j++)
            {
                int roadDistance = graph.DirectDistances[i][j];
                int dijkstraDistance = dijkstraDistances[j];

                if (graph.GraphDistances[i][j] == int.MaxValue && dijkstraDistance != int.MaxValue)
                {
                    int difference = Math.Abs(roadDistance - dijkstraDistance);
                    differences.Add((i, j, roadDistance, dijkstraDistance, difference));
                    Console.WriteLine($"{graph.NodeNames[i]} - {graph.NodeNames[j]}: Karayolu {roadDistance} km, Dijkstra {dijkstraDistance} km, Fark {difference} km");
                }
            }
        }

        if (differences.Count == 0)
        {
            Console.WriteLine("Hesaplanabilir fark bulunamadı.");
            return;
        }

        int minDifference = differences.Min(d => d.difference);
        int maxDifference = differences.Max(d => d.difference);

        Console.WriteLine("\nEn küçük farka sahip çiftler:");
        foreach (var diff in differences.Where(d => d.difference == minDifference))
        {
            Console.WriteLine($"{graph.NodeNames[diff.node1]} - {graph.NodeNames[diff.node2]}: Karayolu {diff.roadDistance} km, Dijkstra {diff.dijkstraDistance} km, Fark {diff.difference} km");
        }

        Console.WriteLine("\nEn büyük farka sahip çiftler:");
        foreach (var diff in differences.Where(d => d.difference == maxDifference))
        {
            Console.WriteLine($"{graph.NodeNames[diff.node1]} - {graph.NodeNames[diff.node2]}: Karayolu {diff.roadDistance} km, Dijkstra {diff.dijkstraDistance} km, Fark {diff.difference} km");
        }
    }
}

class CityGraph
{
    public string[] NodeNames { get; private set; }
    public int[][] DirectDistances { get; private set; }
    public int[][] GraphDistances { get; private set; }
    public int NodeCount { get; private set; }

    public CityGraph(string excelPath, string txtPath, bool isNumericTxt)
    {
        if (!File.Exists(excelPath) || !File.Exists(txtPath))
        {
            throw new FileNotFoundException($"Gerekli dosyalar bulunamadı: {excelPath} veya {txtPath}");
        }

        using (var workbook = new XLWorkbook(excelPath))
        {
            var worksheet = workbook.Worksheet(1);
            
            int rowCount = 0;
            while (!worksheet.Cell(rowCount + 3, 2).IsEmpty())
            {
                rowCount++;
            }
            NodeCount = rowCount;

            NodeNames = new string[NodeCount];
            for (int i = 0; i < NodeCount; i++)
            {
                NodeNames[i] = worksheet.Cell(i + 3, 2).GetString().Trim();
            }

            DirectDistances = new int[NodeCount][];
            GraphDistances = new int[NodeCount][];
            for (int i = 0; i < NodeCount; i++)
            {
                DirectDistances[i] = new int[NodeCount];
                GraphDistances[i] = new int[NodeCount];
                for (int j = 0; j < NodeCount; j++)
                {
                    DirectDistances[i][j] = (int)worksheet.Cell(i + 3, j + 3).GetDouble();
                    GraphDistances[i][j] = int.MaxValue;
                }
            }
        }

        Dictionary<string, List<string>> neighborsList = new Dictionary<string, List<string>>(StringComparer.CurrentCultureIgnoreCase);
        using (StreamReader reader = new StreamReader(txtPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(':');
                if (parts.Length < 2) continue;

                string key = parts[0].Trim();
                var neighbors = parts[1].Split(',').Select(n => n.Trim()).ToList();

                if (isNumericTxt)
                {
                    if (int.TryParse(key, out int plaka) && plaka >= 1 && plaka <= NodeCount)
                    {
                        string nodeName = NodeNames[plaka - 1]; 
                        List<string> namedNeighbors = new List<string>();
                        foreach (var n in neighbors)
                        {
                            if (int.TryParse(n, out int komsuPlaka) && komsuPlaka >= 1 && komsuPlaka <= NodeCount)
                            {
                                namedNeighbors.Add(NodeNames[komsuPlaka - 1]);
                            }
                        }
                        neighborsList[nodeName] = namedNeighbors;
                    }
                }
                else
                {
                    neighborsList[key] = neighbors;
                }
            }
        }

        for (int i = 0; i < NodeCount; i++)
        {
            for (int j = 0; j < NodeCount; j++)
            {
                if (i == j)
                {
                    GraphDistances[i][j] = 0;
                }
                else if (neighborsList.TryGetValue(NodeNames[i], out var nList) && nList.Contains(NodeNames[j], StringComparer.CurrentCultureIgnoreCase))
                {
                    GraphDistances[i][j] = DirectDistances[i][j];
                }
                else
                {
                    GraphDistances[i][j] = int.MaxValue;
                }
            }
        }
    }

    public (int distance, List<int> path) DijkstraPath(int startIndex, int endIndex)
    {
        int[] distances = new int[NodeCount];
        int[] previous = new int[NodeCount];
        bool[] visited = new bool[NodeCount];

        for (int i = 0; i < NodeCount; i++)
        {
            distances[i] = int.MaxValue;
            previous[i] = -1;
            visited[i] = false;
        }

        distances[startIndex] = 0;

        for (int count = 0; count < NodeCount; count++)
        {
            int u = MinDistance(distances, visited);
            if (u == -1 || distances[u] == int.MaxValue) break;

            visited[u] = true;

            if (u == endIndex) break; 

            for (int v = 0; v < NodeCount; v++)
            {
                if (!visited[v] && GraphDistances[u][v] != int.MaxValue &&
                    distances[u] + GraphDistances[u][v] < distances[v])
                {
                    distances[v] = distances[u] + GraphDistances[u][v];
                    previous[v] = u;
                }
            }
        }

        List<int> path = new List<int>();
        if (distances[endIndex] != int.MaxValue)
        {
            int curr = endIndex;
            while (curr != -1)
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Reverse();
        }

        return (distances[endIndex], path);
    }

    public int[] DijkstraAllDistances(int startIndex)
    {
        int[] distances = new int[NodeCount];
        bool[] visited = new bool[NodeCount];

        for (int i = 0; i < NodeCount; i++)
        {
            distances[i] = int.MaxValue;
            visited[i] = false;
        }

        distances[startIndex] = 0;

        for (int count = 0; count < NodeCount; count++)
        {
            int u = MinDistance(distances, visited);
            if (u == -1 || distances[u] == int.MaxValue) break;

            visited[u] = true;

            for (int v = 0; v < NodeCount; v++)
            {
                if (!visited[v] && GraphDistances[u][v] != int.MaxValue &&
                    distances[u] + GraphDistances[u][v] < distances[v])
                {
                    distances[v] = distances[u] + GraphDistances[u][v];
                }
            }
        }
        return distances;
    }

    private int MinDistance(int[] distances, bool[] visited)
    {
        int min = int.MaxValue, minIndex = -1;

        for (int v = 0; v < NodeCount; v++)
        {
            if (!visited[v] && distances[v] <= min)
            {
                min = distances[v];
                minIndex = v;
            }
        }
        return minIndex;
    }
}
