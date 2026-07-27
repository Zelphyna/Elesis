extends Sprite2D

const LOCAL_MODEL_PATH := "res://Elesis/assets/third_party/empire_sword_local/empire_sword.glb"
const MODEL_VIEWPORT_SIZE := Vector2i(640, 640)
const MODEL_SPRITE_SCALE := Vector2(0.84, 0.84)
const MODEL_ATTACK_SPEED := 1.65
const ATTACK_WINDUP_SECONDS := 0.10
const ATTACK_SWING_SECONDS := 0.12
const ATTACK_RECOVERY_SECONDS := 0.20
const ATTACK_LUNGE_DISTANCE := 52.0
const ATTACK_WINDUP_ROTATION := -0.045
const ATTACK_SWING_ROTATION := 0.085

var _base_position: Vector2
var _base_scale: Vector2
var _motion_tween: Tween
var _idle_tween: Tween
var _model_viewport: SubViewport
var _model_animation_player: AnimationPlayer
var _using_rigged_model := false


func _ready() -> void:
	_try_enable_local_rigged_model()
	_base_position = position
	_base_scale = scale
	_start_idle_animation()


func _try_enable_local_rigged_model() -> void:
	if not ResourceLoader.exists(LOCAL_MODEL_PATH):
		return

	var model_scene := load(LOCAL_MODEL_PATH) as PackedScene
	if model_scene == null:
		push_warning("Elesis rigged combat model could not be loaded; using the sprite fallback.")
		return

	_model_viewport = SubViewport.new()
	_model_viewport.name = "ElesisModelViewport"
	_model_viewport.size = MODEL_VIEWPORT_SIZE
	_model_viewport.transparent_bg = true
	_model_viewport.render_target_update_mode = SubViewport.UPDATE_ALWAYS
	_model_viewport.msaa_3d = Viewport.MSAA_2X
	add_child(_model_viewport)

	var stage := Node3D.new()
	stage.name = "ElesisModelStage"
	_model_viewport.add_child(stage)

	var model := model_scene.instantiate()
	model.name = "EmpireSwordModel"
	stage.add_child(model)

	var camera := Camera3D.new()
	camera.name = "ElesisModelCamera"
	camera.projection = Camera3D.PROJECTION_ORTHOGONAL
	camera.size = 2.6
	camera.position = Vector3(0.0, 0.72, 3.0)
	stage.add_child(camera)
	camera.look_at_from_position(camera.position, Vector3(0.0, 0.72, 0.0), Vector3.UP)
	camera.current = true

	var key_light := DirectionalLight3D.new()
	key_light.name = "ElesisModelKeyLight"
	key_light.rotation_degrees = Vector3(-35.0, -25.0, 0.0)
	key_light.light_energy = 1.4
	key_light.shadow_enabled = true
	stage.add_child(key_light)

	var fill_light := DirectionalLight3D.new()
	fill_light.name = "ElesisModelFillLight"
	fill_light.rotation_degrees = Vector3(-20.0, 150.0, 0.0)
	fill_light.light_energy = 0.7
	stage.add_child(fill_light)

	var environment := Environment.new()
	environment.background_mode = Environment.BG_COLOR
	environment.background_color = Color.TRANSPARENT
	environment.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	environment.ambient_light_color = Color("d9e2ff")
	environment.ambient_light_energy = 0.8
	var world_environment := WorldEnvironment.new()
	world_environment.name = "ElesisModelEnvironment"
	world_environment.environment = environment
	stage.add_child(world_environment)

	_model_animation_player = _find_animation_player(model)
	if _model_animation_player == null:
		push_warning("Elesis rigged combat model has no AnimationPlayer; using the sprite fallback.")
		_model_viewport.queue_free()
		_model_viewport = null
		return

	var idle_animation := _model_animation_player.get_animation("idle")
	if idle_animation == null or _model_animation_player.get_animation("attack") == null:
		push_warning("Elesis rigged combat model lacks idle/attack clips; using the sprite fallback.")
		_model_viewport.queue_free()
		_model_viewport = null
		_model_animation_player = null
		return

	idle_animation.loop_mode = Animation.LOOP_LINEAR
	_model_animation_player.animation_finished.connect(_on_model_animation_finished)
	_model_animation_player.play("idle")
	texture = _model_viewport.get_texture()
	scale = MODEL_SPRITE_SCALE
	_using_rigged_model = true


func _find_animation_player(node: Node) -> AnimationPlayer:
	if node is AnimationPlayer:
		return node as AnimationPlayer
	for child in node.get_children():
		var found := _find_animation_player(child)
		if found != null:
			return found
	return null


func _on_model_animation_finished(animation_name: StringName) -> void:
	if animation_name == &"attack" and _model_animation_player != null:
		_model_animation_player.play("idle")


func play_combat_animation(trigger: String) -> void:
	if trigger.to_lower() != "attack":
		return

	if _using_rigged_model and _model_animation_player != null:
		_model_animation_player.stop()
		_model_animation_player.play("attack", -1.0, MODEL_ATTACK_SPEED)

	_stop_active_tweens()
	position = _base_position
	rotation = 0.0
	scale = _base_scale

	_motion_tween = create_tween()
	_motion_tween.set_trans(Tween.TRANS_QUAD)
	_motion_tween.set_ease(Tween.EASE_OUT)
	_motion_tween.tween_property(self, "position", _base_position + Vector2(-12.0, 3.0), ATTACK_WINDUP_SECONDS)
	_motion_tween.parallel().tween_property(self, "rotation", ATTACK_WINDUP_ROTATION, ATTACK_WINDUP_SECONDS)
	_motion_tween.tween_property(self, "position", _base_position + Vector2(ATTACK_LUNGE_DISTANCE, -4.0), ATTACK_SWING_SECONDS)
	_motion_tween.parallel().tween_property(self, "rotation", ATTACK_SWING_ROTATION, ATTACK_SWING_SECONDS)
	_motion_tween.parallel().tween_method(_set_attack_scale, 0.98, 1.04, ATTACK_SWING_SECONDS)
	_motion_tween.tween_callback(_spawn_sword_arc)
	_motion_tween.set_trans(Tween.TRANS_BACK)
	_motion_tween.set_ease(Tween.EASE_OUT)
	_motion_tween.tween_property(self, "position", _base_position, ATTACK_RECOVERY_SECONDS)
	_motion_tween.parallel().tween_property(self, "rotation", 0.0, ATTACK_RECOVERY_SECONDS)
	_motion_tween.parallel().tween_property(self, "scale", _base_scale, ATTACK_RECOVERY_SECONDS)
	_motion_tween.tween_callback(_start_idle_animation)


func _set_attack_scale(multiplier: float) -> void:
	scale = _base_scale * multiplier


func _start_idle_animation() -> void:
	if not is_inside_tree():
		return
	if _idle_tween != null and _idle_tween.is_valid():
		_idle_tween.kill()

	position = _base_position
	rotation = 0.0
	scale = _base_scale
	_idle_tween = create_tween().set_loops()
	_idle_tween.set_trans(Tween.TRANS_SINE)
	_idle_tween.set_ease(Tween.EASE_IN_OUT)
	_idle_tween.tween_property(self, "position", _base_position + Vector2(0.0, -3.0), 1.15)
	_idle_tween.parallel().tween_property(self, "scale", _base_scale * Vector2(1.006, 0.994), 1.15)
	_idle_tween.tween_property(self, "position", _base_position, 1.15)
	_idle_tween.parallel().tween_property(self, "scale", _base_scale, 1.15)


func _stop_active_tweens() -> void:
	if _motion_tween != null and _motion_tween.is_valid():
		_motion_tween.kill()
	if _idle_tween != null and _idle_tween.is_valid():
		_idle_tween.kill()


func _spawn_sword_arc() -> void:
	var parent_node := get_parent()
	if parent_node == null:
		return

	var slash := Line2D.new()
	slash.name = "ElesisSwordArc"
	slash.z_index = z_index + 1
	slash.width = 9.0
	slash.default_color = Color(1.0, 0.63, 0.28, 0.92)
	slash.begin_cap_mode = Line2D.LINE_CAP_ROUND
	slash.end_cap_mode = Line2D.LINE_CAP_ROUND
	slash.joint_mode = Line2D.LINE_JOINT_ROUND
	slash.points = PackedVector2Array([
		Vector2(-58.0, -284.0),
		Vector2(2.0, -314.0),
		Vector2(70.0, -290.0),
		Vector2(112.0, -238.0),
	])
	parent_node.add_child(slash)

	var slash_tween := slash.create_tween()
	slash_tween.set_trans(Tween.TRANS_QUAD)
	slash_tween.set_ease(Tween.EASE_OUT)
	slash_tween.tween_property(slash, "modulate:a", 0.0, 0.18)
	slash_tween.parallel().tween_property(slash, "width", 2.0, 0.18)
	slash_tween.tween_callback(slash.queue_free)
