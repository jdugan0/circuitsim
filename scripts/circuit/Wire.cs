using Godot;
using System;

public partial class Wire : Node2D
{
    [Export] public Vector2I startCell { get; private set; }
    [Export] public Vector2I endCell { get; private set; }

    public bool GetConnectableAlongX()
    {
        return (startCell.X == endCell.X);
    }
    public bool GetConnectableAlongY()
    {
        return (startCell.Y == endCell.Y);
    }

    public bool GetConnected(Vector2I gridCoord)
    {
        if (gridCoord == startCell || gridCoord == endCell) return true;
        int bigX = Math.Max(startCell.X, endCell.X);
        int smallX = Math.Min(startCell.X, endCell.X);
        int bigY = Math.Max(startCell.Y, endCell.Y);
        int smallY = Math.Min(startCell.Y, endCell.Y);
        if (GetConnectableAlongX() && gridCoord.X == startCell.X && gridCoord.Y >= smallY && gridCoord.Y <= bigY) return true;
        if (GetConnectableAlongY() && gridCoord.Y == startCell.Y && gridCoord.X >= smallX && gridCoord.X <= bigX) return true;
        return false;
    }
}
