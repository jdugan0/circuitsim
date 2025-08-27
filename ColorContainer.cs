using Godot;
using System;

public partial class ColorContainer : MarginContainer
{
	public override void _Ready()
	{
		TextureButton redButton = GetNode<TextureButton>("HBoxContainer/RedButton");
		TextureButton greenButton = GetNode<TextureButton>("HBoxContainer/GreenButton");
		TextureButton blueButton = GetNode<TextureButton>("HBoxContainer/BlueButton");
		TextureButton yellowButton = GetNode<TextureButton>("HBoxContainer/YellowButton");
		TextureButton purpleButton = GetNode<TextureButton>("HBoxContainer/PurpleButton");
		TextureButton blackButton = GetNode<TextureButton>("HBoxContainer/BlackButton");
		TextureButton whiteButton = GetNode<TextureButton>("HBoxContainer/WhiteButton");

		// I am very happy I learned how to do this this is so cool
		redButton.Pressed += () => HandleButtonPress(0);
		greenButton.Pressed += () => HandleButtonPress(1);
		blueButton.Pressed += () => HandleButtonPress(2);
		yellowButton.Pressed += () => HandleButtonPress(3);
		purpleButton.Pressed += () => HandleButtonPress(4);
		blackButton.Pressed += () => HandleButtonPress(5);
		whiteButton.Pressed += () => HandleButtonPress(6);
	}

	private void HandleButtonPress(int color)
	{
		if(color == 0)
		{
			GD.Print("red");
		}
		else if(color == 1)
		{
			GD.Print("green");
		}
		else if(color == 2)
		{
			GD.Print("blue");
		}
		else if(color == 3)
		{
			GD.Print("yellow");
		}
		else if(color == 4)
		{
			GD.Print("purple");
		}
		else if(color == 5)
		{
			GD.Print("black");
		}
		else if(color == 6)
		{
			GD.Print("white");
		}
	}
}
