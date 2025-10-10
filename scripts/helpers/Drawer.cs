using Godot;
using System;

public partial class Drawer : Node
{
    [Export] PackedScene scene;
    public void Click()
    {
        PlacementManager.instance.AddDrag(scene);
    }
    public void Click(bool vPlus)
    {
        PlacementManager.instance.AddDrag(scene, vPlus);
    }
}
