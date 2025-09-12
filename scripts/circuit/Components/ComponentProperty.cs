using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class ComponentProperty : Resource
{
    public double current;
    public virtual double[,] GStamp(double[,] G, Pin[] pins) => G;
    public virtual double[,] DStamp(double[,] D) => D;
    public virtual double[,] BStamp(double[,] B, Pin[] pins) => B;
    public virtual double[,] CStamp(double[,] C, Pin[] pins) => C;
    public virtual double[] eStamp(double[] e) => e;
    public virtual void ComputeCurrents(Pin[] pins, double[] fullSolution, int n){}
}