using Godot;
using System;
using System.Collections.Generic;

public partial class Wire : Node2D
{
    [Export] public Pin startCell { get; private set; }
    [Export] public Pin endCell { get; private set; }

    public bool GetConnectableAlongY()
    {
        return (startCell.GetGridCoord().X == endCell.GetGridCoord().X);
    }
    public bool GetConnectableAlongX()
    {
        return (startCell.GetGridCoord().Y == endCell.GetGridCoord().Y);
    }

    public override void _Process(double delta)
    {
        // GD.Print(string.Join(",", CoveredCells()));
    }

    public bool GetConnected(Vector2I gridCoord)
    {
        if (gridCoord == startCell.GetGridCoord() || gridCoord == endCell.GetGridCoord()) return true;
        int bigX = Math.Max(startCell.GetGridCoord().X, endCell.GetGridCoord().X);
        int smallX = Math.Min(startCell.GetGridCoord().X, endCell.GetGridCoord().X);
        int bigY = Math.Max(startCell.GetGridCoord().Y, endCell.GetGridCoord().Y);
        int smallY = Math.Min(startCell.GetGridCoord().Y, endCell.GetGridCoord().Y);
        if (GetConnectableAlongY() && gridCoord.X == startCell.GetGridCoord().X && gridCoord.Y >= smallY && gridCoord.Y <= bigY) return true;
        if (GetConnectableAlongX() && gridCoord.Y == startCell.GetGridCoord().Y && gridCoord.X >= smallX && gridCoord.X <= bigX) return true;
        return false;
    }
    public IEnumerable<Vector2I> CoveredCells()
    {
        if (startCell == endCell)
        {
            yield return startCell.GetGridCoord();
            yield break;
        }

        if (startCell.GetGridCoord().X == endCell.GetGridCoord().X)
        {
            int x = startCell.GetGridCoord().X;
            int y0 = Math.Min(startCell.GetGridCoord().Y, endCell.GetGridCoord().Y);
            int y1 = Math.Max(startCell.GetGridCoord().Y, endCell.GetGridCoord().Y);
            for (int y = y0; y <= y1; y++)
                yield return new Vector2I(x, y);
            yield break;
        }
        if (startCell.GetGridCoord().Y == endCell.GetGridCoord().Y)
        {
            int y = startCell.GetGridCoord().Y;
            int x0 = Math.Min(startCell.GetGridCoord().X, endCell.GetGridCoord().X);
            int x1 = Math.Max(startCell.GetGridCoord().X, endCell.GetGridCoord().X);
            for (int x = x0; x <= x1; x++)
                yield return new Vector2I(x, y);
            yield break;
        }

    }
}
