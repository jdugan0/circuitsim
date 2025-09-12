using Godot;
using Microsoft.Win32.SafeHandles;
using System;
[Tool]
public partial class GridHelper : Node2D
{
    [Export] public int cellSize = 100;
    [Export] public Vector2 zero = Vector2.Zero;
    static GridHelper instance;
    [Export] public Color lineColor = new Color(1, 1, 1, 0.25f);
    [Export] public float lineWidth = 1.0f;
    [Export] public bool drawInGame = false;
    [Export] public int cellAmount;
    [Export] bool enabled;

    public override void _Draw()
    {
        if (!Engine.IsEditorHint() && !drawInGame)
            return;
        if (!enabled)
            return;
        int s = Mathf.Max(1, cellSize);
        int n = Mathf.Max(1, cellAmount);
        for (int i = -n; i <= n; i++)
        {
            float x = zero.X + i * s;
            DrawLine(
                new Vector2(x, zero.Y - n * s),
                new Vector2(x, zero.Y + n * s),
                lineColor, lineWidth
            );
        }

        for (int j = -n; j <= n; j++)
        {
            float y = zero.Y + j * s;
            DrawLine(
                new Vector2(zero.X - n * s, y),
                new Vector2(zero.X + n * s, y),
                lineColor, lineWidth
            );
        }
        DrawLine(new Vector2(zero.X, zero.Y - n * s), new Vector2(zero.X, zero.Y + n * s),
                 lineColor, lineWidth + 0.5f);
        DrawLine(new Vector2(zero.X - n * s, zero.Y), new Vector2(zero.X + n * s, zero.Y),
                 lineColor, lineWidth + 0.5f);
    }
    public override void _Process(double delta)
    {
        // Redraw when values change/moving in editor
        if (Engine.IsEditorHint() || drawInGame)
            QueueRedraw();
    }
    public override void _Ready()
    {
        instance = this;
    }

    public static Vector2I GetGridCoord(Node2D node)
    {
        Vector2 pos = node.GlobalPosition - instance.zero;
        int gx = Mathf.RoundToInt(pos.X / instance.cellSize);
        int gy = Mathf.RoundToInt(pos.Y / instance.cellSize);
        return new Vector2I(gx, gy);
    }
}
