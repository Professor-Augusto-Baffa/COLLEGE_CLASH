using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

public partial class Hideo : CharacterBody2D
{

	[Export]
	// How fast the player will move (pixels/sec).
	public int Speed { get; set; } = 400;
	//public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// Constants
	//public Vector2 velocity = new Vector2(0, 0);

	/*
	public int dash_duration = 10;
	public const int RUNSPEED = 340;
	public const int DASHSPEED = 390;
	public const int WALKSPEED = 200;
	public const int GRAVITY = 1800;
	public const int JUMPFORCE = 500;
	public const int MAX_JUMPFORCE = 800;
	public const int DOUBLEJUMPFORCE = 1000;
	public const int MAXAIRSPEED = 300;
	public const int AIR_ACCEL = 25;
	public const int FALLSPEED = 60;
	public const int FALLINGSPEED = 900;
	public const int MAXFALLSPEED = 900;
	public const int TRACTION = 40;
	public const int ROLL_DISTANCE = 350;
	public int air_dodge_speed = 500;
	public const int UP_B_LAUNCHSPEED = 700;
	*/

	public object States;

	public Label framesLabel;
	public int frame = 0;

	public Vector2 ScreenSize;

	public FightSceneSwitcher sceneSwitcher;

	public Dictionary player_data = new Dictionary();

	public string save_path = "res://HideoStats.sav";

	public bool isFlippedH;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Globals Variables
	//public int frame = 0;
	[Export]
	public int id;

	// Attributes
	[Export]
	public int percentage = 0;
	[Export]
	public int stocks = 3;
	[Export]
	public int weight = 77;//100;
	public int freezeframes = 0;

	//Buffers;
	public int l_cancel = 0;
	public int cooldown = 0;
	public int shield_buffer = 0;

	//Knockback
	public int hdecay;
	public int vdecay;
	public int knockback;
	public int hitstun;
	public bool connected;

	// Ground Variables
	//public int dash_duration = 10;

	// Landing variables
	int landing_frames = 0;
	int lag_frames = 0;

	// Air variables
	int jump_squat = 3;
	bool fastfall = false;
	int airJump = 0;

	[Export]
	int airJumpMax = 1;

	//Ledges 
	public bool last_ledge = false;
	public int regrab = 30;
	public bool catche = false;

	//Hitboxes
	[Export]
	public PackedScene Hitbox { get; set; }

	[Export]
	public PackedScene Projectile { get; set; }

	[Export]
	public PackedScene Grabbox { get; set; }

	public object selfState;

	//Temporary Variables
	public int hit_pause = 0;
	public int hit_pause_dur = 0;
	public Vector2 temp_pos = new Vector2(0, 0);
	public Vector2 temp_vel = new Vector2(0, 0);

	//Attacks
	public int projectile_cooldown = 0;
	public bool grabbing = false;

	//Onready Variables
	public RayCast2D GroundL;
	public RayCast2D GroundR;
	public RayCast2D Ledge_Grab_F;
	public RayCast2D Ledge_Grab_B;
	public Node2D gun_pos;
	public Label states;
	public AnimationPlayer anim;
	public Area2D hurtbox;
	public Area2D parrybox;
	public Sprite2D sprite;

	//main attributes
	public int RUNSPEED = 340 * 2;
	public int DASHSPEED = 390 * 2;
	public int WALKSPEED = 200 * 2;
	public int GRAVITY = 1800 * 2;
	public int JUMPFORCE = 500 * 2;
	public int MAX_JUMPFORCE = 800 * 2;
	public int DOUBLEJUMPFORCE = 1000 * 2;
	public int MAXAIRSPEED = 300 * 2;
	public int AIR_ACCEL = 25 * 2;
	public int FALLSPEED = 60 * 2;
	public int FALLINGSPEED = 900 * 2;
	public int MAXFALLSPEED = 900 * 2;
	public int TRACTION = 40 * 2;
	public int ROLL_DISTANCE = 350 * 2;
	public int air_dodge_speed = 500 * 2;
	public int UP_B_LAUNCHSPEED = 700 * 2;

	[Export] public PackedScene hitbox;
	[Export] public PackedScene grabbox;
	[Export] public PackedScene projectile;


	public override void _Ready()
	{
		States = GetNode<Node>("State");
		ScreenSize = GetViewportRect().Size;
		GetNode<AnimatedSprite2D>("Sprite").Play("idle");
		//sceneSwitcher = GetNode<FightSceneSwitcher>("FightSceneSwitcher");
		//GetNode<AnimatedSprite2D>("Sprite").Connect("animation_finished", OnAnimationFinished());

		//Global GlobalScene = GetTree().Root.GetChild(0);
		//GD.Print("Nome da cena: " + GlobalScene.Name);
		//var targetNode = testStageScene.GetNode<Node2D>("MEL");	
		Global global = (Global)GetNode("/root/Global");

		//isInitialized = GlobalScene.isInitialized;

		//GD.Print("global.isInitializedHideo: ", global.isInitializedHideo);
		if (global.isInitializedHideo)
		{
			Load();
		}
		else
		{
			global.ExecuteOnceHideo();
		}
		//isBoot = false;
		///---

		GroundL = GetNode<RayCast2D>("Raycasts/GroundL");
		GroundR = GetNode<RayCast2D>("Raycasts/GroundR");
		Ledge_Grab_F = GetNode<RayCast2D>("Raycasts/Ledge_Grab_F");
		Ledge_Grab_B = GetNode<RayCast2D>("Raycasts/Ledge_Grab_B");
		gun_pos = GetNode<Node2D>("gun_pos");
		states = GetNode<Label>("State");
		anim = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		hurtbox = GetNode<Area2D>("Hurtbox");
		parrybox = GetNode<Area2D>("Parrybox");
		sprite = GetNode<Sprite2D>("Sprite2D");

		// Define a posição global de gun_pos
		gun_pos.GlobalPosition = new Vector2(100, 200);
	}

	public void Save()
	{
		//GD.Print("global_position: ", GlobalPosition);
		player_data["global_position"] = GlobalPosition;

		// Acessar o AnimatedSprite2D para obter a direção
		AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("Sprite");
		bool isFlippedH = sprite.FlipH;

		// Armazenar a direção no dicionário de dados do jogador
		player_data["direction"] = isFlippedH ? "left" : "right";


		FileAccess file = FileAccess.Open(save_path, FileAccess.ModeFlags.Write);
		file.StoreVar(player_data);
		//GD.Print("global_position salva!");

		file.Close();
	}

	public void Load()
	{
		if (FileAccess.FileExists(save_path))
		{
			using var file = FileAccess.Open(save_path, FileAccess.ModeFlags.Read);
			player_data = (Dictionary)file.GetVar();
			file.Close();

			if (player_data.ContainsKey("global_position"))
			{
				//GD.Print("global_position: ", GlobalPosition);
				GlobalPosition = (Vector2)player_data["global_position"];
				//GD.Print("global_position loaded!");
			}

			if (player_data.ContainsKey("direction"))
			{
				string direction = (string)player_data["direction"];
				AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("Sprite");

				// Aplicar a direção ao AnimatedSprite2D
				sprite.FlipH = direction == "left";
			}

		}
		else
		{
			player_data = new Dictionary();
		}
		//GD.Print("Hideo:", player_data);
	}

	public void OnAnimationFinished()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");
		string anim_name = animatedSprite2D.Animation;

		if (anim_name == "kick" || anim_name == "uppercut")
		{
			if (!animatedSprite2D.Animation.Equals("walk"))
			{
				animatedSprite2D.Play("idle");
			}
		}
	}

	public void SwitchToNextScene()
	{
		Save();

		if (GetParent() is TestStage parent)
		{

			//GD.Print("HIDEO: Parent node: ", parent.Name);

			sceneSwitcher = parent.sceneSwitcher;
			//GD.Print("HIDEO: parent.sceneSwitcher: ", parent.sceneSwitcher);
			if (sceneSwitcher == null)
			{
				//GD.Print("HIDEO: sceneSwitcher is null. Please check if it is properly initialized in the parent node.");
			}
			else
			{
				//GD.Print("HIDEO: sceneSwitcher accessed successfully");
			}
		}
		else
		{
			//GD.Print("HIDEO: Parent node is not of type TestStage or parent node not found.");

			//GD.Print("HIDEO: Parent node not found");
		}

		if (sceneSwitcher != null)
		{
			sceneSwitcher.SwitchScene("res://Chess/Board.tscn");
		}
		else
		{
			//GD.Print("HIDEO: sceneSwitcher is null, cannot switch scene");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Speed * delta;
		// The player's movement vector.
		/*
		var velocity = Vector2.Zero; // OLD Vector2 velocity = Velocity;
		var animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += 0 * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			//animatedSprite2D.Play();
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			//animatedSprite2D.Stop();
		}
		

		Velocity = velocity;
		*/

		//MoveAndSlide();
		/*

		framesLabel = GetNode<Label>("Frames");
		if (framesLabel != null)
		{
			framesLabel.Text = frame.ToString();
		}

		//EndOfBow = GetNode<Node2D>("EndOfBow");
		//creenSize = GetViewportRect().Size;

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			//animatedSprite2D.Play();
		}

		else
		{
			//animatedSprite2D.Stop();
		}
		
		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;

			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		*/

		// Speed * delta;
		// The player's movement vector.
		var velocity = Vector2.Zero;

		var animatedSprite2D = GetNode<AnimatedSprite2D>("Sprite");

		if (Input.IsActionPressed("right_2"))
			velocity.X += 1;

		if (Input.IsActionPressed("left_2"))
			velocity.X -= 1;

		//if (Input.IsActionPressed("move_down"))
		//	velocity.Y += 1;

		//if (Input.IsActionPressed("move_up"))
		//	velocity.Y -= 1;


		if (Input.IsActionPressed("up_2") && !animatedSprite2D.Animation.Equals("uppercut"))
			animatedSprite2D.Play("uppercut");

		if (Input.IsActionPressed("down_2") && !animatedSprite2D.Animation.Equals("kick"))
			animatedSprite2D.Play("kick");

		//if (Input.IsActionPressed("change_scene"))
		//SwitchToNextScene();

		/*	var sprite_frames = $AnimatedSprite2D.sprite_frames
				Get the first texture of the wanted animation (in this case, walk, you can also get the size
				in differents cases)
				If your animation frames has different sizes, use $AnimatedSprite2D.frame instead of 0
			var texture       = sprite_frames.get_frame_texture("walk", 0)
				Get frame size:
			var texture_size  = texture.get_size()
				This is not the end, you will get the texture size, not the node real size, then you need to
				multiply the texture size with the node scale
			var as2d_size     = texture_size * $AnimatedSprite2D.get_scale()
		*/

		/*
			if (velocity.Length() > 0)
			{
				velocity = velocity.Normalized() * Speed;
				animatedSprite2D.Play();
			}

			else
			{
				animatedSprite2D.Stop();
			}
			/*


			//Position += velocity * (float)delta;

			/*
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 27, ScreenSize.Y-27),
				y: Mathf.Clamp(Position.Y, 33.75f, ScreenSize.Y-33.75f) 
			);
			*/

		/*
		if (velocity.X != 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;

			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (animatedSprite2D.Animation != "uppercut" && animatedSprite2D.Animation != "kick")
		{
			animatedSprite2D.Animation = "idle";
		}
		/*

		/*
		if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
		*/


		if (Mathf.Abs(velocity.X) > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play("walk");
			animatedSprite2D.FlipH = velocity.X > 0;
		}
		else if (!animatedSprite2D.Animation.Equals("uppercut") && !animatedSprite2D.Animation.Equals("kick"))
		{
			animatedSprite2D.Play("idle");
		}

		Position += velocity * (float)delta;

		GetNode<Label>("Frames").Text = frame.ToString();
		GetNode<Label>("Health").Text = percentage.ToString();
		selfState = states.Text;

	}


	/*
	public override void _Process(float delta)
	{
		// Implement _Process logic here
	}
	*/

	/*	ponte velha
	public void UpdateFrames(float delta)
		{
			frame += 1;
		}
	*/

	/* ponte velha
	public void Turn(bool direction)
		{
			int dir = direction ? -1 : 1;
			GetNode<AnimatedSprite2D>("Sprite").FlipH = direction;
		}
	*/

	public void ResetFrame()
	{
		frame = 0;
	}

	public void ResetJumps()
	{
		airJump = airJumpMax;
	}

	public Node2D CreateHitbox(float width, float height, float damage, float angle, float baseKb, float kbScaling, float duration, string type, Vector2 points, float angleFlipper, float hitlag = 1)
	{
		Node2D hitboxInstance = (Node2D)hitbox.Instantiate();
		AddChild(hitboxInstance);

		// Rotates the points
		if (Direction() == 1)
		{
			hitboxInstance.Call("set_parameters", width, height, damage, angle, baseKb, kbScaling, duration, type, points, angleFlipper, hitlag);
		}
		else
		{
			Vector2 flipXPoints = new Vector2(-points.X, points.Y);
			hitboxInstance.Call("set_parameters", width, height, damage, -angle + 180, baseKb, kbScaling, duration, type, flipXPoints, angleFlipper, hitlag);
		}

		return hitboxInstance;
	}

	public int Direction()
	{
		// Your implementation of the Direction method
		return 1; // Placeholder
	}

	public void CreateGrabbox(float width, float height, float damage, float duration, Vector2 points)
	{
		Node2D grabboxInstance = (Node2D)grabbox.Instantiate();
		AddChild(grabboxInstance);

		// Rotates the points
		if (Direction() == 1)
		{
			grabboxInstance.Call("set_parameters", width, height, damage, duration, points);
		}
		else
		{
			Vector2 flipXPoints = new Vector2(-points.X, points.Y);
			grabboxInstance.Call("set_parameters", width, height, damage, duration, flipXPoints);
		}
	}

	public void CreateProjectile(float dirX, float dirY, Vector2 point)
	{
		Node2D projectileInstance = (Node2D)projectile.Instantiate();
		projectileInstance.Call("append_to_player_list", this);
		GetParent().AddChild(projectileInstance);

		// Sets position
		//gun_pos.SetPosition(point);
		// Ou se desejar definir a posição localmente
		gun_pos.Position = point;

		// Flips the direction
		if (Direction() == 1)
		{
			projectileInstance.Call("dir", dirX, dirY);
			projectileInstance.GlobalPosition = gun_pos.GlobalPosition;
		}
		else
		{
			gun_pos.Position = new Vector2(-gun_pos.Position.X, gun_pos.Position.Y);
			projectileInstance.Call("dir", -dirX, dirY);
			projectileInstance.GlobalPosition = gun_pos.GlobalPosition;
		}
	}

	public void UpdateFrames(float delta)
	{
		frame += Mathf.FloorToInt(delta * 60);
		l_cancel -= Mathf.FloorToInt(delta * 60);
		l_cancel = Mathf.Clamp(l_cancel, 0, l_cancel);
		cooldown -= Mathf.FloorToInt(delta * 60);
		cooldown = Mathf.Clamp(cooldown, 0, cooldown);

		if (!Input.IsActionPressed($"shield_{id}"))
		{
			shield_buffer = 0;
		}
		else
		{
			shield_buffer += Mathf.FloorToInt(delta * 60);
		}

		if (freezeframes > 0)
		{
			freezeframes -= Mathf.FloorToInt(delta * 60);
		}
		freezeframes = Mathf.Clamp(freezeframes, 0, freezeframes);
	}

	public void Turn(bool direction)
	{
		int dir = 0;
		if (direction)
		{
			dir = -1;
		}
		else
		{
			dir = 1;
		}
		GetNode<AnimatedSprite2D>("Sprite").FlipH = direction;

		Ledge_Grab_F.TargetPosition = new Vector2(dir * Mathf.Abs(Ledge_Grab_F.TargetPosition.X), Ledge_Grab_F.TargetPosition.Y);
		Ledge_Grab_F.Position = new Vector2(dir * Mathf.Abs(Ledge_Grab_F.Position.X), Ledge_Grab_F.Position.Y);
		Ledge_Grab_B.Position = new Vector2(dir * Mathf.Abs(Ledge_Grab_B.Position.X), Ledge_Grab_B.Position.Y);
		Ledge_Grab_B.TargetPosition = new Vector2(-dir * Mathf.Abs(Ledge_Grab_F.TargetPosition.X), Ledge_Grab_F.TargetPosition.Y);
	}

	void HitPause(float delta)
	{
		if (hit_pause < hit_pause_dur)
		{
			GlobalPosition = temp_pos;
			hit_pause += Mathf.FloorToInt((1 * delta) * 60);
		}
		else
		{
			if (temp_vel != new Vector2(0, 0))
			{
				Velocity = temp_vel;
				//velocity.x = temp_vel.x;
				//velocity.y = temp_vel.y;
				temp_vel = new Vector2(0, 0);
			}
			hit_pause_dur = 0;
			hit_pause = 0;
		}
	}

	// Special Attacks
	bool NeutralSpecial()
	{
		if (frame == 4)
		{
			CreateProjectile(1, 0, new Vector2(50, 0));
		}
		if (frame >= 14)
		{
			return true;
		}
		return false;
	}

	// Tilt Attacks
	bool Jab()
	{
		if (frame == 2)
		{
			CreateGrabbox(30, 40, 0, 3, new Vector2(64, 0));
		}
		if (frame == 6)
		{
			if (grabbing)
			{
				return false;
			}
			// Uncomment to create grabbox if needed:
			// CreateGrabbox(40, 50, 0, 13, new Vector2(64, 0));
		}
		if (frame >= 20)
		{
			return true;
		}
		return false;
	}

	bool Jab_1()
	{
		if (frame == 1)
		{
			grabbing = false;
			CreateGrabbox(30, 40, 0, 13, new Vector2(64, 0));
		}
		if (frame == 14)
		{
			CreateHitbox(40, 20, 8, 90, 8000, 1, 5, "normal", new Vector2(48, 8), 0, 0);
		}
		if (frame == 26 || frame == 32 || frame == 39)
		{
			CreateProjectile(0, -1, new Vector2(34.089f, -70.645f));
		}
		if (frame == 43)
		{
			return true;
		}
		return false;
	}

	bool DownTilt()
	{
		if (frame == 5)
		{
			CreateHitbox(40, 20, 8, 90, 70, 50, 3, "normal", new Vector2(64, 32), 0, 1);
			// My version: CreateHitbox(40, 20, 8, 90, 3, 120, 3, "normal", new Vector2(64, 32), 0, 1);
		}
		if (frame >= 10)
		{
			return true;
		}
		return false;
	}

	bool UpTilt()
	{
		if (frame == 5)
		{
			CreateHitbox(48, 68, 8, 76, 20, 110, 3, "normal", new Vector2(-22, -15), 0, 1);
			// My version: CreateHitbox(48, 68, 6, 76, 8, 140, 4, "normal", new Vector2(-22, -15), 0, 1);
		}
		if (frame >= 12)
		{
			return true;
		}
		return false;
	}

	bool ForwardTilt()
	{
		if (frame == 3)
		{
			CreateHitbox(52, 20, 6, 120, 40, 80, 3, "normal", new Vector2(22, 8), 0, 1);
			// My version: CreateHitbox(52, 20, 7, 120, 13, 100, 3, "normal", new Vector2(22, 8), 0, 0.5f);
		}
		if (frame >= 8)
		{
			return true;
		}
		return false;
	}

	// Air attacks
	bool Nair()
	{
		if (frame == 1)
		{
			CreateHitbox(56, 56, 12, 361, 0, 100, 3, "normal", new Vector2(0, 0), 0, 0.4f);
		}
		if (frame > 1)
		{
			if (connected)
			{
				if (frame == 36)
				{
					connected = false;
					return true;
				}
			}
			else
			{
				if (frame == 5)
				{
					CreateHitbox(46, 56, 9, 361, 0, 100, 10, "normal", new Vector2(0, 0), 0, 0.1f);
				}
				if (frame == 36)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool Uair()
	{
		if (frame == 2)
		{
			CreateHitbox(32, 36, 5, 90, 130, 0, 2, "normal", new Vector2(0, -45), 0, 1);
		}
		if (frame == 6)
		{
			CreateHitbox(56, 46, 10, 90, 20, 108, 3, "normal", new Vector2(0, -48), 0, 2);
		}
		if (frame == 15)
		{
			return true;
		}
		return false;
	}

	bool Bair()
	{
		if (frame == 2)
		{
			CreateHitbox(52, 55, 15, 45, 1, 100, 5, "normal", new Vector2(-47, 7), 6, 1);
		}
		if (frame > 1)
		{
			if (connected)
			{
				if (frame == 18)
				{
					connected = false;
					return true;
				}
			}
			else
			{
				if (frame == 7)
				{
					CreateHitbox(52, 55, 5, 45, 3, 140, 10, "normal", new Vector2(-47, 7), 6, 1);
				}
				if (frame == 18)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool Fair()
	{
		if (frame == 2 || frame == 11)
		{
			CreateHitbox(35, 47, 3, 76, 10, 150, 3, "normal", new Vector2(60, -7), 0, 1);
		}
		if (frame == 18)
		{
			return true;
		}
		return false;
	}

	bool Dair()
	{
		if (frame == 2 || frame == 5 || frame == 9 || frame == 14)
		{
			CreateHitbox(36, 58, 2, 290, 140, 0, 2, "normal", new Vector2(28, 17), 0, 1);
		}
		if (frame == 17)
		{
			return true;
		}
		return false;
	}

	public void PlayAnimation(string animationName)
	{
		anim.Play(animationName);
	}

}
