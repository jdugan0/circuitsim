using Godot;
using System;

public partial class Pin : Node2D
{
    public int netIndex;
    public double solvedVoltage;
    [Export] public bool isGround;
    bool bang = false;
    public Vector2I GetGridCoord()
    {
        return GridHelper.GetGridCoord(this);
    }

}
