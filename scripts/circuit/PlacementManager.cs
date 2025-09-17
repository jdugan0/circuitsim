using Godot;
using System;
using System.Linq;

public partial class PlacementManager : Node2D
{
    [Export] PackedScene wireScene;
    Wire wire = null;
    Wire toDrag = null;
    Pin closest;
    public override void _Process(double delta)
    {
        Vector2 mouse = GetGlobalMousePosition();

        Vector2I mouseCell = GridHelper.GetGridCoord(mouse);


        if (Input.IsActionJustPressed("PLACE"))
        {
            Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);

            if (toDrag == null)
            {
                foreach (Wire w in wires)
                {
                    if (w.IsMouseOver())
                    {
                        toDrag = w;
                        closest = toDrag.GetClosestPin();
                        break;
                    }
                }

            }
            if (toDrag == null)
            {
                wire = (Wire)wireScene.Instantiate();
                AddChild(wire);
                wire.startCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
                wire.endCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell += Vector2I.Up);
            }
        }

        if (Input.IsActionPressed("PLACE"))
        {
            if (wire != null)
            {
                if (mouseCell != wire.startCell.GetGridCoord())
                {
                    wire.endCell.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
                }

            }
            if (toDrag != null && closest != null)
            {
                closest.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
            }
        }
        if (Input.IsActionJustReleased("PLACE"))
        {
            wire = null;
            closest = null;
            toDrag = null;
        }
    }

}
