extends Sprite2D

const DEFAULT_REST_OFFSET := Vector2(0, -125)
const DEFAULT_REST_SCALE := Vector2(0.33, 0.33)

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


func _ready() -> void:
	centered = true
	offset = rest_offset
	scale = rest_scale

	var slot := slot_index_override
	if slot <= 0:
		slot = _infer_multiplayer_slot()

	var slot_index := clampi(slot - 1, 0, slot_offsets.size() - 1)
	position += slot_offsets[slot_index]
	rotation_degrees += slot_rotations[slot_index]


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
