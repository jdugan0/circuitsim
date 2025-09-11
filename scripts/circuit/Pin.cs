using Godot;
using System;

public partial class Pin : Node2D
{
    public int netIndex;
    public Vector2I GetGridCoord()
    {
        return GridHelper.GetGridCoord(this);
    }
}
