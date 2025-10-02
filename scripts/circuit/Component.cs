using Godot;
using System;

public partial class Component : Node
{
    [Export] public Pin[] pins { get; private set; }
    [Export] public ComponentProperty componentProperty;
    [Export] public double maxEnergy = 10;
    [Export] public double wattage = 5;
    double energy;
    public double solvedCurrent;
    public double solvedVoltage;
    public override void _Process(double delta)
    {
        energy += (solvedCurrent * solvedCurrent * solvedVoltage - wattage) * delta;
        if (energy >= maxEnergy)
        {
            PlacementManager.instance.PowerBlow(this);
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
