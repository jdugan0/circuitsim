using Godot;
using System;

public partial class Component : Node
{
    [Export] public Pin[] pins { get; private set; }
    [Export] public ComponentProperty componentProperty;
}
