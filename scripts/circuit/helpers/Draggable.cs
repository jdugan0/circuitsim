using Godot;

public interface IGridSnappable
{
    Vector2I GetGridCoord();
    void SetGridCoord(Vector2I cell);
    void OnBeginDrag();
    void OnEndDrag(bool committed);
}
public partial class Draggable : Area2D, IGridSnappable
{
    public virtual Vector2I GetGridCoord() => GridHelper.GetGridCoord(GlobalPosition);
    public virtual void SetGridCoord(Vector2I cell) => GlobalPosition = GridHelper.GetWorldCoord(cell);
    public virtual void OnBeginDrag() { }
    public virtual void OnEndDrag(bool committed) { }
}