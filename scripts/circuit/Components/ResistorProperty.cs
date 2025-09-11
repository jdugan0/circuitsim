using Godot;
using System;
[GlobalClass]
public partial class ResistorProperty : ComponentProperty
{
    [Export] double R;
    public override double[,] GStamp(double[,] G, Pin[] pins)
    {
        var n1 = pins[0].netIndex;
        var n2 = pins[1].netIndex;
        double g = 1.0 / R;
        MnaUtil.Add(ref G, n1, n1, g);
        MnaUtil.Add(ref G, n2, n2, g);
        MnaUtil.Add(ref G, n1, n2, -g);
        MnaUtil.Add(ref G, n2, n1, -g);
        return G;
    }
}
