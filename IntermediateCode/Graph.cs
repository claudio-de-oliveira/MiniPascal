using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateCode
{
    public enum COLOR
    {
        WHITE, GRAY, BLACK
    };

    public class LNode
    {
        private readonly List<LNode> pred;
        private readonly List<LNode> succ;
        private readonly KeyedData vertex;

        public COLOR color;
        public int d;
        public int f;
        public LNode pi;

        public LNode(KeyedData vertex)
        {
            this.vertex = vertex;
            pred = new List<LNode>();
            succ = new List<LNode>();
        }

        public LNode this[int i]
        { get { Debug.Assert(i >= 0 && i < succ.Count); return succ[i]; } }
        public KeyedData Vertex
        { get { return vertex; } }
        public List<LNode> Successor
        { get { return succ; } }
        public List<LNode> Predecessor
        { get { return pred; } }

        public override string ToString()
        { return Vertex.Key + ": " + d.ToString() + "/" + f.ToString(); }
    }

    public class Node
    {
        private readonly List<int> adjacency;
        private readonly KeyedData vertex;

        public Node(KeyedData vertex)
        {
            this.vertex = vertex;
            adjacency = new List<int>();
        }

        public int this[int i]
        { get { Debug.Assert(i >= 0 && i < adjacency.Count); return adjacency[i]; } }
        public KeyedData Vertex
        { get { return vertex; } }
        public List<int> Adjacency
        { get { return adjacency; } }
    }

    public class Graph
    {
        private readonly List<Node> nodes;

        public Graph()
        {
            nodes = new List<Node>();
        }

        public Node this[int i]
        { get { Debug.Assert(i >= 0 && i < Count); return nodes[i]; } }

        public int AddVertex(KeyedData vertex)
        {
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].Vertex.Key == vertex.Key)
                    return i;
            Node node = new Node(vertex);
            nodes.Add(node);
            return nodes.Count;
        }

        public void AddEdge(int from, int to)
        {
            foreach (int n in nodes[from].Adjacency)
                if (nodes[n].Vertex.Key == nodes[to].Vertex.Key)
                    return;
            nodes[from].Adjacency.Add(to);
        }

        public IEnumerator<object> GetEnumerator()
        {
            for (int i = 0; i < nodes.Count; i++)
                yield return nodes[i];
        }

        public int Count
        { get { return nodes.Count; } }

        public string ToLaTeX()
        {
            string str = "";
            int i, j;

            bool[,] tab = new bool[nodes.Count, nodes.Count];

            for (i = 0; i < nodes.Count; i++)
            {
                for (j = 0; j < nodes.Count; j++)
                    tab[i, j] = false;
                for (j = 0; j < nodes[i].Adjacency.Count; j++)
                    tab[i, nodes[i].Adjacency[j]] = true;
            }

            str += "\\begin{tabular}{c";
            for (i = 0; i < nodes.Count; i++)
                str += "|c";
            str += "}\n";
            for (i = 0; i < nodes.Count; i++)
                str += " & " + this[i].Vertex.Key;
            str += " \\\\\n";
            str += "\\hline\n";

            for (i = 0; i < nodes.Count; i++)
            {
                str += this[i].Vertex.Key;

                for (j = 0; j < nodes.Count; j++)
                {
                    if (tab[i, j])
                        str += " & " + this[j].Vertex.Key;
                    else
                        str += " & ";
                }
                str += " \\\\\n";
            }
            str += "\\end{tabular}\n";

            return str;
        }

        public Node[] BFS1(int s)
        {
            Node[] pi = new Node[Count];
            int[] color = new int[Count];
            int[] d = new int[Count];

            for (int u = 0; u < Count; u++)
            {
                color[u] = 0; // WHITE
                d[u] = int.MaxValue;
                pi[u] = null;
            }

            color[s] = 1; // GRAY
            d[s] = 0;
            pi[s] = null;
            Queue<object> Q = new Queue<object>();
            Q.Enqueue(s);
            while (Q.Count > 0)
            {
                int u = (int)Q.Dequeue();

                foreach (int v in this[u].Adjacency)
                {
                    if (color[v] == 0) // WHITE
                    {
                        color[v] = 1; // GRAY
                        d[v] = d[u] + 1;
                        pi[v] = this[u];
                        Q.Enqueue(v);
                    }
                }
                color[u] = 2;
            }

            return pi;
        }

        public int[] BFS(int s)
        {
            int[] color = new int[Count];
            int[] d = new int[Count];
            int[] pi = new int[Count];

            for (int u = 0; u < Count; u++)
            {
                color[u] = 0; // WHITE
                d[u] = int.MaxValue;
                pi[u] = -1;
            }

            color[s] = 1; // GRAY
            d[s] = 0;
            pi[s] = -1;
            Queue<object> Q = new Queue<object>();
            Q.Enqueue(s);
            while (Q.Count > 0)
            {
                int u = (int)Q.Dequeue();

                foreach (int v in this[u].Adjacency)
                {
                    if (color[v] == 0) // WHITE
                    {
                        color[v] = 1; // GRAY
                        d[v] = d[u] + 1;
                        pi[v] = u;
                        Q.Enqueue(v);
                    }
                }
                color[u] = 2;
            }

            return pi;
        }

        public Graph BFTree(ref int s)
        {
            int[] pi = BFS(s);
            int[] pos = new int[Count];
            Graph tree = new Graph();

            for (int i = 0; i < Count; i++)
                pos[i] = -1;

            tree.AddVertex(this[s].Vertex);
            pos[0] = s;
            int p = 1;
            for (int i = 0; i < Count; i++)
            {
                if (i != s && pi[i] != -1)
                {
                    int k;

                    tree.AddVertex(this[i].Vertex);
                    for (k = 0; k < p; k++)
                        if (pos[k] == pi[i])
                            break;
                    tree.AddEdge(i, k);
                    pos[p] = i;
                    p++;
                }
            }

            s = 0;

            return tree;
        }

        public int[] DFS()
        { 
            int[] pi = new int[Count];
            int[] color = new int[Count];
            int[] d = new int[Count];
            int[] f = new int[Count];
            int time;

            for (int u = 0; u < Count; u++)
            {
                color[u] = 0; // WHITE
                pi[u] = -1;
            }
            time = 0;
            for (int u = 0; u < Count; u++)
                if (color[u] == 0)  // WHITE
                    DFS_VISIT(u, ref color, ref time, ref d, ref f, ref pi);
            return pi;
        }

        private void DFS_VISIT(int u, ref int[] color, ref int time, ref int[] d, ref int[] f, ref int[] pi)
        {
            color[u] = 1; // GRAY
            time++;
            d[u] = time;
            foreach (int v in this[u].Adjacency)
            {
                if (color[v] == 0) // WHITE
                {
                    pi[v] = u;
                    DFS_VISIT(v, ref color, ref time, ref d, ref f, ref pi);
                }
            }
            color[u] = 2; // BLACK
            time++;
            f[u] = time;
        }

        public Graph[] DFForest()
        {
            bool[] root = new bool[Count];
            int[] pi = DFS();

            int nTrees = 0;
            for (int i = 0; i < Count; i++)
            {
                if (pi[i] == -1)
                {
                    nTrees++;
                    root[i] = true;
                }
                else
                    root[i] = false;
            }

            Graph[] tree = new Graph[nTrees];

            int t = 0;

            for (int s = 0; s < Count; s++)
            {
                if (root[s])
                {
                    tree[t] = new Graph();

                    tree[t].AddVertex(this[s].Vertex);

                    for (int i = 0; i < Count; i++)
                    {
                        if (i != s && pi[i] != -1)
                        {
                            tree[t].AddVertex(this[i].Vertex);
                            tree[t].AddEdge(i, pi[i]);
                        }
                    }
                }
            }

            return tree;
        }
    }

    public class LGraph : KeyedData 
    {
        protected List<LNode> nodes;
        private readonly string key;

        public LGraph(string key)
        {
            nodes = new List<LNode>();
            this.key = key;
        }

        public override string Key
        { get { return key; } }

        public LNode this[int i]
        { get { Debug.Assert(i >= 0 && i < Count); return nodes[i]; } }

        public LNode AddVertex(KeyedData vertex)
        {
            foreach (LNode u in this)
                if (u.Vertex.Key == vertex.Key)
                    return u;
            LNode node = new LNode(vertex);
            nodes.Add(node);
            return node;
        }

        public LNode this[string key]
        { get { return GetNode(key); } }

        public void AddEdge(LNode from, LNode to)
        {
            foreach (LNode n in from.Successor)
                if (n.Vertex.Key == to.Vertex.Key)
                    return;
            from.Successor.Add(to);
            to.Predecessor.Add(from);
        }

        public IEnumerator<object> GetEnumerator()
        {
            for (int i = 0; i < nodes.Count; i++)
                yield return nodes[i];
        }

        public int Count
        { get { return nodes.Count; } }

        public LGraph Transpose()
        { 
            LGraph transp = new LGraph("\\{" + key + "\\}^T");

            foreach (LNode u in this)
                transp.AddVertex(u.Vertex);
            foreach (LNode u in this)
                foreach (LNode v in u.Successor)
                    transp.AddEdge(transp[v.Vertex.Key], transp[u.Vertex.Key]);

            return transp;
        }

        public LNode FindVertice(string key)
        {
            foreach (LNode n in this)
                if (n.Vertex.Key == key)
                    return n;
            return null;
        }

        private LNode GetNode(string key)
        {
            foreach (LNode u in this)
                if (u.Vertex.Key == key)
                    return u;
            return null;
        }

        public LGraph StronglyConnectedComponents()
        {
            // Chama DFS para computar os tempos finais f[u] para cada vertice u
            List<KeyedData> Q1 = DFS();
            // A segunda invocacao de DFS adiante, requer que os vertices sejam 
            // analisados em ordem decrescente de tempos finais 
            for (int i = 0; i < Q1.Count / 2; i++)
            {
                KeyedData tmp = (KeyedData)Q1[i];
                Q1[i] = Q1[Q1.Count - i - 1];
                Q1[Q1.Count - i - 1] = tmp;
            }
            // Calcula o grafo transposta
            LGraph T = Transpose();
            // Segunda invocacao de DFS eh sobre o grafo transposto
            LGraph[] forest = T.DFForest(Q1);

            // Cada subconjunto de vertices de cada arvore da floresta forest 
            // determina um componente fortemente conectado
            ////////////////////////////////////////////////////////////
            // A partir daqui, vai construir um grafo cujos vertices 
            // sao grafos conectados
            //
            List<KeyedData>[] vertex = new List<KeyedData>[forest.Length];

            // Primeiro particiona o conjunto de vertices
            for (int t = 0; t < forest.Length; t++)
            {
                string key = "";
                vertex[t] = new List<KeyedData>();
                for (int i = 0; i < forest[t].Count; i++)
                {
                    key += forest[t][i].ToString();
                    vertex[t].Add(forest[t][i].Vertex);
                }
            }

            // Conjunto de subgrafos estritamente conectados
            LGraph[] node = new LGraph[vertex.Length];

            // Matrix relacao
            bool[,] mat = ToMatrix();
            // Selecionamento das linhas e colunas da matrix relacao
            bool[,] selected = new bool[node.Length, Count];

            // Para cada subconjunto de vertices do particionamento, 
            // constroi um novo grafo
            for (int t = 0; t < node.Length; t++)
            {
                string key = "";
                for (int v = 0; v < vertex[t].Count; v++)
                    key += vertex[t][v].Key;
                node[t] = new LGraph(key);
                for (int v = 0; v < vertex[t].Count; v++)
                    node[t].AddVertex(vertex[t][v]);
            }

            // Selecionamento das colunas da matrix relacao em funcao do 
            // particionamento do conjunto de vertices
            for (int t = 0; t < node.Length; t++)
                for (int i = 0; i < Count; i++)
                    if (node[t].FindVertice(nodes[i].Vertex.Key) != null)
                        selected[t, i] = true;
                    else
                        selected[t, i] = false;

            // Insere as transicoes dentro dos subgrafos
            for (int t = 0; t < node.Length; t++)
                for (int i = 0; i < Count; i++)
                    for (int j = 0; j < Count; j++)
                        if (selected[t, i] && selected[t, j] && mat[i, j])
                            node[t].AddEdge(
                                node[t].FindVertice(nodes[i].Vertex.Key),
                                node[t].FindVertice(nodes[j].Vertex.Key)
                                );

            // Elimina as transicoes internas aos subgrafos da matrix relacao
            for (int t = 0; t < node.Length; t++)
                for (int i = 0; i < Count; i++)
                    for (int j = 0; j < Count; j++)
                        if (selected[t, i] && selected[t, j])
                            mat[i, j] = false;

            // Constroi o grafo cujos vertices sao subgrafos
            LGraph gr = new LGraph(Key + "_SCC");

            // Insere os subgrafos como vertices do grafo resultante
            for (int t = 0; t < node.Length; t++)
                gr.AddVertex(node[t]);

            // Adiciona as transicoes entre subgrafos no grafo resultante
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    if (mat[i, j])
                    {
                        int from, to;

                        for (from = 0; from < node.Length; from++)
                            if (selected[from, i])
                                break;
                        for (to = 0; to < node.Length; to++)
                            if (selected[to, j])
                                break;
                        gr.AddEdge(gr[from], gr[to]);
                    }
                }
            }

            // Retorna o grafo resultante
            return gr;
        }

        public void BFS(int s)
        {
            foreach (LNode u in this)
            {
                u.color = COLOR.WHITE;
                u.d = int.MaxValue;
                u.pi = null;
            }

            nodes[s].color = COLOR.GRAY;
            nodes[s].d = 0;
            nodes[s].pi = null;
            Queue<object> Q = new Queue<object>();
            Q.Enqueue(nodes[s]);
            while (Q.Count > 0)
            {
                LNode u = (LNode)Q.Dequeue();

                foreach (LNode v in u.Successor)
                {
                    if (v.color == COLOR.WHITE)
                    {
                        v.color = COLOR.GRAY;
                        v.d = u.d + 1;
                        v.pi = u;
                        Q.Enqueue(v);
                    }
                }
                u.color = COLOR.BLACK;
            }
        }

        public LGraph BFTree(int s)
        {
            LGraph tree = new LGraph("BFTree");

            BFS(s);

            tree.AddVertex(this[s].Vertex);

            for (int i = 0; i < Count; i++)
            {
                if (i != s && this[i].pi != null)
                {
                    LNode from = tree.AddVertex(this[i].Vertex);
                    LNode to = tree.AddVertex(this[i].pi.Vertex);
                    tree.AddEdge(from, to);
                }
            }

            return tree;
        }

        public List<KeyedData> DFS()
        {
            int time = 0;

            List<KeyedData> tSort = new List<KeyedData>();

            foreach (LNode u in this)
            {
                u.color = COLOR.WHITE;
                u.pi = null;
            }
            foreach (LNode u in this)
                if (u.color == COLOR.WHITE)
                    DFS_VISIT(u, ref time, ref tSort);

            return tSort;
        }

        public List<KeyedData> DFS(List<KeyedData> Q)
        {
            int time = 0;

            List<KeyedData> tSort = new List<KeyedData>();

            foreach (LNode u in nodes)
            {
                u.color = COLOR.WHITE;
                u.pi = null;
            }
            foreach (KeyedData data in Q)
            {
                LNode u = this[data.Key];
                if (u.color == COLOR.WHITE)
                    DFS_VISIT(u, ref time, ref tSort);
            }

            return tSort;
        }

        private void DFS_VISIT(LNode u, ref int time, ref List<KeyedData> tSort)
        {
            u.color = COLOR.GRAY;
            time++;
            u.d = time;
            foreach (LNode v in u.Successor)
            {
                if (v.color == COLOR.WHITE)
                {
                    v.pi = u;
                    DFS_VISIT(v, ref time, ref tSort);
                }
            }
            u.color = COLOR.BLACK;
            time++;
            u.f = time;
            tSort.Add(u.Vertex);
        }

        public LGraph[] DFForest(List<KeyedData> Q)
        {
            bool[] root = new bool[Count];

            List<KeyedData> tSort = DFS(Q);

            int nTrees = 0;
            for (int i = 0; i < Count; i++)
            {
                if (nodes[i].pi == null)
                {
                    nTrees++;
                    root[i] = true;
                }
                else
                    root[i] = false;
            }

            LGraph[] tree = new LGraph[nTrees];

            int t = 0;

            for (int s = 0; s < Count; s++)
            {
                if (root[s])
                {
                    tree[t] = new LGraph("DFForest");

                    tree[t].AddVertex(this[s].Vertex);

                    Queue<object> q = new Queue<object>();

                    foreach (LNode u in this[s].Successor)
                        if (u.pi == this[s])
                            q.Enqueue(u);

                    while (q.Count > 0)
                    {
                        LNode u = (LNode)q.Dequeue();

                        LNode from = tree[t].AddVertex(u.Vertex);
                        LNode to = tree[t].AddVertex(u.pi.Vertex);
                        tree[t].AddEdge(from, to);

                        foreach (LNode v in u.Successor)
                            if (v.pi == u)
                                q.Enqueue(v);
                    }
                    t++;
                }
            }

            return tree;
        }

        public LGraph[] DFForest()
        {
            bool[] root = new bool[Count];

            List<KeyedData> tSort = DFS();

            int nTrees = 0;
            for (int i = 0; i < Count; i++)
            {
                if (nodes[i].pi == null)
                {
                    nTrees++;
                    root[i] = true;
                }
                else
                    root[i] = false;
            }

            LGraph[] tree = new LGraph[nTrees];

            int t = 0;

            for (int s = 0; s < Count; s++)
            {
                if (root[s])
                {
                    tree[t] = new LGraph("DFForest");

                    tree[t].AddVertex(this[s].Vertex);

                    Queue<object> q = new Queue<object>();

                    foreach (LNode u in this[s].Successor)
                        if (u.pi == this[s])
                            q.Enqueue(u);

                    while (q.Count > 0)
                    {
                        LNode u = (LNode)q.Dequeue();

                        LNode from = tree[t].AddVertex(u.Vertex);
                        LNode to = tree[t].AddVertex(u.pi.Vertex);
                        tree[t].AddEdge(from, to);

                        foreach (LNode v in u.Successor)
                            if (v.pi == u)
                                q.Enqueue(v);
                    }

                    t++;
                }
            }

            return tree;
        }

        public bool[,] ToMatrix()
        { 
            int i, j;

            bool[,] tab = new bool[nodes.Count, nodes.Count];

            for (i = 0; i < nodes.Count; i++)
            {
                for (j = 0; j < nodes.Count; j++)
                    tab[i, j] = false;
                for (j = 0; j < nodes[i].Successor.Count; j++)
                {
                    if (nodes[i].Successor[j] != null)
                    {
                        for (int k = 0; k < Count; k++)
                        {
                            if (nodes[k].Vertex.Key == nodes[i].Successor[j].Vertex.Key)
                            {
                                tab[i, k] = true;
                                break;
                            }
                        }
                    }
                }
            }

            return tab;
        }

        public string ToLaTeX()
        {
            string str = "";
            int i, j;

            bool[,] tab = ToMatrix();

            str += "\\begin{tabular}{c";
            for (i = 0; i < nodes.Count; i++)
                str += "|c";
            str += "}\n";
            for (i = 0; i < nodes.Count; i++)
                str += " & " + this[i].Vertex.Key;
            str += " \\\\\n";
            str += "\\hline\n";

            for (i = 0; i < nodes.Count; i++)
            {
                str += this[i].Vertex.Key;

                for (j = 0; j < nodes.Count; j++)
                {
                    if (tab[i, j])
                        str += " & 1";
                    else
                        str += " & 0";
                }
                str += " \\\\\n";
            }
            str += "\\end{tabular}\n";

            return str;
        }
    }
}
