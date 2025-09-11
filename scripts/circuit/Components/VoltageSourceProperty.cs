using Godot;
using System;
[GlobalClass]
public partial class VoltageSourceProperty : ComponentProperty
{
    [Export] public float voltage;
    public int vCount = 0;
    public override double[,] BStamp(double[,] B, Pin[] pins)
    {
        int neg = pins[0].netIndex;
        int pos = pins[1].netIndex;
        B[neg, vCount] -= 1;
        B[pos, vCount] += 1;
        return B;
    }
    public override double[,] CStamp(double[,] B, Pin[] pins)
    {
        int neg = pins[0].netIndex;
        int pos = pins[1].netIndex;
        B[vCount, neg] -= 1;
        B[vCount, pos] += 1;
        return B;
    }
    public override double[] eStamp(double[] e)
    {
        e[vCount] = voltage;
        return e;
    }


}
