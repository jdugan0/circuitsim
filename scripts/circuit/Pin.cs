using Godot;
using System;

public partial class Pin : Node2D
{
    public int netIndex;
    public double solvedVoltage;
    [Export] public bool isGround;
    [Export] public bool disabled = false;
    [Export] public Draggable draggable;
    bool bang = false;
    public Vector2I GetGridCoord()
    {
        return GridHelper.GetGridCoord(this);
    }
    public override void _Process(double delta)
    {
        if (disabled)
        {
            Visible = false;
        }
        else
        {
            Visible = true;
        }
    }


}
