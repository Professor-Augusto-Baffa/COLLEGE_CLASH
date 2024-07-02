using Godot;
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

		/*
		if (Landing() == true)
		{
			parent.Call("_frame");
			return states.LANDING;
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
			parent.Call("reset_ledge");

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

		if (Input.IsActionJustPressed("attack_%s" % id) && AIREAL() == true)
		{
			if (Input.IsActionPressed("up_%s" % id))
				parent.Call("_frame");

			return states.UAIR;
		}
		if (Input.IsActionPressed("down_%s" % id))
		{
			parent.Call("_frame");
			return states.DAIR;
		}

		int direction = parent.direction();

		if (direction == 1)
		{
			if (Input.IsActionPressed("left_%s" % id))
				parent.Call("_frame");
			return states.BAIR;
		}

		if (Input.IsActionPressed("right_%s" % id))
		{
			parent.Call("_frame");
			return states.FAIR;
		}
		else if (direction == -1)
		{
			if (Input.IsActionPressed("right_%s" % id))
				parent.Call("_frame");

			return states.BAIR;
		}
		if (Input.IsActionPressed("left_%s" % id))
		{
			parent.Call("_frame");
			return states.FAIR;
		}

		
		parent.Call("_frame");
		return states.NAIR;

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
			*/

		return 0;



		/*
		public object GetTransition(Node parent, double delta)
		{
			Mel the_parent = parent as Mel;

			// the_parent.Velocity * 2, Vector2.Zero, Vector2.Up
			the_parent.MoveAndSlide();
			the_parent.States = state.ToString();
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


	}
}
