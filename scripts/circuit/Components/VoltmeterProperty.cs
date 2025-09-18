using Godot;
using System;
[GlobalClass]
public partial class VoltmeterProperty : ComponentProperty
{
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int n)
    {
        double vp = pins[1].solvedVoltage;
        double vn = pins[0].solvedVoltage;
        return (vp - vn, 0);
    }
}
