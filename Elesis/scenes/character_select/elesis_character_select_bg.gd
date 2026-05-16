extends Control

const VIRTUAL_SIZE: Vector2 = Vector2(2564.0, 1204.0)

func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	_build_scene()
	_apply_layout()


func _build_scene() -> void:
	var background := ColorRect.new()
	background.color = Color(0.16, 0.12, 0.13, 1.0)
	background.size = VIRTUAL_SIZE
	background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(background)

	var ember_panel := ColorRect.new()
	ember_panel.color = Color(0.33, 0.08, 0.06, 1.0)
	ember_panel.position = Vector2(VIRTUAL_SIZE.x * 0.58, 0.0)
	ember_panel.size = Vector2(VIRTUAL_SIZE.x * 0.42, VIRTUAL_SIZE.y)
	ember_panel.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(ember_panel)

	var blade_light := ColorRect.new()
	blade_light.color = Color(0.82, 0.30, 0.12, 0.35)
	blade_light.position = Vector2(VIRTUAL_SIZE.x * 0.66, VIRTUAL_SIZE.y * 0.08)
	blade_light.rotation = -0.28
	blade_light.size = Vector2(VIRTUAL_SIZE.x * 0.08, VIRTUAL_SIZE.y * 1.10)
	blade_light.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(blade_light)


func _notification(what: int) -> void:
	if what == NOTIFICATION_RESIZED:
		_apply_layout()


func _apply_layout() -> void:
	var bounds: Vector2 = size
	if bounds.x <= 0.0 or bounds.y <= 0.0:
		bounds = get_viewport_rect().size

	var scale_value: float = max(bounds.x / VIRTUAL_SIZE.x, bounds.y / VIRTUAL_SIZE.y)
	scale = Vector2.ONE * scale_value
	position = (bounds - VIRTUAL_SIZE * scale_value) * 0.5
