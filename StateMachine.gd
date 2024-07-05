extends Node
class_name StateMachine

var state = null : set = set_state
var previous_state = null
var states = {}

@onready var parent = get_parent()

func _physics_process(delta):
	if state !=null:
		state_logic(delta)
		var transition = get_transition(delta)
		if transition != null:
			set_state(transition)

func state_logic(_delta):
	pass

func get_transition(_delta):
	return null

func enter_state(_new_state,_old_state):
	pass

func exit_state(_old_state,_new_state):
	pass

func set_state(new_state):
	print("------------------------")
	print("STATEMACHINE:\nO parent é: " + str(parent.name))
	print("new_state: " + str(new_state))
	print("state: " + str(state))
	print("previous_state: " + str(previous_state))
	
	previous_state = state
	print("***previous_state = state***")
	print("previous_state: " + str(previous_state))
	
	state = new_state
	print("***state = new_state***")
	print("state: " + str(state))
	print("previous_state: " + str(previous_state))
	print("------------------------")
	
	if previous_state !=null:
		exit_state(previous_state, new_state)
	if new_state !=null:
		enter_state(new_state, previous_state)

func add_state(state_name):
	states[state_name] = states.size()
