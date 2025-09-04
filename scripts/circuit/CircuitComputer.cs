using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CircuitComputer : Node
{
    public override void _Process(double delta)
    {
        Component[] components = Array.ConvertAll<Node, Component>(GetTree().GetNodesInGroup("component").ToArray<Node>(), (x) => (Component)x);
        Wire[] wires = Array.ConvertAll<Node, Wire>(GetTree().GetNodesInGroup("wire").ToArray<Node>(), (x) => (Wire)x);

        //two components are connected if their pins are touching or if their pins are touching a wire that connects them.
    }

}
