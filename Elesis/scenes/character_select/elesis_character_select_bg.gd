extends Control

const VIRTUAL_SIZE: Vector2 = Vector2(2564.0, 1204.0)
const TUNING_PANEL_START_VISIBLE: bool = false
const TUNING_PANEL_MARGIN: Vector2 = Vector2(24.0, 24.0)
const DEFAULT_IMAGE_POS: Vector2 = Vector2.ZERO
const DEFAULT_IMAGE_SCALE: float = 1.0
const BG_TEXTURE: Texture2D = preload("res://Elesis/images/charui/elesis_character_select_bg.png")

static var _saved_image_pos: Vector2 = DEFAULT_IMAGE_POS
static var _saved_image_scale: float = DEFAULT_IMAGE_SCALE
static var _saved_tuning_panel_visible: bool = TUNING_PANEL_START_VISIBLE

var _canvas: Control
var _background: TextureRect
var _tuning_panel: PanelContainer
var _tuning_body: VBoxContainer
var _collapse_button: Button
var _tuning_sliders: Dictionary = {}
var _image_pos: Vector2 = DEFAULT_IMAGE_POS
var _image_scale: float = DEFAULT_IMAGE_SCALE
var _tuning_panel_visible: bool = TUNING_PANEL_START_VISIBLE


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	mouse_filter = Control.MOUSE_FILTER_IGNORE
	clip_contents = true
	set_process_unhandled_input(true)
	_restore_saved_tuning_values()
	_build_scene()
	_apply_layout()


func _build_scene() -> void:
	_canvas = Control.new()
	_canvas.name = "VirtualCanvas"
	_canvas.size = VIRTUAL_SIZE
	_canvas.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_canvas.process_mode = Node.PROCESS_MODE_ALWAYS
	add_child(_canvas)

	_background = TextureRect.new()
	_background.name = "BackgroundImage"
	_background.texture = BG_TEXTURE
	_background.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	_background.stretch_mode = TextureRect.STRETCH_SCALE
	_background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_canvas.add_child(_background)
	_apply_image_tuning()

	_tuning_panel = _build_tuning_panel()
	_tuning_panel.visible = _tuning_panel_visible
	add_child(_tuning_panel)


func _notification(what: int) -> void:
	if what == NOTIFICATION_RESIZED:
		_apply_layout()


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_F3:
		_toggle_tuning_panel_visible()
		get_viewport().set_input_as_handled()


func _apply_layout() -> void:
	if _canvas == null:
		return

	var bounds: Vector2 = size
	if bounds.x <= 0.0 or bounds.y <= 0.0:
		bounds = get_viewport_rect().size

	var scale_value: float = max(bounds.x / VIRTUAL_SIZE.x, bounds.y / VIRTUAL_SIZE.y)
	_canvas.scale = Vector2.ONE * scale_value
	_canvas.position = (bounds - VIRTUAL_SIZE * scale_value) * 0.5
	_apply_tuning_panel_layout()


func _apply_image_tuning() -> void:
	if _background == null:
		return

	_background.position = _image_pos
	_background.size = VIRTUAL_SIZE * _image_scale


func _build_tuning_panel() -> PanelContainer:
	var panel: PanelContainer = PanelContainer.new()
	panel.name = "ElesisImagePlacementTuner"
	panel.custom_minimum_size = Vector2(500.0, 190.0)
	panel.mouse_filter = Control.MOUSE_FILTER_STOP
	panel.process_mode = Node.PROCESS_MODE_ALWAYS
	panel.z_index = 1000

	var margin: MarginContainer = MarginContainer.new()
	margin.add_theme_constant_override("margin_left", 12)
	margin.add_theme_constant_override("margin_top", 10)
	margin.add_theme_constant_override("margin_right", 12)
	margin.add_theme_constant_override("margin_bottom", 10)
	panel.add_child(margin)

	var layout: VBoxContainer = VBoxContainer.new()
	layout.mouse_filter = Control.MOUSE_FILTER_PASS
	margin.add_child(layout)

	var header: HBoxContainer = HBoxContainer.new()
	layout.add_child(header)

	var title: Label = Label.new()
	title.text = "Elesis Image Placement Tuner"
	title.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	header.add_child(title)

	_collapse_button = Button.new()
	_collapse_button.text = "Collapse"
	_collapse_button.pressed.connect(_toggle_tuning_panel_collapsed)
	header.add_child(_collapse_button)

	_tuning_body = VBoxContainer.new()
	_tuning_body.mouse_filter = Control.MOUSE_FILTER_PASS
	layout.add_child(_tuning_body)

	_tuning_body.add_child(_create_tuning_slider("X", "x", -1300.0, 1300.0, _image_pos.x, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Y", "y", -700.0, 700.0, _image_pos.y, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Scale", "scale", 0.25, 3.0, _image_scale, 0.01))

	var button_row: HBoxContainer = HBoxContainer.new()
	_tuning_body.add_child(button_row)

	var print_button: Button = Button.new()
	print_button.text = "Copy Values"
	print_button.pressed.connect(_print_tuning_values)
	button_row.add_child(print_button)

	var reset_button: Button = Button.new()
	reset_button.text = "Reset"
	reset_button.pressed.connect(_reset_tuning_values)
	button_row.add_child(reset_button)

	_apply_tuning_panel_layout()
	return panel


func _apply_tuning_panel_layout() -> void:
	if _tuning_panel == null:
		return
	if not _tuning_panel_visible:
		_tuning_panel.visible = false
		return

	var viewport_size: Vector2 = get_viewport().get_visible_rect().size
	if viewport_size.x <= 0.0 or viewport_size.y <= 0.0:
		viewport_size = get_viewport_rect().size

	var panel_size := Vector2(
		min(500.0, max(320.0, viewport_size.x - TUNING_PANEL_MARGIN.x * 2.0)),
		min(_current_tuning_panel_height(), max(56.0, viewport_size.y - TUNING_PANEL_MARGIN.y * 2.0))
	)
	_tuning_panel.custom_minimum_size = panel_size
	_tuning_panel.visible = true
	_tuning_panel.position = TUNING_PANEL_MARGIN
	_tuning_panel.size = panel_size


func _create_tuning_slider(label_text: String, key: String, min_value: float, max_value: float, value: float, step: float) -> HBoxContainer:
	var row: HBoxContainer = HBoxContainer.new()
	row.custom_minimum_size = Vector2(0.0, 34.0)

	var name_label: Label = Label.new()
	name_label.text = label_text
	name_label.custom_minimum_size = Vector2(80.0, 0.0)
	row.add_child(name_label)

	var slider: HSlider = HSlider.new()
	slider.min_value = min_value
	slider.max_value = max_value
	slider.step = step
	slider.value = value
	slider.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	slider.mouse_filter = Control.MOUSE_FILTER_STOP
	row.add_child(slider)
	_tuning_sliders[key] = slider

	var value_label: Label = Label.new()
	value_label.text = _format_tuning_number(value)
	value_label.custom_minimum_size = Vector2(58.0, 0.0)
	row.add_child(value_label)

	slider.value_changed.connect(_on_tuning_slider_changed.bind(key, value_label))
	return row


func _on_tuning_slider_changed(value: float, key: String, value_label: Label) -> void:
	match key:
		"x":
			_image_pos.x = value
		"y":
			_image_pos.y = value
		"scale":
			_image_scale = value

	value_label.text = _format_tuning_number(value)
	_apply_image_tuning()
	_save_tuning_values()


func _toggle_tuning_panel_visible() -> void:
	_tuning_panel_visible = not _tuning_panel_visible
	mouse_filter = Control.MOUSE_FILTER_PASS if _tuning_panel_visible else Control.MOUSE_FILTER_IGNORE
	clip_contents = not _tuning_panel_visible
	_apply_tuning_panel_layout()
	_save_tuning_values()


func _toggle_tuning_panel_collapsed() -> void:
	if _tuning_body == null:
		return

	_tuning_body.visible = not _tuning_body.visible
	if _collapse_button != null:
		_collapse_button.text = "Collapse" if _tuning_body.visible else "Expand"
	_apply_tuning_panel_layout()


func _current_tuning_panel_height() -> float:
	return 190.0 if _tuning_body == null or _tuning_body.visible else 56.0


func _reset_tuning_values() -> void:
	_image_pos = DEFAULT_IMAGE_POS
	_image_scale = DEFAULT_IMAGE_SCALE
	_set_slider_value("x", _image_pos.x)
	_set_slider_value("y", _image_pos.y)
	_set_slider_value("scale", _image_scale)
	_apply_image_tuning()
	_save_tuning_values()


func _restore_saved_tuning_values() -> void:
	_image_pos = _saved_image_pos
	_image_scale = _saved_image_scale
	_tuning_panel_visible = _saved_tuning_panel_visible
	mouse_filter = Control.MOUSE_FILTER_PASS if _tuning_panel_visible else Control.MOUSE_FILTER_IGNORE
	clip_contents = not _tuning_panel_visible


func _save_tuning_values() -> void:
	_saved_image_pos = _image_pos
	_saved_image_scale = _image_scale
	_saved_tuning_panel_visible = _tuning_panel_visible


func _set_slider_value(key: String, value: float) -> void:
	if _tuning_sliders.has(key):
		_tuning_sliders[key].value = value


func _print_tuning_values() -> void:
	var values := _tuning_values_text()
	DisplayServer.clipboard_set(values)
	print(values)


func _tuning_values_text() -> String:
	return "character_select_bg x=%s y=%s scale=%s width=%s height=%s" % [
		_format_tuning_number(_image_pos.x),
		_format_tuning_number(_image_pos.y),
		_format_tuning_number(_image_scale),
		_format_tuning_number(VIRTUAL_SIZE.x * _image_scale),
		_format_tuning_number(VIRTUAL_SIZE.y * _image_scale),
	]


func _format_tuning_number(value: float) -> String:
	return str(snappedf(value, 0.01))
