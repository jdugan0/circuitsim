using Godot;
using System;
[GlobalClass]
public partial class VoltmeterProperty : ComponentProperty
{
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int n)
    {
        if (pins[0].disabled || pins[1].disabled)
        {
            return (0, 0);
        }
        double vp = pins[1].solvedVoltage;
        double vn = pins[0].solvedVoltage;
        return (vp - vn, 0);
    }
}
