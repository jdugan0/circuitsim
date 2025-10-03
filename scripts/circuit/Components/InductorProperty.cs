using System;
using Godot;
[GlobalClass]
public partial class InductorProperty : CurrentEquation
{
    [Export] public double L = 1e-3;
    private double iPrev = 0.0;

    public override double[,] BStamp(double[,] B, Pin[] pins)
    {
        int n = pins[0].netIndex;
        int p = pins[1].netIndex;
        MnaUtil.Add(ref B, p, vIndex, +1);
        MnaUtil.Add(ref B, n, vIndex, -1); 
        return B;
    }

    public override double[,] CStamp(double[,] C, Pin[] pins)
    {
        int n = pins[0].netIndex;
        int p = pins[1].netIndex;
        MnaUtil.Add(ref C, vIndex, p, +1);
        MnaUtil.Add(ref C, vIndex, n, -1);
        return C;
    }

    public override double[,] DStamp(double[,] D)
    {
        double Req = L / Math.Max(CircuitComputer.dt, 1e-15);
        MnaUtil.Add(ref D, vIndex, vIndex, -Req);
        return D;
    }

    public override double[] eStamp(double[] e)
    {
        double Req = L / Math.Max(CircuitComputer.dt, 1e-15);
        e[vIndex] += -Req * iPrev; 
        return e;
    }

    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int nUnknowns)
    {
        double I = fullSolution[nUnknowns + vIndex];
        double V = pins[1].solvedVoltage - pins[0].solvedVoltage;
        iPrev = I;
        return (V, I);
    }
}
