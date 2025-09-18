using Godot;
using System;
[GlobalClass]
public partial class AmmeterProperty : CurrentEquation
{
    public override (double voltage, double current) ComputeCurrents(Pin[] pins, double[] x, int n)
    {
        return (0, x[n + vIndex]);
    }
}
