using Godot;
using System;

public partial class Component : Node
{
    [Export] public Pin[] pins { get; private set; }
    [Export] public ComponentProperty componentProperty;

    public override void _Process(double delta)
    {
        if (componentProperty is ResistorProperty r)
        {
            GD.Print("Voltage: " + r.solvedVoltage + " current: " + r.solvedCurrent);
        }
    }

}
