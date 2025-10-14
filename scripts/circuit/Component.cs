using Godot;
using System;

public partial class Component : Node2D
{
    [Export] public Pin[] pins { get; private set; }
    [Export] public ComponentProperty componentProperty;
    double energy;
    public double solvedCurrent;
    public double solvedVoltage;

    public bool IsActive = true;
    public bool drag;
    public float dragTimer;
    [Export] public float dragTime = 0.1f;
    [Signal] public delegate void DragChangedEventHandler();
    public override void _PhysicsProcess(double delta)
    {
        if (!(componentProperty is OhmmeterProperty || componentProperty is VoltmeterProperty || componentProperty is AmmeterProperty))
        {
            energy = Math.Max(0, energy + (Math.Abs(solvedCurrent * solvedVoltage) - componentProperty.wattage) * delta);
        }
        if (energy >= componentProperty.maxEnergy)
        {
            PlacementManager.instance.PowerBlow(this);
        }
        if (!IsActive)
        {
            solvedCurrent = 0;
            solvedVoltage = 0;
            energy = 0;
        }
        if (drag)
        {
            dragTimer += (float)delta;
        }

        if (componentProperty is VoltmeterProperty)
        {
            if (!CircuitComputer.instance.PinsInSameGroup(pins[0], pins[1]))
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }
            var net0 = CircuitComputer.instance.GetPinNet(pins[0]);
            var net1 = CircuitComputer.instance.GetPinNet(pins[1]);

            if (net0 == net1)
                IsActive = false;
            else
                IsActive = true;
        }
    }
    public override void _Ready()
    {
        if (componentProperty != null)
        {
            componentProperty = (ComponentProperty)componentProperty.Duplicate(true);
            componentProperty.ResourceLocalToScene = true;
        }
        DragChanged += dragChanged;
    }

    public void dragChanged()
    {
        if (!drag)
        {
            if (dragTimer <= dragTime && componentProperty is SwitchProperty)
            {
                IsActive = !IsActive;
                PlacementManager.instance.EmitSignal(PlacementManager.SignalName.OnGridChange);
            }
            dragTimer = 0f;
        }
    }

}
