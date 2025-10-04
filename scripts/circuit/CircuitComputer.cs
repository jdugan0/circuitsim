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
    public static double dt;
    Pin[] pins;
    Wire[] wires;
    Component[] components;
    [Export] PlacementManager placementManager;
    Dictionary<Vector2I, HashSet<Vector2I>> groups = new Dictionary<Vector2I, HashSet<Vector2I>>();
    public override void _Ready()
    {
        placementManager.OnGridChange += UpdateDSU;
        UpdateDSU();
    }

    public void UpdateDSU()
    {
        pins = Array.ConvertAll<Node, Pin>(GetTree().GetNodesInGroup("pin").ToArray<Node>(), (x) => (Pin)x);
        wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);
        components = Array.ConvertAll<Node, Component>(GetTree().GetNodesInGroup("component").ToArray<Node>(), (x) => (Component)x);
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
            var startGc = w.startCell.GetGridCoord();
            var endGc = w.endCell.GetGridCoord();
            dsu.Add(startGc);
            dsu.Add(endGc);
            dsu.Union(startGc, endGc);

            foreach (var cell in w.CoveredCells())
            {
                if (pinsAtCell.TryGetValue(cell, out var overlappedPins))
                {
                    foreach (var p in overlappedPins)
                        dsu.Union(startGc, p.GetGridCoord());
                }
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
        groups = new Dictionary<Vector2I, HashSet<Vector2I>>();
        foreach (var net in nets)
        {
            var root = netDsu.Find(net);
            if (!groups.ContainsKey(root)) groups[root] = new HashSet<Vector2I>();
            groups[root].Add(net);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        dt = delta;

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

}
