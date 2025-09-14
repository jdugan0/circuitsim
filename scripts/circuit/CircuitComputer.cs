using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public partial class CircuitComputer : Node
{
    Dictionary<Pin, Vector2I> pinNet = new();
    private Dictionary<Vector2I, int> netToIndex = new();
    public override void _Process(double delta)
    {
        Pin[] pins = Array.ConvertAll<Node, Pin>(GetTree().GetNodesInGroup("pin").ToArray<Node>(), (x) => (Pin)x);
        Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);
        Component[] components = Array.ConvertAll<Node, Component>(GetTree().GetNodesInGroup("component").ToArray<Node>(), (x) => (Component)x);

        var dsu = new DSU<Vector2I>();
        var pinsAtCell = new Dictionary<Vector2I, List<Pin>>();

        foreach (var p in pins)
        {
            var gc = p.GetGridCoord();
            dsu.Add(gc);
            if (!pinsAtCell.TryGetValue(gc, out var list))
            {
                list = new List<Pin>();
                pinsAtCell[gc] = list;
            }
            list.Add(p);
        }
        foreach (var w in wires)
        {
            Vector2I? prev = null;
            foreach (var c in w.CoveredCells())
            {
                dsu.Add(c);
                if (prev.HasValue) dsu.Union(prev.Value, c);
                if (pinsAtCell.TryGetValue(c, out var plist))
                    foreach (var p in plist) dsu.Union(c, p.GetGridCoord());
                prev = c;
            }
        }
        pinNet.Clear();
        foreach (var p in pins) pinNet[p] = dsu.Find(p.GetGridCoord());



        var nets = pinNet.Values.Distinct().ToList();
        // GD.Print(nets.Count);
        var netDsu = new DSU<Vector2I>();
        foreach (var net in nets) netDsu.Add(net);
        foreach (var c in components)
        {
            var n0 = pinNet[c.pins[0]];
            for (int k = 1; k < c.pins.Length; k++)
                netDsu.Union(n0, pinNet[c.pins[k]]);
        }
        var groups = new Dictionary<Vector2I, HashSet<Vector2I>>();
        foreach (var net in nets)
        {
            var root = netDsu.Find(net);
            if (!groups.ContainsKey(root)) groups[root] = new HashSet<Vector2I>();
            groups[root].Add(net);
        }

        foreach (var kv in groups)
        {
            var sub = new Subcircuit(kv.Value, (x) => pinNet[x]);

            sub.Initialize(
                allPins: pins,
                allComponents: components,
                chooseGround: ChooseGround
            );

            sub.StampAll();
            sub.Solve();
        }
        // GD.Print(groups.Count); 
    }
    private Vector2I ChooseGround(HashSet<Vector2I> netsInIsland, IEnumerable<Pin> pinsInIsland)
    {
        var groundPin = pinsInIsland.FirstOrDefault(p => p.isGround);
        if (groundPin != null) return pinNet[groundPin];

        return netsInIsland.OrderBy(v => v.X).ThenBy(v => v.Y).First();
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
