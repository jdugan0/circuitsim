using Godot;
using System;
[GlobalClass]
public partial class InductorProperty : CurrentEquation
{
    [Export] public double L = 1e-3;
    private double iPrev = 0.0;
    public override double[,] BStamp(double[,] B, Pin[] pins)
    {
        int n0 = pins[0].netIndex;
        int n1 = pins[1].netIndex;
        MnaUtil.Add(ref B, n0, vIndex, +1);
        MnaUtil.Add(ref B, n1, vIndex, -1);
        return B;
    }

    public override double[,] CStamp(double[,] C, Pin[] pins)
    {
        int n0 = pins[0].netIndex;
        int n1 = pins[1].netIndex;
        MnaUtil.Add(ref C, vIndex, n0, +1);
        MnaUtil.Add(ref C, vIndex, n1, -1);
        return C;
    }

    public override double[,] DStamp(double[,] D)
    {
        double req = L / Math.Max(CircuitComputer.dt, 1e-15);
        MnaUtil.Add(ref D, vIndex, vIndex, -req);
        return D;
    }

    public override double[] eStamp(double[] e)
    {
        double req = L / Math.Max(CircuitComputer.dt, 1e-15);
        double Vhist = -req * iPrev;
        e[vIndex] += Vhist;
        return e;
    }

    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int n)
    {
        double I = fullSolution[n + vIndex];
        iPrev = I;
        double V = pins[0].solvedVoltage - pins[1].solvedVoltage;
        return (V, I);
    }


}
