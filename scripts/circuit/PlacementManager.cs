using Godot;
using System;
using System.Linq;

public partial class PlacementManager : Node2D
{
    [Export] PackedScene wireScene;
    Wire wire = null;
    Wire wireToDrag = null;
    Pin closest;
    Draggable currentDrag;
    public int bounding = 0;
    [Signal] public delegate void OnGridChangeEventHandler();
    public static PlacementManager instance;
    public override void _Ready()
    {
        instance = this;
    }

    public void AddDrag(PackedScene scene)
    {
        Node n = scene.Instantiate();
        currentDrag = (Draggable)n.GetChild(1);
        currentDrag.BeginDrag();
        AddChild(n);
    }

    public override void _Process(double delta)
    {
        Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);
        Draggable[] draggables = Array.ConvertAll<Node, Draggable>(GetTree().GetNodesInGroup("draggable").ToArray<Node>(), (x) => (Draggable)x);
        Vector2 mouse = GetGlobalMousePosition();
        Vector2I mouseCell = GridHelper.GetGridCoord(mouse);


        if (Input.IsActionJustPressed("PLACE") && bounding == 0)
        {
            foreach (Draggable d in draggables)
            {
                if (d.IsMouseOver())
                {
                    currentDrag = d;
                    currentDrag.BeginDrag();
                    break;
                }
            }
            if (currentDrag == null)
            {
                foreach (Wire w in wires)
                {
                    if (w.IsMouseOver())
                    {
                        wireToDrag = w;
                        closest = wireToDrag.GetClosestPin();
                        break;
                    }
                }
            }
            if (wireToDrag == null && currentDrag == null)
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
            if (wireToDrag != null && closest != null)
            {
                closest.GlobalPosition = GridHelper.GetWorldCoord(mouseCell);
            }
            if (currentDrag != null)
            {
                currentDrag.SetGridCoord(mouse);
            }
        }
        if (Input.IsActionJustReleased("PLACE"))
        {
            if (currentDrag != null || wire != null || wireToDrag != null || closest != null)
            {
                EmitSignal(SignalName.OnGridChange);
            }
            wire = null;
            closest = null;
            wireToDrag = null;
            if (currentDrag != null) currentDrag.EndDrag();
            currentDrag = null;
        }
    }

    public void PowerBlow(Component component)
    {
        component.componentProperty.IsActive = false;
        EmitSignal(SignalName.OnGridChange);
    }

}
