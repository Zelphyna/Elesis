extends Control

const VIRTUAL_SIZE: Vector2 = Vector2(2564.0, 1204.0)
const BG_TEXTURE: Texture2D = preload("res://Elesis/images/charui/elesis_character_select_bg.png")

func _ready() -> void:
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	_build_scene()
	_apply_layout()


func _build_scene() -> void:
	var background := TextureRect.new()
	background.texture = BG_TEXTURE
	background.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	background.stretch_mode = TextureRect.STRETCH_SCALE
	background.size = VIRTUAL_SIZE
	background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(background)


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
