using System;
using Godot;

[GlobalClass]
public partial class CapacitorProperty : ComponentProperty
{
    [Export] public double C = 1e-6;
    private double vPrev = 0.0;
    private double iPrev = 0.0;

    public override double[,] GStamp(double[,] G, Pin[] pins)
    {
        double g = 2.0 * C / Math.Max(CircuitComputer.dt, 1e-15);
        int n0 = pins[0].netIndex;
        int n1 = pins[1].netIndex;

        MnaUtil.Add(ref G, n0, n0,  g);
        MnaUtil.Add(ref G, n1, n1,  g);
        MnaUtil.Add(ref G, n0, n1, -g);
        MnaUtil.Add(ref G, n1, n0, -g);
        return G;
    }

    public override double[] iStamp(double[] i, Pin[] pins)
    {
        double g = 2.0 * C / Math.Max(CircuitComputer.dt, 1e-15);
        double Ieq = g * vPrev + iPrev;

        int n0 = pins[0].netIndex;
        int n1 = pins[1].netIndex;

        if (n1 >= 0) i[n1] += Ieq;
        if (n0 >= 0) i[n0] -= Ieq;
        return i;
    }

    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int n)
    {
        double vn = pins[0].solvedVoltage;
        double vp = pins[1].solvedVoltage;
        double v = vp - vn;
        
        double g = 2.0 * C / Math.Max(CircuitComputer.dt, 1e-15);
        double I = g * (v - vPrev) - iPrev;

        vPrev = v;
        iPrev = I;

        return (v, I);
    }
}