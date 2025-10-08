using Godot;
using System;

public partial class UI : CanvasLayer
{
    public void ClearGrid()
    {
        for (int i = PlacementManager.instance.GetChildCount(); i > 0; i--)
        {
            Node n = PlacementManager.instance.GetChild(0);
            PlacementManager.instance.RemoveChild(n);
            n.QueueFree();
        }
        PlacementManager.instance.EmitSignal(PlacementManager.SignalName.OnGridChange);
    }
}
