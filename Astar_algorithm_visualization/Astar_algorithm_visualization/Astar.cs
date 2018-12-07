using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

class FGH //V, g(n), h(n) 저장용 (시각화용)
{
    public int v;
    public int g;
    public int h;
    public FGH(int v, int g, int h)
    {
        this.v = v;
        this.g = g;
        this.h = h;
    }
}

class Astar
{
    class cost
    {
        public int v;
        public int w;
        public cost(int v, int w)
        {
            this.v = v;
            this.w = w;
        }
    }

    private int V, E, S, T; // vertex, edge, start vertex, target vertex
    private List<cost>[] W; // weight | W[u][i] = {v, c}; u -> v : c

    //A* 알고리즘 컨테이너
    private Heap OpenList; // 갱신될 여지가 있기 때문에 탐색할 가치가 있는 노드 저장
    //private Heap ClosedList; // 더이상 탐색할 필요가 없는 노드 저장
    private bool[] IsClosed; // closed 리스트에 포함되어 있는지 여부
    private bool[] IsOopened; // open 리스트에 포함되어 있는지 여부 (시각화를 위함)
    private int[] preV; // 이전 정점 (경로 추출)

    //f(n) = g(n) + h(n) = 예상 총 비용
    private int[] G; // S노드 부터 n노드까지 비용
    private int[] H; // n노드 부터 T노드까지 휴리스틱 비용

    public Astar(int V, int E, int S, int T)
    {
        this.V = V;
        this.E = E;
        this.S = S;
        this.T = T;

        W = new List<cost>[this.V + 1];
        for (int j = 1; j <= this.V; j++)
            W[j] = new List<cost>();
        OpenList = new Heap();
        //ClosedList = new Heap();
        IsClosed = new bool[this.V + 1];
        IsOopened = new bool[this.V + 1];
        preV = new int[this.V + 1];
        for(int j = 1; j <= this.V; j++)
        {
            preV[j] = 0;
        }
        G = new int[this.V + 1];
        H = new int[this.V + 1];

        for(int j = 1; j <= this.V; j++)
        {
            IsClosed[j] = false;
            IsOopened[j] = false;

            G[j] = -1; // 결정되지 않음
            if (j == S)
                G[j] = 0;
        }
    }

    // weight 설정
    public void AddWeight(int u, int v, int w)
    {
        W[u].Add(new cost(v, w));
    }

    // 휴리스틱 값 설정
    public void SetHeuristics(int v, int h)
    {
        H[v] = h;
    }

    public int run()
    {
        int ans = -1; // 경로가 나오지 않을 경우 -1

        OpenList.InsertItem(new HeapItem(G[S] + H[S], S));

        while (!OpenList.IsEmpty)
        {
            HeapItem top_item = OpenList.DeleteItem();
            int v = (int)top_item.Value;

            if (IsClosed[v]) continue; // 이미 Closed된 노드 (중복되어 OpenList에 추가되어도 힙 구조이기 때문에 상관 없음)
            
            IsClosed[v] = true; // 해당 노드를 더이상 탐색이 필요 없는 최솟값을 달성했다고 결정
            
            if (v == T)
            {
                ans = G[v] + H[v]; // = top_item.Ranking;
                break;
            }

            int cnt = W[v].Count;
            for(int j = 0; j < cnt; j++)
            {
                int u = W[v][j].v;
                if (IsClosed[u]) continue; // 갱신할 필요 없는 경우 제외

                // G값 갱신 여부
                if (G[u] == -1 || G[u] > G[v] + W[v][j].w)
                {
                    preV[u] = v;
                    G[u] = G[v] + W[v][j].w;
                }

                int f = G[u] + H[u];
                OpenList.InsertItem(new HeapItem(f, u));
            }
        }

        return ans;
    }

    // T 정점 까지의 경로 출력
    public void printPath()
    {
        List<int> tmp = new List<int>();
        for(int v = T; v > 0; v = preV[v])
        {
            tmp.Add(v);
        }
        while(tmp.Count > 0)
        {
            Console.Write(tmp[tmp.Count - 1]);
            tmp.RemoveAt(tmp.Count - 1);
            if (tmp.Count > 0) Console.Write(" -> ");
        }
        Console.Write("\n");
    }

    // ### 시각화를 위함 ###
    public int run_draw(ref Bitmap visual, ref Graphics g, ref int[,] Vs, int width, int height)
    {
        int ans = -1; // 경로가 나오지 않을 경우 -1

        OpenList.InsertItem(new HeapItem(G[S] + H[S], new FGH(S, G[S], H[S])));
        IsOopened[S] = true;
        //시각화
        visualize(ref visual, ref g, ref Vs, width, height, -1, -1, "시작 정점을 열린목록(OpenList) 힙(min-heap)에 삽입(PUSH)");

        while (!OpenList.IsEmpty)
        {
            HeapItem top_item = OpenList.DeleteItem();
            int v = ((FGH)top_item.Value).v;

            //시각화
            visualize(ref visual, ref g, ref Vs, width, height, v, -1, "열린목록(OpenList) 힙(min-heap)에서 팝(POP)한 정점 " + v + "을 선택");

            // 이미 Closed된 노드 (중복되어 OpenList에 추가되어도 힙 구조이기 때문에 상관 없음)
            if (IsClosed[v])
            {
                //시각화
                visualize(ref visual, ref g, ref Vs, width, height, -1, -1, "정점 " + v + "는 이미 닫힌목록(ClosedList)에 존재하므로 무시");
                continue;
            }

            IsClosed[v] = true; // 해당 노드를 더이상 탐색이 필요 없는 최솟값을 달성했다고 결정
            IsOopened[v] = false;

            //시각화
            visualize(ref visual, ref g, ref Vs, width, height, v, -1, "정점 " + v + "를 닫힌목록(ClosedList)에 추가");

            if (v == T)
            {
                ans = G[v] + H[v]; // = top_item.Ranking;

                //시각화
                visualize(ref visual, ref g, ref Vs, width, height, v, -1, "정점 " + v + "은 도착 정점이므로 알고리즘 종료");
                break;
            }

            int cnt = W[v].Count;
            for (int j = 0; j < cnt; j++)
            {
                int u = W[v][j].v;
                //시각화
                visualize(ref visual, ref g, ref Vs, width, height, v, j, "선택한 정점 " + v + "와 연결된 정점 " + u + "탐색");

                // 갱신할 필요 없는 경우 제외
                if (IsClosed[u])
                {
                    //시각화
                    visualize(ref visual, ref g, ref Vs, width, height, v, j, "정점 " + u + "는 이미 닫힌목록(ClosedList)에 존재하므로 무시");
                    continue;
                }

                // G값 갱신 여부
                if (G[u] == -1 || G[u] > G[v] + W[v][j].w)
                {
                    preV[u] = v;
                    G[u] = G[v] + W[v][j].w;
                }

                int f = G[u] + H[u];
                OpenList.InsertItem(new HeapItem(f, new FGH(u, G[u], H[u])));
                IsOopened[u] = true;

                //시각화
                visualize(ref visual, ref g, ref Vs, width, height, v, j, "정점 " + u + "로 가는 F값 갱신 및 열린목록(OpenList) 힙(min-heap)에 삽입(PUSH)");
            }
        }

        return ans;
    }


    private int frames = 0;
    public void visualize(ref Bitmap visual, ref Graphics g, ref int[,] Vs, int width, int height, int selected_vertex, int selected_edge, string info_str, bool show_com = false)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.Clear(Color.White);

        int r = 25; //시각화 원 반지름
        // ### 간선 그리기 ###
        if (show_com) //완성된 경로만 보이기
        {
            for (int v = T; preV[v] > 0; v = preV[v])
            {
                int v1 = preV[v];
                int v2 = v;
                Pen p = new Pen(Color.Orange, 5);

                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5); //큰 화살표
                p.StartCap = LineCap.Round;
                p.CustomEndCap = bigArrow;

                double m = (double)(Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) / (double)(Vs[v1 - 1, 0] - Vs[v2 - 1, 0]);
                double seta = Math.Atan(m);
                int e = 1;
                if (Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] < Vs[v2 - 1, 1]
                    || Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] > Vs[v2 - 1, 1])
                {
                    e = -1; //arctan 범위별 조정
                }
                g.DrawLine(p, Vs[v1 - 1, 0], Vs[v1 - 1, 1], Vs[v2 - 1, 0] - (int)(Math.Cos(seta) * r) * e, Vs[v2 - 1, 1] - (int)(Math.Sin(seta) * r) * e);
            }
        }
        else
        for (int j = 1; j <= V; j++)
        {
            int v1 = j;

            int cnt = W[j].Count;
            for (int i = 0; i < cnt; i++)
            {
                int v2 = W[j][i].v;
                Pen p = new Pen(Color.FromArgb(100, 0, 0, 0), 5);
                if (preV[v2] == v1) //탐색된 경로
                {
                    p = new Pen(Color.Yellow, 5); //Pen(Color.Orange, 5);
                }
                if (selected_vertex == j && selected_edge == i)
                {
                    p = new Pen(Color.BlueViolet, 10);
                }
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5); //큰 화살표
                p.StartCap = LineCap.Round;
                p.CustomEndCap = bigArrow;

                double m = (double)(Vs[v1 - 1, 1] - Vs[v2 - 1, 1]) / (double)(Vs[v1 - 1, 0] - Vs[v2 - 1, 0]);
                double seta = Math.Atan(m);
                int e = 1;
                if (Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] < Vs[v2 - 1, 1]
                    || Vs[v1 - 1, 0] > Vs[v2 - 1, 0] && Vs[v1 - 1, 1] > Vs[v2 - 1, 1])
                {
                    e = -1; //arctan 범위별 조정
                }
                g.DrawLine(p, Vs[v1 - 1, 0], Vs[v1 - 1, 1], Vs[v2 - 1, 0] - (int)(Math.Cos(seta) * r) * e, Vs[v2 - 1, 1] - (int)(Math.Sin(seta) * r) * e);
            }
        }

        // ### 정점 그리기 ###
        for (int j = 0; j < V; j++)
        {
            SolidBrush brush = new SolidBrush(Color.White);
            if (j == S - 1) //출발 정점
                brush = new SolidBrush(Color.Red);
            else if (j == T - 1) //도착 정점
                brush = new SolidBrush(Color.GreenYellow);
            else if (IsClosed[j + 1]) //닫힌 정점
                brush = new SolidBrush(Color.Orange);
            else if (IsOopened[j + 1]) //열린 정점
                brush = new SolidBrush(Color.DeepSkyBlue);
            g.FillEllipse(brush, Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);
            if (selected_vertex - 1 == j)
                g.DrawEllipse(new Pen(Color.BlueViolet, 10), Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);
            else
                g.DrawEllipse(new Pen(Color.Black, 5), Vs[j, 0] - r, Vs[j, 1] - r, r * 2, r * 2);

            if (j + 1 >= 100)
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 18), new SolidBrush(Color.Black), Vs[j, 0] - 25, Vs[j, 1] - 15);
            }
            else if (j + 1 >= 10)
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 25), new SolidBrush(Color.Black), Vs[j, 0] - 25, Vs[j, 1] - 20);
            }
            else
            {
                g.DrawString((j + 1) + "", new Font("나눔고딕", 30), new SolidBrush(Color.Black), Vs[j, 0] - 20, Vs[j, 1] - 25);
            }
        }


        Font font = new Font("나눔고딕", 30);

        // ### OpenList HEAP 그리기
        Bitmap visual_heap = new Bitmap(3000, 1500);
        OpenList.visualHeap(ref visual_heap);
        g.DrawImage(visual_heap, 50, height + 200);

        // ### ClosedList 그리기
        Bitmap visual_closed = new Bitmap(2000, 1000);
        Graphics g_ = Graphics.FromImage(visual_closed);
        g_.SmoothingMode = SmoothingMode.AntiAlias;
        g_.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g_.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g_.Clear(Color.White);
        int w_ = 80, h_ = 50; // 시각화 사각형
        int rx = 100, ry = 100;
        for (int v = 1, cnt = 0; v <= V; v++)
        {
            if (!IsClosed[v]) continue;
            rx += w_ * 2 + 50;

            cnt++;
            if (cnt > 7)
            {
                cnt = 0;
                rx = 100 + w_ * 2 + 50;
                ry += h_ * 2 + 30;
            }

            g_.FillRectangle(new SolidBrush(Color.White), rx - w_, ry - h_, w_ * 2, h_ * 2);
            g_.DrawRectangle(new Pen(Color.Black, 5), rx - w_, ry - h_, w_ * 2, h_ * 2);
            g_.DrawLine(new Pen(Color.Black, 5), rx - w_, ry, rx + w_, ry);
            g_.DrawLine(new Pen(Color.Black, 5), rx, ry, rx, ry + h_);

            g_.DrawString("f(" + v + ")=" + (G[v] + H[v]), new Font("나눔고딕", 20), new SolidBrush(Color.Black), rx - 80, ry - 40);
            g_.DrawString(G[v] + "", new Font("나눔고딕", 20), new SolidBrush(Color.Black), rx - 80, ry + 10);
            g_.DrawString(H[v] + "", new Font("나눔고딕", 20), new SolidBrush(Color.Black), rx - 0, ry + 10);
        }
        g.DrawImage(visual_closed, width + 200, 200);


        font = new Font("나눔고딕", 60);
        g.DrawString(info_str, font, new SolidBrush(Color.Orange), 50, height + 1800);

        // 프레임 저장
        visual.Save(@"datas\" + frames + ".png", System.Drawing.Imaging.ImageFormat.Png); frames++;
    }
}