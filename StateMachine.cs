using Godot;
using Godot.Collections;
using System;

public partial class StateMachine : Node
{
	public object state = null;
	public object previousState = null;
	public Dictionary<string, int> states = new Dictionary<string, int>();

	[Export]
	public NodePath ParentPath;

	public Node parent;

	public override void _Ready()
	{
		parent = GetNode<Node>(ParentPath);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (state != null)
		{
			StateLogic(delta);
			object transition = GetTransition(delta);
			if (transition != null)
			{
				SetState(transition);
			}
		}
	}

	public void StateLogic(double delta)
	{
		// Implement your state logic here
	}

	public object GetTransition(double delta)
	{
		return null;
	}

	public void EnterState(object newState, object oldState)
	{
		// Implement enter state logic here
	}

	public void ExitState(object oldState, object newState)
	{
		// Implement exit state logic here
	}

	public void SetState(object newState)
	{
		previousState = state;
		state = newState;

		if (previousState != null)
		{
			ExitState(previousState, newState);
		}
		if (newState != null)
		{
			EnterState(newState, previousState);
		}
	}

	public void AddState(string stateName)
	{
		states[stateName] = states.Count;
	}
}


/*using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	public string state = null;
	public string previousState = null;
	public Dictionary<string, string> states = new Dictionary<string, string>();

	public Node parent;

	public override void _Ready()
	{
		parent = GetParent<Node>();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (state != null)
		{
			StateLogic(delta);
			var transition = GetTransition(delta);
			if (transition != null)
			{
				SetState(transition);
			}
		}
	}

	public void StateLogic(double delta)
	{
		// Implement logic for the current state here
	}

	public string GetTransition(double delta)
	{
		// Implement logic for determining state transitions here
		return null;
	}

	public void EnterState(string newState, string oldState)
	{
		// Implement behavior to execute when entering a new state
	}

	public void ExitState(string oldState, string newState)
	{
		// Implement behavior to execute when exiting the current state
	}

	public void SetState(string newState)
	{
		previousState = state;
		state = newState;

		if (previousState != null)
		{
			ExitState(previousState, newState);
		}
		if (newState != null)
		{
			EnterState(newState, previousState);
		}
	}

	public void AddState(string stateName)
	{
		states[stateName] = states.Count.ToString();
	}
}*/
