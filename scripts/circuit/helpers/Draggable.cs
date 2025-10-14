using Godot;
public partial class Draggable : Area2D
{
    bool hovered;
    Vector2? inital = null;
    Area2D inArea = null;
    [Export] Vector2 offset = new Vector2(15, 0);
    [Export] Vector2 realOffset;
    [Export] public Node2D moveTarget;
    public bool valid;
    public override void _Ready()
    {
        realOffset = offset;
    }
    public override void _Process(double delta)
    {
        offset = realOffset.Rotated(((Node2D)GetParent()).Rotation);
    }
    public void EnterArea(Area2D area)
    {
        if (area.IsInGroup("draggable")) inArea = area;
        if (area.IsInGroup("valid")) valid = true;
    }
    public void ExitArea(Area2D area)
    {
        if (area == inArea) inArea = null;
        if (area.IsInGroup("valid")) valid = false;
    }
    public void OnHover()
    {
        hovered = true;
    }
    public void OnHoverRemove()
    {
        hovered = false;
    }
    public void BeginDrag()
    {
        if (moveTarget is Component c)
        {
            c.drag = true;
            c.EmitSignal("DragChanged");
        }
    }
    public bool EndDrag()
    {
        if (moveTarget is Component c)
        {
            c.drag = false;
            c.EmitSignal("DragChanged");
        }
        if (inArea == null && valid)
        {
            inital = GlobalPosition;
            return true;
        }

        if (inital == null || !valid)
        {
            GetParent().QueueFree();
            moveTarget.QueueFree();
            QueueFree();
            return false;
        }

        GlobalPosition = inital.Value;
        moveTarget.GlobalPosition = inital.Value;
        return false;
    }
    public void SetGridCoord(Vector2 coord)
    {
        GlobalPosition = GridHelper.GetWorldCoord(GridHelper.GetGridCoord(coord)) + offset;
        moveTarget.GlobalPosition = GlobalPosition;
    }

    public bool IsMouseOver()
    {
        return hovered;
    }
}