using Godot;
using System;
[GlobalClass]
public partial class ComponentProperty : Resource
{
    public virtual double[,] GStamp(double[,] G, Pin[] pins)
    {
        return new double[G.GetLength(0), G.GetLength(1)];
    }
    public virtual double[,] DStamp(double[,] D)
    {
        return new double[D.GetLength(0), D.GetLength(1)];
    }
    public virtual double[,] BStamp(double[,] B, Pin[] pins)
    {
        return new double[B.GetLength(0), B.GetLength(1)];
    }
    public virtual double[,] CStamp(double[,] B, Pin[] pins)
    {
        return new double[B.GetLength(0), B.GetLength(1)];
    }
    public virtual double[] eStamp(double[] e)
    {
        return new double[e.Length];
    }
}