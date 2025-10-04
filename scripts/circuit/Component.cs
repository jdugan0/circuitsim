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
            public override void _PhysicsProcess(double delta)
            {
                energy = Math.Max(0, energy + (((solvedCurrent * solvedVoltage - componentProperty.wattage))) * delta);
                if (energy >= componentProperty.maxEnergy)
                {
                    PlacementManager.instance.PowerBlow(this);
                }
                if (componentProperty is CapacitorProperty)
                {
                    if (!IsActive)
                    {
                        GD.Print("boom");
                    }
                    else
                    {
                        GD.Print("voltage: " + solvedVoltage + " current: " + solvedCurrent + " energy: " + energy);
                    }
                }
                if (!IsActive)
                {
                    solvedCurrent = 0;
                    solvedVoltage = 0;
                    energy = 0;
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
