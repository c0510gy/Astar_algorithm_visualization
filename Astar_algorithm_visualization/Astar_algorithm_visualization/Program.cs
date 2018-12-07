using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Astar_algorithm_visualization
{
    class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
        private static extern IntPtr GetConsoleHandle();
        static IntPtr handler = GetConsoleHandle();

        static Random random = new Random();

        static void Main(string[] args)
        {
            int width = 2000, height = 1500;
            
            int V = 100, E = 1000, S = 1, T = 100;
            int[,] w = new int[E, 3];
            int[,] Vs = new int[V, 2]; //각 정점의 좌표
            int[] H = new int[V];
            for (int j = 0; j < V; j++)
            {
                Vs[j, 0] = random.Next(30, width - 15 + 1);
                Vs[j, 1] = random.Next(30, height - 15 + 1);
            }

            bool[,] chk = new bool[V, V];
            for (int j = 0; j < E; j++)
            {
                int v1 = random.Next(1, V + 1);
                int v2 = random.Next(1, V + 1);
                if (v1 == v2 || chk[v1 - 1, v2 - 1])
                {
                    j--;
                    continue;
                }
                chk[v1 - 1, v2 - 1] = true;

                w[j, 0] = v1;
                w[j, 1] = v2;
                int dis = (Vs[v1 - 1, 0] - Vs[v2 - 1, 0]) * (Vs[v1 - 1, 0] - Vs[v2 - 1, 0])
                    + (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) * (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]);
                w[j, 2] = (int)Math.Sqrt((double)dis);
            }
            for(int j = 0; j < V; j++)
            {
                int v1 = j + 1;
                int v2 = T;
                int dis = (Vs[v1 - 1, 0] - Vs[v2 - 1, 0]) * (Vs[v1 - 1, 0] - Vs[v2 - 1, 0])
                    + (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) * (Vs[v1 - 1, 1] - Vs[v2 - 1, 1]);
                H[j] = (int)Math.Sqrt((double)dis); // 휴리스틱 값 = 유클리드 거리
            }
            
            Astar astar = new Astar(V, E, S, T);
            for (int j = 1; j <= V; j++)
            {
                astar.SetHeuristics(j, H[j - 1]);
            }
            for (int j = 0; j < E; j++)
            {
                astar.AddWeight(w[j, 0], w[j, 1], w[j, 2]);
            }

            Bitmap visual = new Bitmap(width + 2000, height + 2000);
            Graphics g = Graphics.FromImage(visual);
            int ans = astar.run_draw(ref visual, ref g, ref Vs, width, height);

            astar.visualize(ref visual, ref g, ref Vs, width, height, -1, -1, "A* 알고리즘을 이용한 최소경로 계산 완료  최소경로 비용 = " + ans, true);

            Console.WriteLine(T + " : " + ans);
            astar.printPath();

            Console.ReadLine();
        }
    }
}
