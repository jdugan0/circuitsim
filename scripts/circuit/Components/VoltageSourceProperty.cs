using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class VoltageSourceProperty : ComponentProperty
{
    [Export] public float voltage = 0f;
    public int vIndex = -1;

    public override double[,] BStamp(double[,] B, Pin[] pins)
    {
        int neg = pins[0].netIndex, pos = pins[1].netIndex;
        MnaUtil.Add(ref B, neg, vIndex, -1);
        MnaUtil.Add(ref B, pos, vIndex, +1);
        return B;
    }
    public override double[,] CStamp(double[,] C, Pin[] pins)
    {
        int neg = pins[0].netIndex, pos = pins[1].netIndex;
        MnaUtil.Add(ref C, vIndex, neg, -1);
        MnaUtil.Add(ref C, vIndex, pos, +1);
        return C;
    }
    public override double[] eStamp(double[] e)
    {
        MnaUtil.Set(ref e, vIndex, voltage);
        return e;
    }
    public override void ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        current = x[n + vIndex];
    }
}
