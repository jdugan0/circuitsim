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

    Component voltMeterCurrent = null;
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
    public void AddDrag(PackedScene scene, bool vPlus)
    {
        Node n = scene.Instantiate();
        if (n is Component c && c.componentProperty is VoltmeterProperty v)
        {
            if (voltMeterCurrent == null)
            {
                voltMeterCurrent = (Component)n;
                voltMeterCurrent.pins[vPlus ? 1 : 0].draggable.BeginDrag();
                voltMeterCurrent.pins[vPlus ? 1 : 0].disabled = false;
                currentDrag = voltMeterCurrent.pins[vPlus ? 1 : 0].draggable;
                AddChild(n);
            }
            else
            {
                if (voltMeterCurrent.pins[vPlus ? 1 : 0].disabled)
                {
                    voltMeterCurrent.pins[vPlus ? 1 : 0].disabled = false;
                    currentDrag = voltMeterCurrent.pins[vPlus ? 1 : 0].draggable;
                    voltMeterCurrent.pins[vPlus ? 1 : 0].draggable.BeginDrag();
                }
            }
            if (!voltMeterCurrent.pins[1].disabled && !voltMeterCurrent.pins[0].disabled)
            {
                voltMeterCurrent = null;
            }
        }
    }

    public override void _Process(double delta)
    {
        Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);
        Draggable[] draggables = Array.ConvertAll<Node, Draggable>(GetTree().GetNodesInGroup("draggable").ToArray<Node>(), (x) => (Draggable)x);
        Vector2 mouse = GetGlobalMousePosition();
        Vector2I mouseCell = GridHelper.GetGridCoord(mouse);

        if (currentDrag != null && Input.IsActionJustPressed("ROTATEL"))
        {
            ((Node2D)currentDrag.GetParent()).RotationDegrees += 90;
        }
        if (currentDrag != null && Input.IsActionJustPressed("ROTATER"))
        {
            ((Node2D)currentDrag.GetParent()).RotationDegrees -= 90;
        }

        if (Input.IsActionJustPressed("REMOVE") && currentDrag == null && wire == null && closest == null && wireToDrag == null)
        {
            foreach (Draggable d in draggables)
            {
                if (d.IsMouseOver())
                {
                    d.GetParent().QueueFree();
                    break;
                }
            }
            foreach (Wire w in wires)
            {
                if (w.IsMouseOver())
                {
                    w.QueueFree();
                    break;
                }
            }
        }
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
                if ((currentDrag != null && currentDrag.EndDrag()) || currentDrag == null)
                    EmitSignal(SignalName.OnGridChange);
            }
            wire = null;
            closest = null;
            wireToDrag = null;
            currentDrag = null;
        }

    }

    public void PowerBlow(Component component)
    {
        component.IsActive = false;
        EmitSignal(SignalName.OnGridChange);
    }

}
