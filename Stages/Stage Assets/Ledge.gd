#The Dimensions for the CollisionShape2D is 30 x 20.6
extends Area2D

@export var ledge_Side = "Left" # (String,"Left", "Right")
@onready var label = $"Label"
@onready var collision = $"CollisionShape2D"
var is_grabbed = false

@onready var connected_bodies = []

func _on_Ledge_body_entered(body):
	print(body)
	if connected_bodies.is_empty():
		connected_bodies.push_back(body)
	else:
		is_grabbed = true


func _on_Ledge_body_exited(body):
	print(connected_bodies.back())
	if body == connected_bodies.back():
		connected_bodies.clear()
		is_grabbed = false

func _ready():
	if ledge_Side == "Left":
		label.text = "Ledge_L"
	else:
		label.text = "Ledge_R"

