using System;
using System.Collections.Generic;

namespace EulerHierholzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== THUAT TOAN HIERHOLZER - CHU TRINH EULER TRONG DO THI VO HUONG ===");

            Console.Write("Nhập số đỉnh n: ");
            int n = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Nhập số cạnh m: ");
            int m = int.Parse(Console.ReadLine() ?? "0");

            if (m == 0)
            {
                Console.WriteLine("Đồ thị không có cạnh nên không xét chu trình Euler.");
                Console.WriteLine("Nhấn Enter để kết thúc...");
                Console.ReadLine();
                return;
            }

            var adj = new List<int>[n + 1];
            for (int i = 1; i <= n; i++)
            {
                adj[i] = new List<int>();
            }

            int[] degree = new int[n + 1];

            Console.WriteLine("Nhập các cạnh dạng: u v (đồ thị vô hướng)");
            Console.WriteLine("Ví dụ: 1 2 nghĩa là cạnh nối giữa đỉnh 1 và 2.\n");

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
                if (parts.Length < 2)
                {
                    Console.WriteLine("Bạn phải nhập đủ 2 số: u v. Nhập lại.");
                    i--;
                    continue;
                }

                int u = int.Parse(parts[0]);
                int v = int.Parse(parts[1]);

                if (u < 1 || u > n || v < 1 || v > n)
                {
                    Console.WriteLine("Đỉnh không hợp lệ (ngoài 1..n). Nhập lại.");
                    i--;
                    continue;
                }

                adj[u].Add(v);
                adj[v].Add(u);
                degree[u]++;
                degree[v]++;
            }

            // Kiểm tra điều kiện tồn tại chu trình Euler
            if (!HasEulerianCycle(adj, degree, n, out int startVertex, out string reason))
            {
                Console.WriteLine($"\nKhông tồn tại chu trình Euler. Lý do: {reason}");
                Console.WriteLine("Nhấn Enter để kết thúc...");
                Console.ReadLine();
                return;
            }

            // Tìm chu trình Euler bằng Hierholzer
            var eulerCycle = Hierholzer(adj, startVertex);

            Console.WriteLine("\nChu trình Euler tìm được:");
            Console.WriteLine(string.Join(" -> ", eulerCycle));

            Console.WriteLine("\nNhấn Enter để kết thúc...");
            Console.ReadLine();
        }

        /// <summary>
        /// Kiểm tra điều kiện tồn tại chu trình Euler trong đồ thị vô hướng
        /// </summary>
        static bool HasEulerianCycle(List<int>[] adj, int[] degree, int n,
                                     out int startVertex, out string reason)
        {
            startVertex = -1;
            reason = "";

            // Tìm một đỉnh có bậc > 0 để bắt đầu DFS
            for (int i = 1; i <= n; i++)
            {
                if (degree[i] > 0)
                {
                    startVertex = i;
                    break;
                }
            }

            if (startVertex == -1)
            {
                reason = "Tất cả các đỉnh đều có bậc 0 (đồ thị không có cạnh).";
                return false;
            }

            // Kiểm tra liên thông (chỉ xét các đỉnh có bậc > 0)
            bool[] visited = new bool[n + 1];
            DFS(adj, startVertex, visited);

            for (int i = 1; i <= n; i++)
            {
                if (degree[i] > 0 && !visited[i])
                {
                    reason = "Đồ thị không liên thông (tồn tại thành phần có cạnh nhưng không nối với thành phần khác).";
                    return false;
                }
            }

            // Kiểm tra tất cả các đỉnh đều có bậc chẵn
            for (int i = 1; i <= n; i++)
            {
                if (degree[i] % 2 != 0)
                {
                    reason = $"Tồn tại đỉnh bậc lẻ (ví dụ đỉnh {i}), nên không có chu trình Euler.";
                    return false;
                }
            }

            reason = "OK";
            return true;
        }

        /// <summary>
        /// DFS dùng để kiểm tra liên thông
        /// </summary>
        static void DFS(List<int>[] adj, int u, bool[] visited)
        {
            visited[u] = true;
            foreach (int v in adj[u])
            {
                if (!visited[v])
                    DFS(adj, v, visited);
            }
        }

        /// <summary>
        /// Thuật toán Hierholzer dùng stack tìm chu trình Euler
        /// </summary>
        static List<int> Hierholzer(List<int>[] adj, int start)
        {
            var stack = new Stack<int>();
            var circuit = new List<int>();

            stack.Push(start);

            while (stack.Count > 0)
            {
                int u = stack.Peek();

                if (adj[u].Count > 0)
                {
                    // Lấy một đỉnh kề v bất kỳ (ở cuối danh sách)
                    int v = adj[u][adj[u].Count - 1];

                    // Xóa cạnh (u, v) khỏi danh sách kề
                    adj[u].RemoveAt(adj[u].Count - 1);
                    // Đồng thời xóa u khỏi danh sách kề của v
                    adj[v].Remove(u);

                    // Đi tiếp sang v
                    stack.Push(v);
                }
                else
                {
                    // u không còn cạnh chưa dùng
                    stack.Pop();
                    circuit.Add(u);
                }
            }

            // circuit hiện theo thứ tự ngược, đảo lại để có chu trình đúng chiều
            circuit.Reverse();
            return circuit;
        }
    }
}
