using Godot;
using System;

public partial class Component : Node
{
    [Export] public Pin[] pins { get; private set; }
    [Export] public ComponentProperty componentProperty;
    public double solvedCurrent;
    public double solvedVoltage;
    public override void _Process(double delta)
    {
        if (componentProperty is ResistorProperty)
        {
            GD.Print("Voltage: " + solvedVoltage + " current: " + solvedCurrent);
        }
    }
    public override void _Ready()
    {
        if (componentProperty != null)
        {
            componentProperty = (ComponentProperty)componentProperty.Duplicate(true);
            componentProperty.ResourceLocalToScene = true;
        }
    }


}
