using Godot;
using System;

public partial class GridHelper
{
    public static int cellSize = 100;
    public static Vector2 zero;
    public static Vector2 GetGridCoord(Node2D node)
    {
        Vector2 pos = node.GlobalPosition - zero;
        int gx = Mathf.RoundToInt(pos.X / cellSize);
        int gy = Mathf.RoundToInt(pos.Y / cellSize);
        return new Vector2(gx, gy);
    }
}
