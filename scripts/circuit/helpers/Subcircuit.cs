using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

public sealed class Subcircuit
{
    private readonly HashSet<Vector2I> nets;
    private readonly Func<Pin, Vector2I> netOf;
    private readonly List<Pin> pins = new();
    private readonly List<Component> components = new();
    private readonly List<CurrentEquation> vSourceComponents = new();
    private readonly Dictionary<Vector2I, int> netToIndex = new();
    private Vector2I groundNet;
    public double[,] Garray; // n x n
    public double[,] Barray; // n x m
    public double[,] Carray; // m x n
    public double[,] Darray; // m x m
    public double[] iArray; // n
    public double[] eArray; // m
    private Dictionary<int, List<Component>> compsByNode;

    public Subcircuit(HashSet<Vector2I> nets, Func<Pin, Vector2I> netOf)
    {
        this.nets = nets;
        this.netOf = netOf;
    }

    public void Initialize(
        IEnumerable<Pin> allPins,
        IEnumerable<Component> allComponents,
        Func<HashSet<Vector2I>, IEnumerable<Pin>, Vector2I> chooseGround)
    {
        //find useable pins/components:
        foreach (var p in allPins)
            if (nets.Contains(netOf(p))) pins.Add(p);
        components.Clear();
        foreach (var c in allComponents)
            if (c.pins != null && c.pins.Any(p => nets.Contains(netOf(p))) && c.IsActive) components.Add(c);
        groundNet = chooseGround(nets, pins);
        int idx = 0;
        // deterministic ordering:
        foreach (var net in nets.OrderBy(v => v.X).ThenBy(v => v.Y))
            if (net != groundNet) netToIndex[net] = idx++;

        // find m and n:
        foreach (var p in pins)
            p.netIndex = (netOf(p) == groundNet) ? -1 : netToIndex[netOf(p)];
        vSourceComponents.Clear();
        foreach (var c in components)
            if (c.componentProperty is CurrentEquation vsp && c.IsActive) vSourceComponents.Add(vsp);

        for (int k = 0; k < vSourceComponents.Count; ++k)
            vSourceComponents[k].vIndex = k;
        int n = netToIndex.Count;
        int m = vSourceComponents.Count;

        Garray = new double[n, n];
        Barray = new double[n, m];
        Carray = new double[m, n];
        Darray = new double[m, m];
        iArray = new double[n];
        eArray = new double[m];
    }
    public void StampAll()
    {
        foreach (var c in components)
        {
            Garray = c.componentProperty.GStamp(Garray, c.pins);
            Barray = c.componentProperty.BStamp(Barray, c.pins);
            Carray = c.componentProperty.CStamp(Carray, c.pins);
            Darray = c.componentProperty.DStamp(Darray);

            eArray = c.componentProperty.eStamp(eArray);
            iArray = c.componentProperty.iStamp(iArray, c.pins);
        }
    }
    private void IndexComponentsByNode()
    {
        compsByNode = new Dictionary<int, List<Component>>();
        foreach (var c in components)
        {
            foreach (var p in c.pins)
            {
                if (p.netIndex < 0) continue;
                if (!compsByNode.TryGetValue(p.netIndex, out var list))
                {
                    list = new List<Component>();
                    compsByNode[p.netIndex] = list;
                }
                list.Add(c);
            }
        }
    }

    private double EstimateLocalSeriesR(int n0, int n1)
    {
        double r = double.PositiveInfinity;
        bool vsp = false;

        void probeNode(int node)
        {
            if (node < 0) return;
            if (!compsByNode.TryGetValue(node, out var list)) return;
            foreach (var c in list)
            {
                if (c.componentProperty is VoltageSourceProperty vs)
                {
                    vsp = true;
                    r = Math.Min(r, Math.Max(0.0, vs.R_series));
                }
                if (c.componentProperty is ResistorProperty res)
                {
                    r = Math.Min(r, Math.Max(0.0, res.R));
                }
            }
        }
        probeNode(n0);
        probeNode(n1);

        if (double.IsPositiveInfinity(r) || vsp) r = 1e-6;
        if (!vsp) r = double.PositiveInfinity;
        return r;
    }


    public void Solve()
    {
        IndexComponentsByNode();
        foreach (var c in components)
        {
            if (c.componentProperty is CapacitorProperty cap)
            {
                int n0 = c.pins[0].netIndex, n1 = c.pins[1].netIndex;
                double Rser = EstimateLocalSeriesR(n0, n1);
                double tau = Rser * cap.C;
                if (CircuitComputer.dt > 1.0 * Math.Max(1e-15, tau)) cap.SetTheta(1.0);
                else cap.SetTheta(0.5);

            }
            else if (c.componentProperty is InductorProperty ind)
            {
                int n0 = c.pins[0].netIndex, n1 = c.pins[1].netIndex;
                double Rser = EstimateLocalSeriesR(n0, n1);
                double tau = Rser * ind.L;
                if (CircuitComputer.dt > 1.0 * Math.Max(1e-15, tau)) ind.SetTheta(1.0);
                else ind.SetTheta(0.5);
            }
        }
        int n = Garray.GetLength(0);
        int m = Darray.GetLength(0);
        if (n + m == 0) return;
        int N = n + m;
        double[,] A = new double[N, N];
        double[] z = new double[N];
        // G
        for (int r = 0; r < n; ++r)
            for (int c = 0; c < n; ++c)
                A[r, c] = Garray[r, c];

        // B
        for (int r = 0; r < n; ++r)
            for (int c = 0; c < m; ++c)
                A[r, n + c] = Barray[r, c];

        // C
        for (int r = 0; r < m; ++r)
            for (int c = 0; c < n; ++c)
                A[n + r, c] = Carray[r, c];

        // D
        for (int r = 0; r < m; ++r)
            for (int c = 0; c < m; ++c)
                A[n + r, n + c] = Darray[r, c];

        for (int r = 0; r < n; ++r) z[r] = iArray[r];
        for (int r = 0; r < m; ++r) z[n + r] = eArray[r];

        var matrixA = Matrix<double>.Build.DenseOfArray(A);

        var vectorZ = Vector<double>.Build.Dense(z);


        var x = matrixA.Solve(vectorZ);
        if (x == null) return;

        //push solution back to net!
        foreach (var p in pins) p.solvedVoltage = (p.netIndex >= 0) ? x[p.netIndex] : 0.0;
        foreach (var comp in components)
        {
            (comp.solvedVoltage, comp.solvedCurrent) = comp.componentProperty.ComputeCurrents(comp.pins, x.ToArray(), n);
        }
    }
    public static void Print2DArray<T>(T[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                GD.Print(matrix[i, j] + "\t");
            }
            GD.Print();
        }
    }
}
