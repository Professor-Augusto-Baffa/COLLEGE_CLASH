using Godot;

// TODO acertar modularização (?)
public partial class BoardSceneSwitcher : Node
{
	public AudioStreamPlayer music_player;
	
	public void SwitchScene(string scenePath)	
	{
		music_player = GetNode<AudioStreamPlayer>("Music");
		music_player.Stop();
		GetTree().ChangeSceneToFile(scenePath);
	}
}
