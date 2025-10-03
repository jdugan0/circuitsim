using Godot;
public partial class Draggable : Area2D
{
    bool hovered;
    Vector2? inital = null;
    Area2D inArea = null;
    [Export] Vector2 offset = new Vector2(15, 0);
    [Export] public Node2D moveTarget;
    public void EnterArea(Area2D area)
    {
        if (area.IsInGroup("draggable") || area.IsInGroup("blocking")) inArea = area;
    }
    public void ExitArea(Area2D area)
    {
        if (area == inArea) inArea = null;
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

    }
    public bool EndDrag()
    {
        if (inArea == null)
        {
            inital = GlobalPosition;
            return true;
        }

        if (inital == null || inArea.IsInGroup("blocking"))
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