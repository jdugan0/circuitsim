using Godot;
using System;
using System.Collections.Generic;

public partial class SelectionLayer : TileMapLayer
{

	public Vector2I hoveredCoords = Vector2I.Zero;
	public Vector2I selectionAtlasCoords = new Vector2I(0, 0);
	[Export] public int source_id = 2;

	//Vector2I cannot be exported, cast later
	[Export] public Vector2[] atlasCoords;

	public int current_atlas_index = 0;

	public override void _Ready()
	{
		SetCell(hoveredCoords, source_id, selectionAtlasCoords, 0);
	}

	//Replaces hovered tile as mouse moves over grid
	public override void _Process(double _delta)
	{
		if (hoveredCoords != LocalToMap(GetGlobalMousePosition()))
		{
			ReplaceCell();
		}
	}

	// Press Space or Enter to switch to different component size
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			if (current_atlas_index < atlasCoords.Length - 1)
			{
				current_atlas_index += 1;
			}
			else
			{
				current_atlas_index = 0;
			}
			selectionAtlasCoords = (Vector2I)atlasCoords[current_atlas_index];
			ReplaceCell();
		}
	}

	public void ReplaceCell()
	{
		EraseCell(hoveredCoords);
		hoveredCoords = LocalToMap(GetGlobalMousePosition());
		SetCell(hoveredCoords, source_id, selectionAtlasCoords, 0);
	}
}
