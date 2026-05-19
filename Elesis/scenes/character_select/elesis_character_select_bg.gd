extends Control

const VIRTUAL_SIZE: Vector2 = Vector2(2564.0, 1204.0)
const TUNING_PANEL_START_VISIBLE: bool = false
const TUNING_PANEL_MARGIN: Vector2 = Vector2(24.0, 24.0)
const DEFAULT_CHARACTER_POS: Vector2 = Vector2(1320.0, 0.0)
const DEFAULT_CHARACTER_SCALE: float = 0.76
const DEFAULT_BACKGROUND_INDEX: int = 0
const DEFAULT_CHARACTER_INDEX: int = 0
const BACKGROUND_OPTIONS: Array[Dictionary] = [
	{
		"label": "Active",
		"texture": preload("res://Elesis/images/charui/elesis_character_select_background.png"),
	},
	{
		"label": "Soft 01",
		"texture": preload("res://Elesis/images/versions/character-select/soft-backgrounds-v1/background-01.png"),
	},
	{
		"label": "Soft 02",
		"texture": preload("res://Elesis/images/versions/character-select/soft-backgrounds-v1/background-02.png"),
	},
	{
		"label": "V4 Set 01",
		"texture": preload("res://Elesis/images/versions/character-select/separated-v4/set-01/background.png"),
	},
	{
		"label": "V4 Set 02",
		"texture": preload("res://Elesis/images/versions/character-select/separated-v4/set-02/background.png"),
	},
]
const CHARACTER_OPTIONS: Array[Dictionary] = [
	{
		"label": "Active",
		"texture": preload("res://Elesis/images/charui/elesis_character_select_elesis.png"),
	},
	{
		"label": "V3 Clean",
		"texture": preload("res://Elesis/images/versions/character-select/separated-v3/poses/elesis-character-select-pose-01.png"),
	},
	{
		"label": "V4 Set 01",
		"texture": preload("res://Elesis/images/versions/character-select/separated-v4/set-01/elesis.png"),
	},
	{
		"label": "V4 Set 02",
		"texture": preload("res://Elesis/images/versions/character-select/separated-v4/set-02/elesis.png"),
	},
]

static var _saved_character_pos: Vector2 = DEFAULT_CHARACTER_POS
static var _saved_character_scale: float = DEFAULT_CHARACTER_SCALE
static var _saved_background_index: int = DEFAULT_BACKGROUND_INDEX
static var _saved_character_index: int = DEFAULT_CHARACTER_INDEX
static var _saved_tuning_panel_visible: bool = TUNING_PANEL_START_VISIBLE

var _canvas: Control
var _background: TextureRect
var _character: TextureRect
var _tuning_panel: PanelContainer
var _tuning_body: VBoxContainer
var _collapse_button: Button
var _background_option_label: Label
var _character_option_label: Label
var _tuning_sliders: Dictionary = {}
var _character_pos: Vector2 = DEFAULT_CHARACTER_POS
var _character_scale: float = DEFAULT_CHARACTER_SCALE
var _background_index: int = DEFAULT_BACKGROUND_INDEX
var _character_index: int = DEFAULT_CHARACTER_INDEX
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
	_background.texture = _current_background_texture()
	_background.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	_background.stretch_mode = TextureRect.STRETCH_SCALE
	_background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_canvas.add_child(_background)
	_apply_background_layout()

	_character = TextureRect.new()
	_character.name = "CharacterImage"
	_character.texture = _current_character_texture()
	_character.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	_character.stretch_mode = TextureRect.STRETCH_SCALE
	_character.mouse_filter = Control.MOUSE_FILTER_IGNORE
	_canvas.add_child(_character)
	_apply_character_tuning()

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


func _apply_background_layout() -> void:
	if _background == null:
		return

	_background.position = Vector2.ZERO
	_background.size = VIRTUAL_SIZE


func _apply_character_tuning() -> void:
	if _character == null:
		return

	var texture_size: Vector2 = _current_character_texture().get_size()
	_character.position = _character_pos
	_character.size = texture_size * _character_scale


func _build_tuning_panel() -> PanelContainer:
	var panel: PanelContainer = PanelContainer.new()
	panel.name = "ElesisImagePlacementTuner"
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

	_tuning_body.add_child(_create_option_selector(
		"Fond",
		_background_index,
		BACKGROUND_OPTIONS.size(),
		_on_previous_background_pressed,
		_on_next_background_pressed
	))
	_background_option_label = _last_option_label(_tuning_body)

	_tuning_body.add_child(_create_option_selector(
		"Elesis",
		_character_index,
		CHARACTER_OPTIONS.size(),
		_on_previous_character_pressed,
		_on_next_character_pressed
	))
	_character_option_label = _last_option_label(_tuning_body)
	_update_option_labels()

	_tuning_body.add_child(_create_tuning_slider("X", "x", -300.0, 2600.0, _character_pos.x, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Y", "y", -700.0, 700.0, _character_pos.y, 1.0))
	_tuning_body.add_child(_create_tuning_slider("Scale", "scale", 0.25, 3.0, _character_scale, 0.01))

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
	var root_transform: Transform2D = get_global_transform_with_canvas()
	var root_scale: Vector2 = root_transform.get_scale()
	if absf(root_scale.x) < 0.001:
		root_scale.x = 1.0
	if absf(root_scale.y) < 0.001:
		root_scale.y = 1.0

	_tuning_panel.custom_minimum_size = panel_size
	_tuning_panel.visible = true
	_tuning_panel.position = root_transform.affine_inverse() * TUNING_PANEL_MARGIN
	_tuning_panel.scale = Vector2(1.0 / absf(root_scale.x), 1.0 / absf(root_scale.y))
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
			_character_pos.x = value
		"y":
			_character_pos.y = value
		"scale":
			_character_scale = value

	value_label.text = _format_tuning_number(value)
	_apply_character_tuning()
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
	return 270.0 if _tuning_body == null or _tuning_body.visible else 56.0


func _reset_tuning_values() -> void:
	_character_pos = DEFAULT_CHARACTER_POS
	_character_scale = DEFAULT_CHARACTER_SCALE
	_set_slider_value("x", _character_pos.x)
	_set_slider_value("y", _character_pos.y)
	_set_slider_value("scale", _character_scale)
	_apply_character_tuning()
	_save_tuning_values()


func _restore_saved_tuning_values() -> void:
	_character_pos = _saved_character_pos
	_character_scale = _saved_character_scale
	_background_index = clampi(_saved_background_index, 0, BACKGROUND_OPTIONS.size() - 1)
	_character_index = clampi(_saved_character_index, 0, CHARACTER_OPTIONS.size() - 1)
	_tuning_panel_visible = _saved_tuning_panel_visible
	mouse_filter = Control.MOUSE_FILTER_PASS if _tuning_panel_visible else Control.MOUSE_FILTER_IGNORE
	clip_contents = not _tuning_panel_visible


func _save_tuning_values() -> void:
	_saved_character_pos = _character_pos
	_saved_character_scale = _character_scale
	_saved_background_index = _background_index
	_saved_character_index = _character_index
	_saved_tuning_panel_visible = _tuning_panel_visible


func _set_slider_value(key: String, value: float) -> void:
	if _tuning_sliders.has(key):
		_tuning_sliders[key].value = value


func _print_tuning_values() -> void:
	var values := _tuning_values_text()
	DisplayServer.clipboard_set(values)
	print(values)


func _tuning_values_text() -> String:
	var character_size: Vector2 = _current_character_texture().get_size() * _character_scale
	return "character_select background=%s character=%s x=%s y=%s scale=%s width=%s height=%s" % [
		_current_background_label(),
		_current_character_label(),
		_format_tuning_number(_character_pos.x),
		_format_tuning_number(_character_pos.y),
		_format_tuning_number(_character_scale),
		_format_tuning_number(character_size.x),
		_format_tuning_number(character_size.y),
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


func _on_previous_background_pressed() -> void:
	_select_background(_background_index - 1)


func _on_next_background_pressed() -> void:
	_select_background(_background_index + 1)


func _on_previous_character_pressed() -> void:
	_select_character(_character_index - 1)


func _on_next_character_pressed() -> void:
	_select_character(_character_index + 1)


func _select_background(index: int) -> void:
	_background_index = posmod(index, BACKGROUND_OPTIONS.size())
	if _background != null:
		_background.texture = _current_background_texture()
	_apply_background_layout()
	_update_option_labels()
	_save_tuning_values()


func _select_character(index: int) -> void:
	_character_index = posmod(index, CHARACTER_OPTIONS.size())
	if _character != null:
		_character.texture = _current_character_texture()
	_apply_character_tuning()
	_update_option_labels()
	_save_tuning_values()


func _update_option_labels() -> void:
	if _background_option_label != null:
		_background_option_label.text = _option_text(_background_index, BACKGROUND_OPTIONS.size(), _current_background_label())
	if _character_option_label != null:
		_character_option_label.text = _option_text(_character_index, CHARACTER_OPTIONS.size(), _current_character_label())


func _option_text(index: int, count: int, label_text: String) -> String:
	return "%s/%s %s" % [str(index + 1), str(count), label_text]


func _current_background_label() -> String:
	return BACKGROUND_OPTIONS[_background_index]["label"] as String


func _current_character_label() -> String:
	return CHARACTER_OPTIONS[_character_index]["label"] as String


func _current_background_texture() -> Texture2D:
	return BACKGROUND_OPTIONS[_background_index]["texture"] as Texture2D


func _current_character_texture() -> Texture2D:
	return CHARACTER_OPTIONS[_character_index]["texture"] as Texture2D
