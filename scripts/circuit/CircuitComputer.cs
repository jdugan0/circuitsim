using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public partial class CircuitComputer : Node
{
    Dictionary<Pin, Vector2I> pinNet = new();
    private Dictionary<Vector2I, int> netToIndex = new();
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
        int nodeCount = 0;
        foreach (var net in nets)
        {
            if (net == nets[0]) continue;
            netToIndex[net] = nodeCount++;
        }

        foreach (Pin p in pins) { p.netIndex = NodeIndexFor(p); }

        Component[] components = Array.ConvertAll<Node, Component>(GetTree().GetNodesInGroup("component").ToArray<Node>(), (x) => (Component)x);
        List<Component> vSourceComponents = new();
        List<Component> normalComponets = new();
        int vCount = 0;
        foreach (var c in components) if (c.componentProperty is VoltageSourceProperty v)
            { vSourceComponents.Add(c); v.vIndex = vCount; vCount++; }
            else { normalComponets.Add(c); }

        int n = normalComponets.Count;
        int m = vSourceComponents.Count;
        double[,] Garray = new double[n, n];
        double[,] Barray = new double[n, m];
        double[,] Carray = new double[m, n];
        double[,] Darray = new double[m, m];
        double[] eArray = new double[m];
        double[] iArray = new double[n];
        foreach (Component c in components)
        {
            //making A:
            Garray = c.componentProperty.GStamp(Garray, c.pins);
            Barray = c.componentProperty.BStamp(Barray, c.pins);
            Carray = c.componentProperty.CStamp(Carray, c.pins);
            Darray = c.componentProperty.DStamp(Darray);

            //making z:
            eArray = c.componentProperty.eStamp(eArray);
        }

    }
    public bool AreConnected(Component A, Component B)
    {
        var aNets = A.pins.Select(p => pinNet[p]).ToHashSet();
        return B.pins.Any(p => aNets.Contains(pinNet[p]));
    }

    public int NodeIndexFor(Pin p)
    {
        var net = pinNet[p];
        var nets = pinNet.Values.Distinct().ToList();
        return net == nets[0] ? -1 : netToIndex[net];
    }

}
