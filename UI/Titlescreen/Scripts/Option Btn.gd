extends Button

@export var path = ''
@onready var label = get_node("/root/Titlescreen/AnimationPlayer/Label")

func _on_OptionsButton_focus_entered():
	add_theme_color_override("font_outline_color", Color(1, 0.32549020648003, 0))

func _on_OptionsButton_focus_exited():
	add_theme_color_override("font_outline_color", Color(0, 0, 0))

func _on_Area_2d_area_entered(area):
	emit_signal("focus_entered")

func _on_area_2d_area_exited(area):
	emit_signal("focus_exited")

func _on_pressed():
	if (path != ''):
		get_tree().change_scene_to_file(path)
