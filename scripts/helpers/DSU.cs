using System.Collections.Generic;

public class DSU<T>
{
    private readonly Dictionary<T, T> parent = new();
    private readonly Dictionary<T, int> rank = new();

    public void Add(T x)
    {
        if (parent.ContainsKey(x)) return;
        parent[x] = x; rank[x] = 0;
    }

    public T Find(T x)
    {
        var p = parent[x];
        if (!p.Equals(x)) parent[x] = Find(p);
        return parent[x];
    }

    public void Union(T a, T b)
    {
        a = Find(a); b = Find(b);
        if (a.Equals(b)) return;
        if (rank[a] < rank[b]) (a, b) = (b, a);
        parent[b] = a;
        if (rank[a] == rank[b]) rank[a]++;
    }
}
