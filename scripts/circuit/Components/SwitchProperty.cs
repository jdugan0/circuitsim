using Godot;
using System;

public partial class SwitchProperty : CurrentEquation
{
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        return (pins[1].solvedVoltage - pins[0].solvedVoltage, x[n + vIndex]);
    }
}
