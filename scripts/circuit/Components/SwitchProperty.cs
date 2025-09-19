using Godot;
using System;

public partial class SwitchProperty : CurrentEquation
{
    public bool enabled;
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        if (enabled)
        {
            return (pins[1].solvedVoltage - pins[0].solvedVoltage, x[n + vIndex]);
        }
        return (0, 0);
    }
}
