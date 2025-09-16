using Godot;
using System;

public partial class PlacementManager : Node2D
{
    [Export] PackedScene wireScene;
    Wire wire;
    public override void _Process(double delta)
    {
        Vector2 mouse = GetGlobalMousePosition();

        Vector2I mouseCell = GridHelper.GetGridCoord(mouse);

        if (Input.IsActionJustPressed("PLACE"))
        {
            GD.Print("hi");
            wire = (Wire)wireScene.Instantiate();
            AddChild(wire);
            wire.startCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
            wire.endCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell += Vector2I.Up);
        }

        if (Input.IsActionPressed("PLACE"))
        {
            if (mouseCell != wire.startCell.GetGridCoord())
            {
                wire.endCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
            }
        }
        if (Input.IsActionJustReleased("PLACE"))
        {
            wire = null;
        }
    }

}
