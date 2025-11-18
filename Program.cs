using System;
using System.Collections.Generic;

namespace DijkstraShortestPath
{
    class Program
    {
        private const long INF = (long)1e18;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== THUAT TOAN DIJKSTRA - DUONG DI NGAN NHAT TU 1 DINH NGUON ===");

            // Nhập số đỉnh và số cạnh
            Console.Write("Nhập số đỉnh n: ");
            int n = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Nhập số cạnh m: ");
            int m = int.Parse(Console.ReadLine() ?? "0");

            // Khởi tạo danh sách kề
            var graph = new List<(int to, int w)>[n + 1];
            for (int i = 1; i <= n; i++)
            {
                graph[i] = new List<(int to, int w)>();
            }

            Console.WriteLine("Nhập các cạnh dạng: u v w (u, v là đỉnh; w là trọng số không âm)");
            Console.WriteLine("Ví dụ: 1 2 5 nghĩa là cạnh (1,2) có trọng số 5.");
            Console.WriteLine("Giả sử đồ thị vô hướng.\n");

            for (int i = 0; i < m; i++)
            {
                Console.Write($"Cạnh {i + 1}: ");
                string? line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    i--;
                    continue;
                }

                string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    Console.WriteLine("Bạn phải nhập đủ 3 số: u v w. Nhập lại.");
                    i--;
                    continue;
                }

                int u = int.Parse(parts[0]);
                int v = int.Parse(parts[1]);
                int w = int.Parse(parts[2]);

                if (u < 1 || u > n || v < 1 || v > n || w < 0)
                {
                    Console.WriteLine("Dữ liệu cạnh không hợp lệ (đỉnh ngoài 1..n hoặc trọng số âm). Nhập lại.");
                    i--;
                    continue;
                }

                // Đồ thị vô hướng
                graph[u].Add((v, w));
                graph[v].Add((u, w));
            }

            Console.Write("\nNhập đỉnh nguồn s (1..n): ");
            int s = int.Parse(Console.ReadLine() ?? "1");
            if (s < 1 || s > n)
            {
                Console.WriteLine("Đỉnh nguồn không hợp lệ.");
                Console.WriteLine("Nhấn Enter để thoát...");
                Console.ReadLine();
                return;
            }

            // Chạy Dijkstra
            Dijkstra(graph, n, s, out long[] dist, out int[] parent);

            // In kết quả
            Console.WriteLine($"\nKẾT QUẢ: Đường đi ngắn nhất từ đỉnh nguồn {s}");
            Console.WriteLine("Đỉnh\tKhoảng cách\tĐường đi");

            for (int v = 1; v <= n; v++)
            {
                if (dist[v] == INF)
                {
                    Console.WriteLine($"{v}\tKhông tới được\t-");
                }
                else
                {
                    var path = BuildPath(v, parent);
                    Console.WriteLine($"{v}\t{dist[v]}\t\t{string.Join(" -> ", path)}");
                }
            }

            Console.WriteLine("\nNhấn Enter để kết thúc...");
            Console.ReadLine();
        }

        /// <summary>
        /// Thuật toán Dijkstra tìm đường đi ngắn nhất từ 1 đỉnh nguồn
        /// </summary>
        static void Dijkstra(List<(int to, int w)>[] graph, int n, int start,
                             out long[] dist, out int[] parent)
        {
            dist = new long[n + 1];
            parent = new int[n + 1];
            bool[] visited = new bool[n + 1];

            for (int i = 1; i <= n; i++)
            {
                dist[i] = INF;
                parent[i] = -1;
                visited[i] = false;
            }

            var pq = new PriorityQueue<int, long>();

            dist[start] = 0;
            pq.Enqueue(start, 0);

            while (pq.Count > 0)
            {
                pq.TryDequeue(out int u, out long du);

                if (visited[u]) continue;
                visited[u] = true;

                foreach (var edge in graph[u])
                {
                    int v = edge.to;
                    int w = edge.w;

                    if (!visited[v] && dist[v] > dist[u] + w)
                    {
                        dist[v] = dist[u] + w;
                        parent[v] = u;
                        pq.Enqueue(v, dist[v]);
                    }
                }
            }
        }

        /// <summary>
        /// Truy vết đường đi từ nguồn đến v thông qua mảng parent
        /// </summary>
        static List<int> BuildPath(int v, int[] parent)
        {
            var path = new List<int>();
            int cur = v;
            while (cur != -1)
            {
                path.Add(cur);
                cur = parent[cur];
            }
            path.Reverse();
            return path;
        }
    }
}
