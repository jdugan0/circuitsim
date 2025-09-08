using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CircuitComputer : Node
{
    Dictionary<Pin, Vector2I> pinNet = new();
    public override void _Process(double delta)
    {
        Pin[] pins = Array.ConvertAll<Node, Pin>(GetTree().GetNodesInGroup("pin").ToArray<Node>(), (x) => (Pin)x);
        Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);

        var dsu = new DSU<Vector2I>();

        foreach (var p in pins) dsu.Add(p.GetGridCoord());
        foreach (var w in wires)
        {
            Vector2I? prev = null;
            foreach (var c in w.CoveredCells())
            {
                dsu.Add(c);
                if (prev.HasValue) dsu.Union(prev.Value, c);
                prev = c;
            }
        }

        foreach (var p in pins) pinNet[p] = dsu.Find(p.GetGridCoord());

        var nets = pinNet.Values.Distinct().ToList();
        var groundNet = nets[0];
        var nodeIndex = new Dictionary<Vector2I, int>();
        int idx = 0;
        foreach (var n in nets)
            if (n != groundNet) nodeIndex[n] = idx++;
        int nNodes = idx;

        int nVsrc = CountVoltageSources();
        int N = nNodes + nVsrc;
        var A = new double[N, N];
        var b = new double[N];
        var vsrcToAux = new Dictionary<Component, int>();
    }


    public bool AreConnected(Component A, Component B)
    {
        var aNets = A.pins.Select(p => pinNet[p]).ToHashSet();
        return B.pins.Any(p => aNets.Contains(pinNet[p]));
    }

}
