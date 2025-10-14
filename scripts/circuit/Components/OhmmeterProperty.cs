using Godot;
using System;

[GlobalClass]
public partial class OhmmeterProperty : ComponentProperty
{
    [Export] public double testCurrent = 1e-9;

    public override double[] iStamp(double[] i, Pin[] pins)
    {
        if (pins[1].netIndex >= 0) i[pins[1].netIndex] += testCurrent;
        if (pins[0].netIndex >= 0) i[pins[0].netIndex] -= testCurrent;
        return i;
    }

    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        double voltage = pins[1].solvedVoltage - pins[0].solvedVoltage;
        double r = voltage / testCurrent;
        if (r > 1e5) r = double.PositiveInfinity;
        return (r, testCurrent);
    }
}