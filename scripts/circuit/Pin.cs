using Godot;
using System;

public partial class Pin : Node2D
{
    public int netIndex;
    [Export] public bool isGround;
    public Vector2I GetGridCoord()
    {
        return GridHelper.GetGridCoord(this);
    }
}
