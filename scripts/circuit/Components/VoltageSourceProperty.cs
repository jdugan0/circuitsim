using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class VoltageSourceProperty : CurrentEquation
{
    [Export] public float voltage = 0f;
    [Export] public double R_series = 0;

    public override double[,] BStamp(double[,] B, Pin[] pins)
    {
        int neg = pins[0].netIndex;
        int pos = pins[1].netIndex;
        MnaUtil.Add(ref B, neg, vIndex, -1);
        MnaUtil.Add(ref B, pos, vIndex, +1);
        return B;
    }
    public override double[,] CStamp(double[,] C, Pin[] pins)
    {
        int neg = pins[0].netIndex;
        int pos = pins[1].netIndex;
        MnaUtil.Add(ref C, vIndex, neg, -1);
        MnaUtil.Add(ref C, vIndex, pos, +1);
        return C;
    }

    public override double[,] DStamp(double[,] D)
    {
        MnaUtil.Add(ref D, vIndex, vIndex, -R_series);
        return D;
    }

    public override double[] eStamp(double[] e)
    {
        MnaUtil.Set(ref e, vIndex, voltage);
        return e;
    }
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        double current = x[n + vIndex];
        double terminalVoltage = pins[1].solvedVoltage - pins[0].solvedVoltage;
        return (terminalVoltage, current);
    }
}