using Godot;
using System;

public partial class Bounding : Area2D
{
    public void MouseEnter()
    {
        PlacementManager.instance.bounding++;
    }
    public void MouseExit()
    {
        PlacementManager.instance.bounding--;
    }

}
