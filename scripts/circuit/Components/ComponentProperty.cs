using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class ComponentProperty : Resource
{
    public virtual double[,] GStamp(double[,] G, Pin[] pins) => G;
    public virtual double[,] DStamp(double[,] D) => D;
    public virtual double[,] BStamp(double[,] B, Pin[] pins) => B;
    public virtual double[,] CStamp(double[,] C, Pin[] pins) => C;
    public virtual double[] eStamp(double[] e) => e;
    public virtual double[] iStamp(double[] i, Pin[] p) => i;
    public virtual (double voltage, double current) ComputeCurrents(Pin[] pins, double[] fullSolution, int n) { return (0, 0); }
    [Export] public double maxEnergy = 10;
    [Export] public double wattage = 5;
}