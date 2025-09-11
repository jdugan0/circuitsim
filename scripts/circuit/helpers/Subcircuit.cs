using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class Subcircuit
{
    private readonly HashSet<Vector2I> nets;
    private readonly Func<Pin, Vector2I> netOf;
    private readonly List<Pin> pins = new();
    private readonly List<Component> components = new();
    private readonly List<VoltageSourceProperty> vSourceComponents = new();
    private readonly Dictionary<Vector2I, int> netToIndex = new();
    private Vector2I groundNet;
    public double[,] Garray; // n x n
    public double[,] Barray; // n x m
    public double[,] Carray; // m x n
    public double[,] Darray; // m x m
    public double[] iArray; // n
    public double[] eArray; // m

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

        foreach (var c in allComponents)
            if (c.pins != null && c.pins.Any(p => nets.Contains(netOf(p)))) components.Add(c);
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
            if (c.componentProperty is VoltageSourceProperty vsp) vSourceComponents.Add(vsp);

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
        }
    }
}
