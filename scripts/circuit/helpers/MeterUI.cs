using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MeterUI : Label
{
    public override void _Process(double delta)
    {
        var components = Array.ConvertAll<Node, Component>(GetTree().GetNodesInGroup("component").ToArray<Node>(), (x) => (Component)x);
        string s = "";
        int v = 0;
        int i = 0;
        int o = 0;
        foreach (Component component in components)
        {
            if (component.componentProperty is VoltmeterProperty)
            {
                s += $"Voltage{v}: {component.solvedVoltage:N2}\n";
                v++;
            }
            if (component.componentProperty is AmmeterProperty)
            {
                s += $"Current{i}: {component.solvedCurrent:N2}\n";
                i++;
            }
            if (component.componentProperty is OhmmeterProperty)
            {
                s += $"Resistance{o}: {component.solvedVoltage:N2}\n";
                o++;
            }
        }
        Text = s;
    }

}
