extends Control

const VIRTUAL_SIZE: Vector2 = Vector2(2564.0, 1204.0)

func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	_build_scene()


func _build_scene() -> void:
	var background := ColorRect.new()
	background.color = Color(0.23, 0.28, 0.62, 1.0)
	background.size = VIRTUAL_SIZE
	background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(background)

	var accent := ColorRect.new()
	accent.color = Color(0.16, 0.20, 0.48, 1.0)
	accent.position = Vector2(VIRTUAL_SIZE.x * 0.60, 0.0)
	accent.size = Vector2(VIRTUAL_SIZE.x * 0.40, VIRTUAL_SIZE.y)
	accent.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(accent)


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
