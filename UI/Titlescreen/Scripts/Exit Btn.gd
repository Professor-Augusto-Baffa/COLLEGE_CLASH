extends Button

func _on_QuitButton_focus_entered():
	add_theme_color_override("font_outline_color", Color(1, 0.32549020648003, 0))

func _on_QuitButton_focus_exited():
	add_theme_color_override("font_outline_color", Color(0, 0, 0))

func _on_pressed():
	get_tree().quit()

func _on_area_2d_area_entered(_area):
	emit_signal("focus_entered")

func _on_area_2d_area_exited(_area):
	emit_signal("focus_exited")
