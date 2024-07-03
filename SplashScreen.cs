using Godot;
using System;
public partial class splash_screen : Control
{
	[Export]
	public AnimationPlayer Animation;
	private void OnAnimationAnimationFinished(StringName anim_name)
	{
		if (anim_name == "fade_in")
			Animation.Play("fade_out");
		else if (anim_name == "fade_out")
			GetTree().ChangeSceneToFile("res://scene/scene.tscn");
	}
}
