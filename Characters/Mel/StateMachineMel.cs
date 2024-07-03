/*using Godot;
using System;
using System.ComponentModel;

public partial class StateMachineMel : StateMachine
{
	[Export]
	public int id = 1;

	public override void _Ready()
	{
		// Adicione os estados
		AddState("STAND");
		AddState("JUMP_SQUAT");
		AddState("SHORT_HOP");
		AddState("FULL_HOP");
		AddState("DASH");
		AddState("RUN");
		AddState("WALK");
		AddState("MOONWALK");
		AddState("TURN");
		AddState("CROUCH");
		AddState("AIR");
		AddState("LANDING");
		AddState("AIR_DODGE");
		AddState("FREE_FALL");
		AddState("LEDGE_CATCH");
		AddState("LEDGE_HOLD");
		AddState("LEDGE_CLIMB");
		AddState("LEDGE_JUMP");
		AddState("LEDGE_ROLL");
		AddState("HITFREEZE");
		AddState("HITSTUN");
		AddState("PARRY");
		AddState("ROLL_RIGHT");
		AddState("ROLL_LEFT");
		AddState("GRABBED");
		AddState("STUNNED");
		AddState("GROUND_ATTACK");
		AddState("JAB");
		AddState("JAB_1");
		AddState("DOWN_TILT");
		AddState("UP_TILT");
		AddState("FORWARD_TILT");
		AddState("NEUTRAL_SPECIAL");
		AddState("AIR_ATTACK");
		AddState("NAIR");
		AddState("UAIR");
		AddState("BAIR");
		AddState("FAIR");
		AddState("DAIR");

		// Defina o estado inicial
		CallDeferred("SetState", "STAND");
	}

	public void StateLogic(Node parent, float delta)
	{
		parent.Call("UpdateFrames", delta);
		parent.Call("_PhysicsProcess", delta);
		if ((int)parent.Get("regrab") > 0) // Supondo que 'regrab' seja uma propriedade que precisa ser acessada assim
		{
			parent.Set("regrab", (int)parent.Get("regrab") - 1);
		}
		parent.Call("_hit_pause", delta);
	}

	public int GetTransition(float delta)
	{
		// TODO: Converter o snap em Godot 4.0 para float, não vector como no Godot 3 - valor anterior `Vector2.ZERO`
		parent.Call("SetUpDirection", Vector2.Zero, Vector2.Up);
		parent.Call("MoveAndSlide");




		if (Landing() == true)
		{
			parent.Call("_frame");
			SetState("LANDING");
		}

		if (Falling() == true)
			return states.AIR;

		if (ResetModulate() == true)
			parent.Call("sprite.set_modulate", new Color(1, 1, 1, 1));

		if (Ledge() == true)
		{
			parent.Call("_frame");
			return states.LEDGE_CATCH;
		}
		else
			parent.Call("ResetLedge");

		if (Input.IsActionJustPressed("attack_%s" % id) && TILT() == true)
		{
			parent.Call("_frame");
			return states.GROUND_ATTACK;
		}

		if (Input.IsActionJustPressed("special_%s" % id) && SPECIAL() == true)
		{
			parent.Call("_frame");
			return states.NEUTRAL_SPECIAL;
		}

		if (Input.IsActionJustPressed($"attack_{id}") && AIREAL() == true)
		{
			if (Input.IsActionPressed($"up_{id}"))
			{
				parent._frame();
				return states.UAIR;
			}
			if (Input.IsActionPressed($"down_{id}"))
			{
				parent._frame();
				return states.DAIR;
			}
			switch (parent.direction())
			{
				case 1:
					if (Input.IsActionPressed($"left_{id}"))
					{
						parent._frame();
						return states.BAIR;
					}
					if (Input.IsActionPressed($"right_{id}"))
					{
						parent._frame();
						return states.FAIR;
					}
					break;
				case -1:
					if (Input.IsActionPressed($"right_{id}"))
					{
						parent._frame();
						return states.BAIR;
					}
					if (Input.IsActionPressed($"left_{id}"))
					{
						parent._frame();
						return states.FAIR;
					}
					break;
			}
			parent._frame();
			return states.NAIR;
		}

		if (Input.IsActionJustPressed("shield_%s" % id) && AIREAL() && parent.cooldown == 0)
			parent.Call("l_cancel", 11);
		parent.cooldown = 40;
		GD.Print("L_cancel is true");

		if (Input.IsActionPressed("shield_%s" % id) && can_roll() == true && parent.cooldown == 0 && parent.shield_buffer == 2)
			if (Input.IsActionPressed("right_%s" % id))
				parent.Call("_frame");
		return states.ROLL_RIGHT;

		else if (Input.IsActionPressed("left_%s" % id))
			parent.Call("_frame");
		return states.ROLL_LEFT;

		else
			parent.Call("_frame");
		return states.PARRY;

		switch (state):

		case states.STAND:
			parent.Call("reset_Jumps");
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;
			if (Input.IsActionPressed("down_%s" % id))
				parent.Call("_frame");
			return states.CROUCH;
			if (Input.get_action_strength("right_%s" % id) == 1)
				parent.velocity.x = parent.RUNSPEED;
			parent.Call("_frame");
			parent.turn(false);
			return states.DASH;
			if (Input.get_action_strength("left_%s" % id) == 1)
				parent.velocity.x = -parent.RUNSPEED;
			parent.Call("_frame");
			parent.turn(true);
			return states.DASH;
			if (parent.velocity.x > 0 && state == states.STAND)
				parent.velocity.x += -parent.TRACTION * 1;
			parent.elocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);

			else if (parent.velocity.x < 0 && state == states.STAND)
				parent.velocity.x += parent.TRACTION * 1;
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
			break;

		case states.JUMP_SQUAT:
			if (parent.frame == parent.jump_squat)
				if (!Input.IsActionPressed("jump_%s" % id))
					parent.velocity.x = Mathf.Lerp(parent.velocity.x, 0, 0.08f);
			parent.Call("_frame");
			return states.SHORT_HOP;

				else
				parent.velocity.x = Mathf.Lerp(parent.velocity.x, 0, 0.08f);
			parent.Call("_frame");
			return states.FULL_HOP;
			break;

		case states.SHORT_HOP:
			parent.velocity.y = -parent.JUMPFORCE;
			parent.Call("_frame");
			return states.AIR;
			break;

		case states.FULL_HOP:
			parent.velocity.y = -parent.MAX_JUMPFORCE;
			parent.Call("_frame");
			return states.AIR;
			break;

		case states.DASH:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;

			else if (Input.IsActionPressed("left_%s" % id))
				if (parent.velocity.x > 0)
					parent.Call("_frame");
			parent.velocity.x = -parent.DASHSPEED;
			if (parent.frame <= parent.dash_duration - 1)
				if (Input.IsActionJustPressed("down_%s" % id))
					parent.Call("_frame");
			return states.MOONWALK;
			parent.turn(true);
			return states.DASH;

				else
				parent.turn(true);
			parent.Call("_frame");
			return states.RUN;

			else if (Input.IsActionPressed("right_%s" % id))
				if (parent.velocity.x < 0)

					parent.Call("_frame");
			parent.velocity.x = parent.DASHSPEED;
			if (parent.frame <= parent.dash_duration - 1)
				if (Input.IsActionJustPressed("down_%s" % id))
					parent.Call("_frame");
			return states.MOONWALK;
			parent.turn(false);
			return states.DASH;

				else
				parent.turn(false);
			parent.Call("_frame");
			return states.RUN;

			else
				if (parent.frame >= parent.dash_duration - 1)
				foreach (var s in states)
					if (s != "JUMP_SQUAT")
						parent.Call("_frame");
			return states.STAND;
			break;

		case states.RUN:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;
			if (Input.IsActionJustPressed("down_%s" % id))
				parent.Call("_frame");
			return states.CROUCH;
			if (Input.get_action_strength("left_%s" % id))
				if (parent.velocity.x <= 0)
					parent.velocity.x = -parent.RUNSPEED;
			parent.turn(true);

				else
				parent.Call("_frame");
			return states.TURN;

			else if (Input.get_action_strength("right_%s" % id))
				if (parent.velocity.x >= 0)
					parent.velocity.x = parent.RUNSPEED;
			parent.turn(false);

				else
				parent.Call("_frame");
			return states.TURN;

			else
				parent.Call("_frame");
			return states.STAND;
			break;

		case states.TURN:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;
			if (parent.velocity.x > 0)
				parent.turn(true);
			parent.velocity.x += -parent.TRACTION * 2;
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);

			else if (parent.velocity.x < 0)

				parent.turn(false);
			parent.velocity.x += parent.TRACTION * 2;
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);

			else
				if (!Input.IsActionPressed("left_%s" % id) && !Input.IsActionPressed("right_%s" % id))
				parent.Call("_frame");
			return states.STAND;

				else
				parent.Call("_frame");
			return states.RUN;
			break;

		case states.MOONWALK:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;

			else if (Input.IsActionPressed("left_%s" % id) && parent.direction() == 1)
				if (parent.velocity.x > 0)
					parent.Call("_frame");
			parent.velocity.x += -parent.AIR_ACCEL * Input.get_action_strength("left_%s" % id);
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, -parent.DASHSPEED, parent.velocity.x);
			if (parent.frame <= parent.dash_duration * 2)
				parent.turn(false);
			return states.MOONWALK;

				else
				parent.turn(true);
			parent.Call("_frame");
			return states.STAND;

			else if (Input.IsActionPressed("right_%s" % id) && parent.direction() == -1)
				if (parent.velocity.x < 0)

					parent.Call("_frame");
			parent.velocity.x += parent.AIR_ACCEL * Input.get_action_strength("right_%s" % id);
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, parent.DASHSPEED);
			if (parent.frame <= parent.dash_duration * 2)
				parent.turn(true);
			return states.MOONWALK;

				else
				parent.turn(false);
			parent.Call("_frame");
			return states.STAND;

			else
				if (parent.frame >= parent.dash_duration - 1)
				foreach (var s in states)
					if (s != "JUMP_SQUAT")
						return states.STAND;
			break;

		case states.WALK:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;
			if (Input.IsActionJustPressed("down_%s" % id))
				parent.Call("_frame");
			return states.CROUCH;
			if (Input.get_action_strength("left_%s" % id))
				parent.velocity.x = -parent.WALKSPEED * Input.get_action_strength("left_%s" % id);
			parent.turn(true);

			else if (Input.get_action_strength("right_%s" % id))
				parent.velocity.x = parent.WALKSPEED * Input.get_action_strength("right_%s" % id);
			parent.turn(false);

			else
				parent.Call("_frame");
			return states.STAND;
			break;

		case states.CROUCH:
			if (Input.IsActionJustPressed("jump_%s" % id))
				parent.Call("_frame");
			return states.JUMP_SQUAT;
			if (Input.is_action_just_released("down_%s" % id))
				parent.Call("_frame");
			return states.STAND;

			else if (parent.velocity.x > 0)
				if (parent.velocity.x > parent.RUNSPEED)
					parent.velocity.x += -(parent.TRACTION * 4);
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);

				else
				parent.velocity.x += -(parent.TRACTION / 2);
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);

			else if (parent.velocity.x < 0)
				if (Mathf.Abs(parent.velocity.x) > parent.RUNSPEED)
					parent.velocity.x += parent.TRACTION * 4;
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);

				else
				parent.velocity.x += parent.TRACTION / 2;
			parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
			break;

		case states.AIR:
			AIRMOVEMENT();
			if (Input.IsActionJustPressed("shield_%s" % id))
				parent.Call("_frame");
			return states.AIR_DODGE;
			if (Input.IsActionJustPressed("jump_%s" % id) && parent.airJump > 0)
				parent.fastfall = false;
			parent.velocity.x = 0;
			parent.velocity.y = -parent.DOUBLEJUMPFORCE;
			parent.airJump -= 1;
			if (Input.IsActionPressed("left_%s" % id))
				parent.velocity.x = -parent.MAXAIRSPEED;
			else if (Input.IsActionPressed("right_%s" % id))
				parent.velocity.x = parent.MAXAIRSPEED;
			if (Input.IsActionJustPressed("special_%s" % id))
				parent.Call("_frame");
			return states.NEUTRAL_SPECIAL;

		case States.LANDING:
			if (parent.frame == 1)
			{
				if (parent.l_cancel > 0)
				{
					parent.lag_frames = Mathf.Floor(parent.lag_frames / 2);
				}
			}
			if (parent.frame <= parent.landing_frames + parent.lag_frames)
			{
				if (parent.velocity.x > 0)
				{
					parent.velocity.x -= parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					parent.velocity.x += parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}
			else
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent.lag_frames = 0;
					parent._frame();
					parent.reset_Jumps();
					return States.CROUCH;
				}
				else
				{
					parent._frame();
					parent.lag_frames = 0;
					parent.reset_Jumps();
					return States.STAND;
				}
			}
			break;

		case States.AIR_DODGE:
			if (parent.frame == 1)
			{
				parent.velocity.x = 0;
				parent.velocity.y = 0;

				bool deadzone = (Input.GetActionStrength($"right_{id}") - Input.GetActionStrength($"left_{id}") >= -0.2f &&
								 Input.GetActionStrength($"right_{id}") - Input.GetActionStrength($"left_{id}") <= 1.2f &&
								 Input.GetActionStrength($"up_{id}") - Input.GetActionStrength($"down_{id}") >= -0.2f &&
								 Input.GetActionStrength($"up_{id}") - Input.GetActionStrength($"down_{id}") <= 1.2f);

				Vector2 direction = new Vector2(Input.GetActionStrength($"right_{id}") - Input.GetActionStrength($"left_{id}"),
												Input.GetActionStrength($"down_{id}") - Input.GetActionStrength($"up_{id}"));
				if (deadzone)
				{
					direction = Vector2.Zero;
				}
				parent.velocity = parent.air_dodge_speed * direction.Normalized();
				if (Mathf.Abs(parent.velocity.x) == Mathf.Abs(parent.velocity.y))
				{
					parent.velocity.x /= 1.15f;
					parent.velocity.y /= 1.15f;
				}
				parent.lag_frames = 3;
			}

			if (parent.frame >= 4 && parent.frame <= 10)
			{
				parent.hurtbox.Disabled = true;
				if (parent.frame == 5)
				{
					//pass
				}
				parent.velocity.x /= 1.15f;
				parent.velocity.y /= 1.15f;
			}
			if (parent.frame >= 10 && parent.frame < 20)
			{
				parent.velocity.x = 0;
				parent.velocity.y = 0;
			}
			else if (parent.frame == 20)
			{
				parent.lag_frames = 8;
				parent.frame = 0;
				parent._frame();
				return States.FREE_FALL;
			}
			if (parent.IsOnFloor())
			{
				parent.frame = 0;
				if (parent.velocity.y > 0)
				{
					parent.velocity.y = 0;
				}
				//parent.reset_platform();
				parent.fastfall = false;
				parent._frame();
				return States.LANDING;
			}
			break;

		case States.FREE_FALL:
			if (parent.velocity.y < parent.MAXFALLSPEED)
			{
				parent.velocity.y += parent.FALLSPEED;
			}

			if (Input.IsActionJustPressed($"down_{id}") && parent.velocity.y > 0 && !parent.fastfall)
			{
				parent.velocity.y = parent.MAXFALLSPEED;
				parent.fastfall = true;
			}

			if (Mathf.Abs(parent.velocity.x) >= Mathf.Abs(parent.MAXAIRSPEED))
			{
				if (parent.velocity.x > 0)
				{
					if (Input.IsActionPressed($"left_{id}"))
					{
						parent.velocity.x -= parent.AIR_ACCEL;
					}
					else if (Input.IsActionPressed($"right_{id}"))
					{
						//parent.velocity.x = parent.velocity.x;
					}
				}
				if (parent.velocity.x < 0)
				{
					if (Input.IsActionPressed($"left_{id}"))
					{
						//parent.velocity.x = parent.velocity.x;
					}
					else if (Input.IsActionPressed($"right_{id}"))
					{
						parent.velocity.x += parent.AIR_ACCEL;
					}
				}
			}
			else if (Mathf.Abs(parent.velocity.x) < Mathf.Abs(parent.MAXAIRSPEED))
			{
				if (Input.IsActionPressed($"left_{id}"))
				{
					parent.velocity.x -= parent.AIR_ACCEL;
				}
				if (Input.IsActionPressed($"right_{id}"))
				{
					parent.velocity.x += parent.AIR_ACCEL;
				}
			}
			if (!Input.IsActionPressed($"left_{id}") && !Input.IsActionPressed($"right_{id}"))
			{
				if (parent.velocity.x < 0)
				{
					parent.velocity.x += (parent.AIR_ACCEL / 2);
				}
				else if (parent.velocity.x > 0)
				{
					parent.velocity.x -= (parent.AIR_ACCEL / 2);
				}
			}
			break;

		case States.LEDGE_CATCH:
			if (parent.frame > 7)
			{
				parent.lag_frames = 0;
				parent.reset_Jumps();
				parent._frame();
				return States.LEDGE_HOLD;
			}
			break;

		case States.LEDGE_HOLD:
			if (parent.frame >= 390) // 3.5 seconds
			{
				parent.Position += new Vector2(0, -25);
				parent._frame();
				return States.AIR;
			}
			if (Input.IsActionJustPressed($"down_{id}"))
			{
				parent.fastfall = true;
				parent.regrab = 30;
				parent.reset_ledge();
				parent.Position += new Vector2(0, -25);
				parent.catche = false;
				parent._frame();
				return States.AIR;
			}
			else if (parent.Ledge_Grab_F.get_target_position().x > 0) // Facing Right
			{
				if (Input.IsActionJustPressed($"left_{id}"))
				{
					parent.velocity.x = (parent.AIR_ACCEL / 2);
					parent.regrab = 30;
					parent.reset_ledge();
					parent.Position += new Vector2(0, -25);
					parent.catche = false;
					parent._frame();
					return States.AIR;
				}
				else if (Input.IsActionJustPressed($"right_{id}"))
				{
					parent._frame();
					return States.LEDGE_CLIMB;
				}
				else if (Input.IsActionJustPressed($"shield_{id}"))
				{
					parent._frame();
					return States.LEDGE_ROLL;
				}
				else if (Input.IsActionJustPressed($"jump_{id}"))
				{
					parent._frame();
					return States.LEDGE_JUMP;
				}
			}
			else if (parent.Ledge_Grab_F.get_target_position().x < 0) // Facing Left
			{
				if (Input.IsActionJustPressed($"right_{id}"))
				{
					parent.velocity.x = (parent.AIR_ACCEL / 2);
					parent.regrab = 30;
					parent.reset_ledge();
					parent.Position += new Vector2(0, -25);
					parent.catche = false;
					parent._frame();
					return States.AIR;
				}
				else if (Input.IsActionJustPressed($"left_{id}"))
				{
					parent._frame();
					return States.LEDGE_CLIMB;
				}
				else if (Input.IsActionJustPressed($"shield_{id}"))
				{
					parent._frame();
					return States.LEDGE_ROLL;
				}
				else if (Input.IsActionJustPressed($"jump_{id}"))
				{
					parent._frame();
					return States.LEDGE_JUMP;
				}
			}
			break;

		case States.LEDGE_CLIMB:
			if (parent.frame == 1)
			{
				//pass
			}
			if (parent.frame == 5)
			{
				parent.Position += new Vector2(0, -25);
			}
			if (parent.frame == 10)
			{
				parent.Position += new Vector2(0, -25);
			}
			if (parent.frame == 20)
			{
				parent.Position += new Vector2(0, -25);
			}
			if (parent.frame == 22)
			{
				parent.catche = false;
				parent.Position += new Vector2(0, -25);
				parent.Position += new Vector2(50 * parent.direction(), 0);
			}
			if (parent.frame == 25)
			{
				parent.velocity.y = 0;
				parent.velocity.x = 0;
				parent.MoveAndCollide(new Vector2(parent.direction() * 20, 50));
			}
			if (parent.frame == 30)
			{
				parent.reset_ledge();
				parent._frame();
				return States.STAND;
			}
			break;

		case States.LEDGE_JUMP:
			if (parent.frame > 14)
			{
				if (Input.IsActionJustPressed($"attack_{id}"))
				{
					parent._frame();
					return States.AIR_ATTACK;
				}
				if (Input.IsActionJustPressed($"special_{id}"))
				{
					parent._frame();
					return States.SPECIAL;
				}
			}
			if (parent.frame == 5)
			{
				parent.reset_ledge();
				parent.Position += new Vector2(0, -20);
			}
			if (parent.frame == 10)
			{
				parent.catche = false;
				parent.Position += new Vector2(0, -20);
				if (Input.IsActionJustPressed($"jump_{id}") && parent.airJump > 0)
				{
					parent.fastfall = false;
					parent.velocity.y = -parent.DOUBLEJUMPFORCE;
					parent.velocity.x = 0;
					parent.airJump -= 1;
					parent._frame();
					return States.AIR;
				}
			}
			if (parent.frame == 15)
			{
				parent.Position += new Vector2(0, -20);
				parent.velocity.y -= parent.DOUBLEJUMPFORCE;
				parent.velocity.x += 220 * parent.direction();
				if (Input.IsActionJustPressed($"jump_{id}") && parent.airJump > 0)
				{
					parent.fastfall = false;
					parent.velocity.y = -parent.DOUBLEJUMPFORCE;
					parent.velocity.x = 0;
					parent.airJump -= 1;
					parent._frame();
					return States.AIR;
				}
				if (Input.IsActionJustPressed($"attack_{id}"))
				{
					parent._frame();
					return States.AIR_ATTACK;
				}
			}
			else if (parent.frame > 15 && parent.frame < 20)
			{
				parent.velocity.y += parent.FALLSPEED;
				if (Input.IsActionJustPressed($"jump_{id}") && parent.airJump > 0)
				{
					parent.fastfall = false;
					parent.velocity.y = -parent.DOUBLEJUMPFORCE;
					parent.velocity.x = 0;
					parent.airJump -= 1;
					parent._frame();
					return States.AIR;
				}
				if (Input.IsActionJustPressed($"attack_{id}"))
				{
					parent._frame();
					return States.AIR_ATTACK;
				}
			}
			if (parent.frame == 20)
			{
				parent._frame();
				return States.AIR;
			}
			break;

		case States.LEDGE_ROLL:
			if (parent.frame == 1)
			{
				//pass
			}
			if (parent.frame == 5)
			{
				parent.Position += new Vector2(0, -30);
			}
			if (parent.frame == 10)
			{
				parent.Position += new Vector2(0, -30);
			}
			if (parent.frame == 20)
			{
				parent.catche = false;
				parent.Position += new Vector2(0, -30);
			}
			if (parent.frame == 22)
			{
				parent.Position += new Vector2(0, -30);
				parent.Position += new Vector2(50 * parent.direction(), 0);
			}
			if (parent.frame > 22 && parent.frame < 28)
			{
				parent.Position += new Vector2(30 * parent.direction(), 0);
			}
			if (parent.frame == 29)
			{
				parent.MoveAndCollide(new Vector2(parent.direction() * 20, 50));
			}
			if (parent.frame == 30)
			{
				parent.velocity.y = 0;
				parent.velocity.x = 0;
				parent.reset_ledge();
				parent._frame();
				return States.STAND;
			}
			break;

		case States.HITFREEZE:
			if (parent.freezeframes == 0)
			{
				parent._frame();
				parent.velocity.x = kbx;
				parent.velocity.y = kby;
				parent.hdecay = hd;
				parent.vdecay = vd;
				return States.HITSTUN;
			}
			parent.Position = pos;
			break;

		case States.HITSTUN:
			if (parent.knockback >= 10)
			{
				var bounce = parent.MoveAndCollide(parent.velocity * delta);
				if (parent.IsOnWall())
				{
					parent.velocity.x = kbx - parent.velocity.x;
					parent.velocity = parent.velocity.Bounce(parent.GetWallNormal()) * 0.8f;
					parent.hdecay *= -1;
					parent.hitstun = Mathf.Round(parent.hitstun * 0.8f);
				}
				if (parent.IsOnFloor())
				{
					parent.velocity.y = kby - parent.velocity.y;
					parent.velocity = parent.velocity.Bounce(parent.GetFloorNormal()) * 0.8f;
					parent.hitstun = Mathf.Round(parent.hitstun * 0.8f);
				}
			}
			if (parent.velocity.y < 0)
			{
				parent.velocity.y += parent.vdecay * 0.5f * Engine.TimeScale;
				parent.velocity.y = Mathf.Clamp(parent.velocity.y, parent.velocity.y, 0);
			}
			if (parent.velocity.x < 0)
			{
				parent.velocity.x += (parent.hdecay) * 0.4f * -1 * Engine.TimeScale;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
			}
			else if (parent.velocity.x > 0)
			{
				parent.velocity.x -= parent.hdecay * 0.4f * Engine.TimeScale;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
			}

			if (parent.frame >= parent.hitstun)
			{
				if (parent.knockback >= 24)
				{
					parent._frame();
					return States.AIR;
				}
				else
				{
					parent._frame();
					return States.AIR;
				}
			}
			else if (parent.frame > 60 * 5)
			{
				return States.AIR;
			}

		case States.ROLL_RIGHT:
			parent.Turn(true);
			if (parent.frame == 1)
			{
				parent.velocity.x = 0;
			}
			if (parent.frame == 4)
			{
				parent.velocity.x = parent.ROLL_DISTANCE;
				parent.hurtbox.Disabled = true; // Disable Hurtbox
			}
			if (parent.frame == 20)
			{
				parent.hurtbox.Disabled = false; // Enable Hurtbox
			}
			if (parent.frame > 19)
			{
				parent.velocity.x -= parent.TRACTION * 5;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				if (parent.velocity.x == 0)
				{
					parent.cooldown = 20; // Can only roll again after 20 frames
					parent.lag_frames = 10;
					parent._Frame();
					state = States.LANDING;
				}
			}
			break;

		case States.ROLL_LEFT:
			parent.Turn(false);
			if (parent.frame == 1)
			{
				parent.velocity.x = 0;
			}
			if (parent.frame == 4)
			{
				parent.velocity.x = -parent.ROLL_DISTANCE;
				parent.hurtbox.Disabled = true; // Disable Hurtbox
			}
			if (parent.frame == 20)
			{
				parent.hurtbox.Disabled = false; // Enable Hurtbox
			}
			if (parent.frame > 19)
			{
				parent.velocity.x += parent.TRACTION * 5;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				if (parent.velocity.x == 0)
				{
					parent.cooldown = 20; // Can only roll again after 20 frames
					parent.lag_frames = 10;
					parent._Frame();
					state = States.LANDING;
				}
			}
			break;

		case States.NEUTRAL_SPECIAL:
			if (!AIREAL())
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 10;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 10;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}
			else
			{
				AIRMOVEMENT();
			}

			if (parent.frame <= 1)
			{
				if (parent.projectile_cooldown == 1)
				{
					parent.projectile_cooldown--;
				}
				if (parent.projectile_cooldown == 0)
				{
					parent.projectile_cooldown++;
					parent._Frame();
					NeutralSpecial();
				}
			}

			if (parent.frame < 14)
			{
				if (Input.IsActionJustPressed($"special_{id}"))
				{
					parent._Frame();
					state = States.NEUTRAL_SPECIAL;
				}
			}

			if (NeutralSpecial())
			{
				if (AIREAL())
				{
					state = States.AIR;
				}
				else if (parent.frame == 14)
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.AIR_ATTACK:
			AIRMOVEMENT();
			if (Input.IsActionPressed($"up_{id}"))
			{
				parent._Frame();
				state = States.UAIR;
			}
			else if (Input.IsActionPressed($"down_{id}"))
			{
				parent._Frame();
				state = States.DAIR;
			}
			else if (parent.Direction() == 1)
			{
				if (Input.IsActionPressed($"left_{id}"))
				{
					parent._Frame();
					state = States.BAIR;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					parent._Frame();
					state = States.FAIR;
				}
			}
			else if (parent.Direction() == -1)
			{
				if (Input.IsActionPressed($"right_{id}"))
				{
					parent._Frame();
					state = States.BAIR;
				}
				else if (Input.IsActionPressed($"left_{id}"))
				{
					parent._Frame();
					state = States.FAIR;
				}
			}

			parent._Frame();
			state = States.NAIR;
			break;

		case States.NAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("nair");
				Nair();
			}

			if (Nair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else if (parent.frame < 5 || parent.frame > 15)
			{
				parent.lag_frames = 0;
			}
			else
			{
				parent.lag_frames = 17;
			}
			break;

		case States.UAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("uair");
				Uair();
			}

			if (Uair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 13;
			}
			break;

		case States.BAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("bair");
				Bair();
			}

			if (Bair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 9;
			}
			break;

		case States.FAIR:
			AIRMOVEMENT();
			if (Input.IsActionJustPressed($"jump_{id}") && parent.airJump > 0)
			{
				parent.fastfall = false;
				parent.velocity.x = 0;
				parent.velocity.y = -parent.DOUBLEJUMPFORCE;
				parent.airJump--;
				if (Input.IsActionPressed($"left_{id}"))
				{
					parent.velocity.x = -parent.MAXAIRSPEED;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					parent.velocity.x = parent.MAXAIRSPEED;
				}
				state = States.AIR;
			}

			if (parent.frame == 0)
			{
				GD.Print("fair");
				Fair();
			}

			if (Fair())
			{
				parent.lag_frames = 30;
				parent._Frame();
				state = States.FAIR;
			}
			else
			{
				parent.lag_frames = 18;
			}
			break;

		case States.DAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("dair");
				Dair();
			}

			if (Dair())
			{
				parent.lag_frames = 0;
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 17;
			}
			break;

		case States.GROUND_ATTACK:
			if (Input.IsActionPressed($"up_{id}"))
			{
				parent._Frame();
				state = States.UP_TILT;
			}
			else if (Input.IsActionPressed($"down_{id}"))
			{
				parent._Frame();
				state = States.DOWN_TILT;
			}
			else if (Input.IsActionPressed($"left_{id}"))
			{
				parent.Turn(true);
				parent._Frame();
				state = States.FORWARD_TILT;
			}
			else if (Input.IsActionPressed($"right_{id}"))
			{
				parent.Turn(false);
				parent._Frame();
				state = States.FORWARD_TILT;
			}
			else
			{
				parent._Frame();
				state = States.JAB;
			}
			break;

		case States.JAB:
			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
				Jab();
			}

			if (Jab())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			else
			{
				parent._Frame();
				state = States.JAB_1;
			}
			break;

		case States.JAB_1:
			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
				Jab1();
			}

			if (Jab1())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.DOWN_TILT:
			if (parent.frame == 0)
			{
				DownTilt();
			}

			if (parent.frame >= 1)
			{
				if (parent.velocity.x > 0)
				{
					parent.velocity.x -= parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					parent.velocity.x += parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (DownTilt())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.UP_TILT:
			if (parent.frame == 0)
			{
				parent._Frame();
				UpTilt();
			}

			if (parent.frame >= 1)
			{
				if (parent.velocity.x > 0)
				{
					parent.velocity.x -= parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					parent.velocity.x += parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (UpTilt())
			{
				parent._Frame();
				state = States.STAND;
			}
			break;

		case States.FORWARD_TILT:
			if (parent.frame == 0)
			{
				parent._Frame();
				ForwardTilt();
			}

			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (ForwardTilt())
			{
				if (Input.IsActionPressed($"left_{id}"))
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
					parent._Frame();
					state = States.WALK;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
					parent._Frame();
					state = States.WALK;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}

		case States.ROLL_RIGHT:
			parent.Turn(true);
			if (parent.frame == 1)
			{
				parent.velocity.x = 0;
			}
			if (parent.frame == 4)
			{
				parent.velocity.x = parent.ROLL_DISTANCE;
				parent.hurtbox.Disabled = true; // Disable Hurtbox
			}
			if (parent.frame == 20)
			{
				parent.hurtbox.Disabled = false; // Enable Hurtbox
			}
			if (parent.frame > 19)
			{
				parent.velocity.x -= parent.TRACTION * 5;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				if (parent.velocity.x == 0)
				{
					parent.cooldown = 20; // Can only roll again after 20 frames
					parent.lag_frames = 10;
					parent._Frame();
					state = States.LANDING;
				}
			}
			break;

		case States.ROLL_LEFT:
			parent.Turn(false);
			if (parent.frame == 1)
			{
				parent.velocity.x = 0;
			}
			if (parent.frame == 4)
			{
				parent.velocity.x = -parent.ROLL_DISTANCE;
				parent.hurtbox.Disabled = true; // Disable Hurtbox
			}
			if (parent.frame == 20)
			{
				parent.hurtbox.Disabled = false; // Enable Hurtbox
			}
			if (parent.frame > 19)
			{
				parent.velocity.x += parent.TRACTION * 5;
				parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				if (parent.velocity.x == 0)
				{
					parent.cooldown = 20; // Can only roll again after 20 frames
					parent.lag_frames = 10;
					parent._Frame();
					state = States.LANDING;
				}
			}
			break;

		case States.NEUTRAL_SPECIAL:
			if (!AIREAL())
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 10;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 10;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}
			else
			{
				AIRMOVEMENT();
			}

			if (parent.frame <= 1)
			{
				if (parent.projectile_cooldown == 1)
				{
					parent.projectile_cooldown--;
				}
				if (parent.projectile_cooldown == 0)
				{
					parent.projectile_cooldown++;
					parent._Frame();
					NeutralSpecial();
				}
			}

			if (parent.frame < 14)
			{
				if (Input.IsActionJustPressed($"special_{id}"))
				{
					parent._Frame();
					state = States.NEUTRAL_SPECIAL;
				}
			}

			if (NeutralSpecial())
			{
				if (AIREAL())
				{
					state = States.AIR;
				}
				else if (parent.frame == 14)
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.AIR_ATTACK:
			AIRMOVEMENT();
			if (Input.IsActionPressed($"up_{id}"))
			{
				parent._Frame();
				state = States.UAIR;
			}
			else if (Input.IsActionPressed($"down_{id}"))
			{
				parent._Frame();
				state = States.DAIR;
			}
			else if (parent.Direction() == 1)
			{
				if (Input.IsActionPressed($"left_{id}"))
				{
					parent._Frame();
					state = States.BAIR;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					parent._Frame();
					state = States.FAIR;
				}
			}
			else if (parent.Direction() == -1)
			{
				if (Input.IsActionPressed($"right_{id}"))
				{
					parent._Frame();
					state = States.BAIR;
				}
				else if (Input.IsActionPressed($"left_{id}"))
				{
					parent._Frame();
					state = States.FAIR;
				}
			}

			parent._Frame();
			state = States.NAIR;
			break;

		case States.NAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("nair");
				Nair();
			}

			if (Nair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else if (parent.frame < 5 || parent.frame > 15)
			{
				parent.lag_frames = 0;
			}
			else
			{
				parent.lag_frames = 17;
			}
			break;

		case States.UAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("uair");
				Uair();
			}

			if (Uair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 13;
			}
			break;

		case States.BAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("bair");
				Bair();
			}

			if (Bair())
			{
				parent.lag_frames = 0;
				parent._Frame();
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 9;
			}
			break;

		case States.FAIR:
			AIRMOVEMENT();
			if (Input.IsActionJustPressed($"jump_{id}") && parent.airJump > 0)
			{
				parent.fastfall = false;
				parent.velocity.x = 0;
				parent.velocity.y = -parent.DOUBLEJUMPFORCE;
				parent.airJump--;
				if (Input.IsActionPressed($"left_{id}"))
				{
					parent.velocity.x = -parent.MAXAIRSPEED;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					parent.velocity.x = parent.MAXAIRSPEED;
				}
				state = States.AIR;
			}

			if (parent.frame == 0)
			{
				GD.Print("fair");
				Fair();
			}

			if (Fair())
			{
				parent.lag_frames = 30;
				parent._Frame();
				state = States.FAIR;
			}
			else
			{
				parent.lag_frames = 18;
			}
			break;

		case States.DAIR:
			AIRMOVEMENT();
			if (parent.frame == 0)
			{
				GD.Print("dair");
				Dair();
			}

			if (Dair())
			{
				parent.lag_frames = 0;
				state = States.AIR;
			}
			else
			{
				parent.lag_frames = 17;
			}
			break;

		case States.GROUND_ATTACK:
			if (Input.IsActionPressed($"up_{id}"))
			{
				parent._Frame();
				state = States.UP_TILT;
			}
			else if (Input.IsActionPressed($"down_{id}"))
			{
				parent._Frame();
				state = States.DOWN_TILT;
			}
			else if (Input.IsActionPressed($"left_{id}"))
			{
				parent.Turn(true);
				parent._Frame();
				state = States.FORWARD_TILT;
			}
			else if (Input.IsActionPressed($"right_{id}"))
			{
				parent.Turn(false);
				parent._Frame();
				state = States.FORWARD_TILT;
			}
			else
			{
				parent._Frame();
				state = States.JAB;
			}
			break;

		case States.JAB:
			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
				Jab();
			}

			if (Jab())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			else
			{
				parent._Frame();
				state = States.JAB_1;
			}
			break;

		case States.JAB_1:
			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 20;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
				Jab1();
			}

			if (Jab1())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.DOWN_TILT:
			if (parent.frame == 0)
			{
				DownTilt();
			}

			if (parent.frame >= 1)
			{
				if (parent.velocity.x > 0)
				{
					parent.velocity.x -= parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					parent.velocity.x += parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (DownTilt())
			{
				if (Input.IsActionPressed($"down_{id}"))
				{
					parent._Frame();
					state = States.CROUCH;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}
			break;

		case States.UP_TILT:
			if (parent.frame == 0)
			{
				parent._Frame();
				UpTilt();
			}

			if (parent.frame >= 1)
			{
				if (parent.velocity.x > 0)
				{
					parent.velocity.x -= parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					parent.velocity.x += parent.TRACTION * 3;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (UpTilt())
			{
				parent._Frame();
				state = States.STAND;
			}
			break;

		case States.FORWARD_TILT:
			if (parent.frame == 0)
			{
				parent._Frame();
				ForwardTilt();
			}

			if (parent.frame <= 1)
			{
				if (parent.velocity.x > 0)
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION * 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
				}
				else if (parent.velocity.x < 0)
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION * 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
				}
			}

			if (ForwardTilt())
			{
				if (Input.IsActionPressed($"left_{id}"))
				{
					if (parent.velocity.x < -parent.DASHSPEED)
					{
						parent.velocity.x = -parent.DASHSPEED;
					}
					parent.velocity.x += parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
					parent._Frame();
					state = States.WALK;
				}
				else if (Input.IsActionPressed($"right_{id}"))
				{
					if (parent.velocity.x > parent.DASHSPEED)
					{
						parent.velocity.x = parent.DASHSPEED;
					}
					parent.velocity.x -= parent.TRACTION / 2;
					parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
					parent._Frame();
					state = States.WALK;
				}
				else
				{
					parent._Frame();
					state = States.STAND;
				}
			}

		case States.GRABBED:
			foreach (Node body in GetTree().GetNodesInGroup("Character"))
			{
				if (body.Name == temp_body)
				{
					// Assuming `StateMachine` is a component or class within `body`
					// Adjust the access to `state` based on your actual structure
					// e.g., body.GetNode<StateMachine>("StateMachine").state
					if (body.GetNode<StateMachine>("StateMachine").state != temp_state)
					{
						state = States.STAND;
						return;
					}
				}
			}
			break;

		case States.STUNNED:
			if (parent.frame >= 60 * 3)
			{
				parent._Frame();
				state = States.STAND;
				return;
			}
			else
			{
				if (parent.IsOnFloor())
				{
					if (parent.velocity.x > 0)
					{
						if (parent.velocity.x > parent.DASHSPEED)
						{
							parent.velocity.x = parent.DASHSPEED;
						}
						parent.velocity.x -= parent.TRACTION;
						parent.velocity.x = Mathf.Clamp(parent.velocity.x, 0, parent.velocity.x);
					}
					else if (parent.velocity.x < 0)
					{
						if (parent.velocity.x < -parent.DASHSPEED)
						{
							parent.velocity.x = -parent.DASHSPEED;
						}
						parent.velocity.x += parent.TRACTION;
						parent.velocity.x = Mathf.Clamp(parent.velocity.x, parent.velocity.x, 0);
					}
				}

				if (!parent.IsOnFloor())
				{
					AIRMOVEMENT(); // Assuming AIRMOVEMENT() is defined elsewhere
				}
			}
		}



		/*
		public object GetTransition(Node parent, double delta)
		{
			Mel the_parent = parent as Mel;

			// the_parent.Velocity * 2, Vector2.Zero, Vector2.Up
			the_parent.MoveAndSlide();
			the_parent.states = state.ToString();
			*/

/*
ramesLabel = GetNode<Label>("Frames");
if (framesLabel != null)
{
	framesLabel.Text = state.ToString();
}*/

/*
// Lógica do estado atual
switch (state)
{
	case "STAND":
		//if (Input.GetActionStrength("right_" + id) == 1)
		if (Input.IsActionJustPressed("right_1"))
		{
			GD.Print("left_1");
			//the_parent.velocity.X = Mel.RUNSPEED;
			the_parent.ResetFrame();
			the_parent.Turn(false);
			//SetState("DASH");
			return "DASH";
		}
		//if (Input.GetActionStrength("left_" + id) == 1)
		if (Input.IsActionJustPressed("left_1"))
		{
			GD.Print("left_1");
			//the_parent.velocity.X = -Mel.RUNSPEED;
			the_parent.ResetFrame();
			the_parent.Turn(true);
			//SetState("DASH");
			return "DASH";	
		}
		if (the_parent.Velocity.X > 0 && state == "STAND")
		{
			//the_parent.velocity.X += Mel.TRACTION * 1;
			//the_parent.velocity.X = Mathf.Clamp(the_parent.Velocity.X, the_parent.Velocity.X, 0);
		}
		else if (the_parent.Velocity.X < 0 && state == "STAND")
		{
			//the_parent.velocity.X += Mel.TRACTION * 1;
			//the_parent.velocity.X = Mathf.Clamp(the_parent.Velocity.X, the_parent.Velocity.X, 0);
		}

		break;

	case "JUMP_SQUAT":
		// Lógica do estado de salto
		break;

	case "SHORT_HOP":
		// Lógica do salto curto
		break;

	case "FULL_HOP":
		// Lógica do salto alto
		break;

	case "DASH":
		if (Input.IsActionPressed("left_" + id))
		{
			if (the_parent.Velocity.X > 0)
				the_parent.ResetFrame();

			//the_parent.velocity.X = -Mel.DASHSPEED;
		}

		else if (Input.IsActionPressed("right_" + id))
		{
			if (the_parent.Velocity.X < 0)
				the_parent.ResetFrame();

			//the_parent.velocity.X = -Mel.DASHSPEED;
		}

		else
		{
			if (the_parent.frame >= the_parent.dash_duration-1)
				//SetState("STAND");
				return "STAND";
		}

		break;

	case "MOONWALK":
		// Lógica do Moonwalk
		break;

	case "WALK":
		// Lógica do walk
		break;

	case "CROUCH":
		// Lógica do crouch
		break;	

	default:
		return "NONE";
}

return "NONE";
}
*/

/*
public new void EnterState(string oldState, string newState)
{
	// Implement behavior to execute when entering a new state
}

public new void ExitState(string oldState, string newState)
{
	// Implement behavior to execute when exiting the current state
}
/*

/*
public bool StateIncludes(object[] stateArray)
{
	foreach (var eachState in stateArray)
	{
		if (stateArray.Equals(eachState))
		{
			return true;
		}
	}
	return false;
}
*/
/*

	}

	bool Landing()
	{
		return parent.Call("IsOnFloor").AsBool();
	}	

	public bool Ledge()
	{
		throw new NotImplementedException();
	}

	public bool ResetModulate()
	{
		throw new NotImplementedException();
	}

	public bool Falling()
	{
		throw new NotImplementedException();
	}
}
*/
