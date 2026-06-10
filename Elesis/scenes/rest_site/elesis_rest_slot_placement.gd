extends Sprite2D

const TUNING_PANEL_START_VISIBLE: bool = true
const TUNING_PANEL_MARGIN: Vector2 = Vector2(24.0, 24.0)
const DEFAULT_REST_OFFSET := Vector2(0, -125)
const DEFAULT_REST_SCALE := Vector2(0.33, 0.33)
const REST_IMAGE_OPTIONS: Array[Dictionary] = [
	{
		"label": "Scene",
		"texture": null,
	},
	{
		"label": "Base Rest Active",
		"texture": preload("res://Elesis/images/specializations/base/elesis_rest.png"),
	},
	{
		"label": "Base Rest 01 A",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-01/elesis-base-rest-iteration-01-a.png"),
	},
	{
		"label": "Base Rest 01 B",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-01/elesis-base-rest-iteration-01-b.png"),
	},
	{
		"label": "Base Rest 01 C",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-01/elesis-base-rest-iteration-01-c.png"),
	},
	{
		"label": "Base Rest 02 A",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-02/elesis-base-rest-iteration-02-a.png"),
	},
	{
		"label": "Base Rest 02 B",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-02/elesis-base-rest-iteration-02-b.png"),
	},
	{
		"label": "Base Rest 02 C",
		"texture": preload("res://Elesis/images/versions/rest-site/base-iteration-02/elesis-base-rest-iteration-02-c.png"),
	},
]

@export var slot_index_override := 0
@export var rest_offset := DEFAULT_REST_OFFSET
@export var rest_scale := DEFAULT_REST_SCALE
@export var slot_offsets := PackedVector2Array([
	Vector2(0, 0),
	Vector2(-14, 3),
	Vector2(14, 3),
	Vector2(0, 7),
])
@export var slot_rotations := PackedFloat32Array([0.0, -2.0, 2.0, 0.0])

static var _saved_rest_offset: Vector2 = DEFAULT_REST_OFFSET
static var _saved_rest_scale: float = DEFAULT_REST_SCALE.x
static var _saved_rest_image_index: int = 0
static var _saved_tuning_panel_visible: bool = TUNING_PANEL_START_VISIBLE

var _scene_texture: Texture2D
var _scene_rest_offset: Vector2
var _scene_rest_scale: float
var _tuning_layer: CanvasLayer
var _tuning_panel: PanelContainer
var _tuning_body: VBoxContainer
var _collapse_button: Button
var _image_option_label: Label
var _tuning_sliders: Dictionary = {}
var _rest_image_index: int = 0
var _tuning_panel_visible: bool = TUNING_PANEL_START_VISIBLE


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	set_process_input(true)
	set_process_unhandled_input(true)
	_scene_texture = texture
	_scene_rest_offset = offset if rest_offset == DEFAULT_REST_OFFSET else rest_offset
	_scene_rest_scale = scale.x if rest_scale == DEFAULT_REST_SCALE else rest_scale.x
	_restore_saved_tuning_values()
	_tuning_panel_visible = true
	_saved_tuning_panel_visible = true
	centered = true
	_apply_rest_tuning()

	var slot := slot_index_override
	if slot <= 0:
		slot = _infer_multiplayer_slot()

	var slot_index := clampi(slot - 1, 0, slot_offsets.size() - 1)
	position += slot_offsets[slot_index]
	rotation_degrees += slot_rotations[slot_index]
	_tuning_layer = CanvasLayer.new()
	_tuning_layer.name = "ElesisRestSitePlacementTunerLayer"
	_tuning_layer.layer = 100
	add_child(_tuning_layer)
	_tuning_panel = _build_tuning_panel()
	_tuning_panel.visible = _tuning_panel_visible
	_tuning_layer.add_child(_tuning_panel)
	_apply_tuning_panel_layout()


func _input(event: InputEvent) -> void:
	_handle_tuning_shortcut(event)


func _unhandled_input(event: InputEvent) -> void:
	_handle_tuning_shortcut(event)


func _handle_tuning_shortcut(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_F3:
		_toggle_tuning_panel_visible()
		get_viewport().set_input_as_handled()


func _infer_multiplayer_slot() -> int:
	var current: Node = self
	var parent := get_parent()
	while parent != null:
		var parent_name := parent.name.to_lower()
		var child_count := parent.get_child_count()
		if child_count >= 2 and child_count <= 4 and (
			parent_name.contains("rest") or parent_name.contains("character") or parent_name.contains("player")
		):
			return clampi(current.get_index() + 1, 1, 4)

		current = parent
		parent = parent.get_parent()

	return 1


func _apply_rest_tuning() -> void:
	texture = _current_rest_texture()
	offset = rest_offset
	scale = Vector2.ONE * rest_scale.x


func _build_tuning_panel() -> PanelContainer:
	var panel: PanelContainer = PanelContainer.new()
	panel.name = "ElesisRestSitePlacementTuner"
	panel.custom_minimum_size = Vector2(500.0, 270.0)
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
	title.text = "Elesis Rest Placement Tuner"
	title.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	header.add_child(title)

	_collapse_button = Button.new()
	_collapse_button.text = "Collapse"
	_collapse_button.pressed.connect(_toggle_tuning_panel_collapsed)
	header.add_child(_collapse_button)

	_tuning_body = VBoxContainer.new()
	_tuning_body.mouse_filter = Control.MOUSE_FILTER_PASS
	layout.add_child(_tuning_body)

	_tuning_body.add_child(_create_option_selector(
		"Image",
		_rest_image_index,
		REST_IMAGE_OPTIONS.size(),
		_on_previous_image_pressed,
		_on_next_image_pressed
	))
	_image_option_label = _last_option_label(_tuning_body)
	_update_option_labels()

	_tuning_body.add_child(_create_tuning_slider("X", "x", -2600.0, 2600.0, rest_offset.x, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Y", "y", -2600.0, 2600.0, rest_offset.y, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Scale", "scale", 0.01, 1.0, rest_scale.x, 0.001))

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
	_tuning_panel.scale = Vector2.ONE
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
			rest_offset.x = value
		"y":
			rest_offset.y = value
		"scale":
			rest_scale = Vector2.ONE * value

	value_label.text = _format_tuning_number(value)
	_apply_rest_tuning()
	_apply_tuning_panel_layout()
	_save_tuning_values()


func _toggle_tuning_panel_visible() -> void:
	_tuning_panel_visible = not _tuning_panel_visible
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
	return 270.0 if _tuning_body == null or _tuning_body.visible else 56.0


func _reset_tuning_values() -> void:
	rest_offset = _scene_rest_offset
	rest_scale = Vector2.ONE * _scene_rest_scale
	_select_image(0)
	_apply_tuning_panel_layout()
	_set_slider_value("x", rest_offset.x)
	_set_slider_value("y", rest_offset.y)
	_set_slider_value("scale", rest_scale.x)
	_apply_rest_tuning()
	_save_tuning_values()


func _restore_saved_tuning_values() -> void:
	rest_offset = _saved_rest_offset if _saved_rest_offset != DEFAULT_REST_OFFSET else _scene_rest_offset
	rest_scale = Vector2.ONE * (_saved_rest_scale if not is_equal_approx(_saved_rest_scale, DEFAULT_REST_SCALE.x) else _scene_rest_scale)
	_rest_image_index = clampi(_saved_rest_image_index, 0, REST_IMAGE_OPTIONS.size() - 1)
	_tuning_panel_visible = _saved_tuning_panel_visible


func _save_tuning_values() -> void:
	_saved_rest_offset = rest_offset
	_saved_rest_scale = rest_scale.x
	_saved_rest_image_index = _rest_image_index
	_saved_tuning_panel_visible = _tuning_panel_visible


func _set_slider_value(key: String, value: float) -> void:
	if _tuning_sliders.has(key):
		_tuning_sliders[key].value = value


func _print_tuning_values() -> void:
	var values := _tuning_values_text()
	DisplayServer.clipboard_set(values)
	print(values)


func _tuning_values_text() -> String:
	var texture_size: Vector2 = _current_rest_texture().get_size() * rest_scale.x
	return "rest_site image=%s x=%s y=%s scale=%s width=%s height=%s" % [
		_current_rest_label(),
		_format_tuning_number(rest_offset.x),
		_format_tuning_number(rest_offset.y),
		_format_tuning_number(rest_scale.x),
		_format_tuning_number(texture_size.x),
		_format_tuning_number(texture_size.y),
	]


func _format_tuning_number(value: float) -> String:
	return str(snappedf(value, 0.01))


func _create_option_selector(label_text: String, index: int, count: int, previous_callback: Callable, next_callback: Callable) -> HBoxContainer:
	var row: HBoxContainer = HBoxContainer.new()
	row.custom_minimum_size = Vector2(0.0, 34.0)

	var name_label: Label = Label.new()
	name_label.text = label_text
	name_label.custom_minimum_size = Vector2(80.0, 0.0)
	row.add_child(name_label)

	var previous_button: Button = Button.new()
	previous_button.text = "<"
	previous_button.disabled = count <= 1
	previous_button.pressed.connect(previous_callback)
	row.add_child(previous_button)

	var value_label: Label = Label.new()
	value_label.name = "OptionValue"
	value_label.text = _option_text(index, count, "")
	value_label.custom_minimum_size = Vector2(260.0, 0.0)
	value_label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	row.add_child(value_label)

	var next_button: Button = Button.new()
	next_button.text = ">"
	next_button.disabled = count <= 1
	next_button.pressed.connect(next_callback)
	row.add_child(next_button)

	return row


func _last_option_label(parent: Control) -> Label:
	var row: Node = parent.get_child(parent.get_child_count() - 1)
	return row.get_node("OptionValue") as Label


func _on_previous_image_pressed() -> void:
	_select_image(_rest_image_index - 1)


func _on_next_image_pressed() -> void:
	_select_image(_rest_image_index + 1)


func _select_image(index: int) -> void:
	_rest_image_index = posmod(index, REST_IMAGE_OPTIONS.size())
	_apply_rest_tuning()
	_apply_tuning_panel_layout()
	_update_option_labels()
	_save_tuning_values()


func _update_option_labels() -> void:
	if _image_option_label != null:
		_image_option_label.text = _option_text(_rest_image_index, REST_IMAGE_OPTIONS.size(), _current_rest_label())


func _option_text(index: int, count: int, label_text: String) -> String:
	return "%s/%s %s" % [str(index + 1), str(count), label_text]


func _current_rest_label() -> String:
	return REST_IMAGE_OPTIONS[_rest_image_index]["label"] as String


func _current_rest_texture() -> Texture2D:
	if _rest_image_index == 0:
		return _scene_texture
	return REST_IMAGE_OPTIONS[_rest_image_index]["texture"] as Texture2D
