"""Blender-side assembly, animation, preview, and glTF export for Elesis."""

from __future__ import annotations

import math
from pathlib import Path
from typing import Callable

import bpy
from mathutils import Euler, Matrix, Vector


def find_object(object_type: str, name_prefix: str) -> bpy.types.Object:
    matches = [
        obj
        for obj in bpy.context.scene.objects
        if obj.type == object_type and obj.name.startswith(name_prefix)
    ]
    if len(matches) != 1:
        raise RuntimeError(
            f"Expected one {object_type} starting with {name_prefix!r}, got "
            f"{[obj.name for obj in matches]}"
        )
    return matches[0]


def convert_materials(meshes: list[bpy.types.Object]) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for mesh in meshes:
        mesh.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    result = bpy.ops.mmd_tools.convert_materials(
        use_principled=True,
        clean_nodes=True,
        subsurface=0.0,
    )
    if "FINISHED" not in result:
        raise RuntimeError(f"Material conversion failed: {result}")


def remove_shape_keys(mesh: bpy.types.Object) -> None:
    if mesh.data.shape_keys is not None:
        mesh.shape_key_clear()


def attach_claymore(
    source_dir: Path,
    character_armature: bpy.types.Object,
    import_pmx: Callable[[Path], None],
) -> bpy.types.Object:
    import_pmx(source_dir / "Claymore.pmx")
    sword_root = bpy.data.objects.get("Claymore")
    sword_armature = find_object("ARMATURE", "Claymore_arm")
    sword_mesh = find_object("MESH", "Claymore_mesh")

    wrist = character_armature.data.bones.get("Wrist_R")
    if wrist is None:
        raise RuntimeError("Empire Sword rig has no Wrist_R bone")

    grip = Vector((0.0, 0.0, 0.4993))
    placement = Matrix.Translation(wrist.head_local) @ Matrix.Translation(-grip)
    for vertex in sword_mesh.data.vertices:
        vertex.co = placement @ vertex.co

    for modifier in list(sword_mesh.modifiers):
        sword_mesh.modifiers.remove(modifier)
    for group in list(sword_mesh.vertex_groups):
        sword_mesh.vertex_groups.remove(group)

    sword_mesh.parent = character_armature
    sword_mesh.matrix_parent_inverse = Matrix.Identity(4)
    sword_mesh.matrix_local = Matrix.Identity(4)
    sword_mesh.name = "EmpireSword_Claymore"
    sword_mesh.data.name = "EmpireSword_ClaymoreMesh"
    wrist_group = sword_mesh.vertex_groups.new(name="Wrist_R")
    wrist_group.add(range(len(sword_mesh.data.vertices)), 1.0, "REPLACE")
    modifier = sword_mesh.modifiers.new(name="ElesisArmature", type="ARMATURE")
    modifier.object = character_armature

    for old_object in (sword_armature, sword_root):
        if old_object is not None:
            bpy.data.objects.remove(old_object, do_unlink=True)
    return sword_mesh


def reset_pose(armature: bpy.types.Object) -> None:
    for bone in armature.pose.bones:
        bone.matrix_basis.identity()


def rotate_bone(
    armature: bpy.types.Object,
    bone_name: str,
    rotation_degrees: tuple[float, float, float],
) -> bpy.types.PoseBone:
    bone = armature.pose.bones.get(bone_name)
    if bone is None:
        raise RuntimeError(f"Missing animation bone {bone_name}")
    rotation = Euler(
        tuple(math.radians(value) for value in rotation_degrees), "XYZ"
    ).to_matrix().to_4x4()
    pivot = bone.head.copy()
    bone.matrix = Matrix.Translation(pivot) @ rotation @ Matrix.Translation(-pivot) @ bone.matrix
    return bone


def apply_pose(
    armature: bpy.types.Object,
    rotations: dict[str, tuple[float, float, float]],
    center_height: float = 0.0,
) -> list[bpy.types.PoseBone]:
    reset_pose(armature)
    changed = []
    center = armature.pose.bones.get("Center")
    if center is None:
        raise RuntimeError("Empire Sword rig has no Center bone")
    center.location.z = center_height
    changed.append(center)
    for bone_name, rotation in rotations.items():
        changed.append(rotate_bone(armature, bone_name, rotation))
    bpy.context.view_layer.update()
    return changed


IDLE_POSE = {
    "UpperBody": (0.0, 0.0, -2.0),
    "UpperBody2": (1.5, 0.0, 2.0),
    "Arm_R": (0.0, -58.0, 5.0),
    "Elbow_R": (0.0, -14.0, 0.0),
    "Wrist_R": (0.0, 67.0, 0.0),
    "Arm_L": (0.0, 52.0, -7.0),
    "Elbow_L": (0.0, 12.0, 0.0),
}


def keyframe_pose(
    armature: bpy.types.Object,
    action: bpy.types.Action,
    frame: int,
    rotations: dict[str, tuple[float, float, float]],
    center_height: float = 0.0,
) -> None:
    armature.animation_data.action = None
    bpy.context.scene.frame_set(frame)
    changed = apply_pose(armature, rotations, center_height)
    snapshots = {}
    for bone in changed:
        bone.rotation_mode = "QUATERNION"
        snapshots[bone.name] = (
            bone.location.copy(),
            bone.rotation_quaternion.copy(),
            bone.scale.copy(),
        )

    armature.animation_data.action = action
    for bone in changed:
        location, rotation, scale = snapshots[bone.name]
        bone.location = location
        bone.rotation_quaternion = rotation
        bone.scale = scale
        bone.keyframe_insert(data_path="location", frame=frame, group=bone.name)
        bone.keyframe_insert(data_path="rotation_quaternion", frame=frame, group=bone.name)
        bone.keyframe_insert(data_path="scale", frame=frame, group=bone.name)
    armature.animation_data.action = None


def make_action(
    armature: bpy.types.Object,
    name: str,
    poses: list[tuple[int, dict[str, tuple[float, float, float]], float]],
) -> bpy.types.Action:
    action = bpy.data.actions.new(name=name)
    armature.animation_data_create()
    for frame, rotations, center_height in poses:
        keyframe_pose(armature, action, frame, rotations, center_height)
    action.frame_start = poses[0][0]
    action.frame_end = poses[-1][0]
    if poses[0][1] == poses[-1][1] and poses[0][2] == poses[-1][2]:
        for curve in action.fcurves:
            points = sorted(curve.keyframe_points, key=lambda point: point.co.x)
            if len(points) >= 2:
                points[-1].co.y = points[0].co.y
                points[-1].handle_left.y = points[0].handle_left.y
                points[-1].handle_right.y = points[0].handle_right.y
    for curve in action.fcurves:
        for point in curve.keyframe_points:
            point.interpolation = "BEZIER"
    armature.animation_data.action = None
    return action


def build_animations(armature: bpy.types.Object) -> tuple[bpy.types.Action, bpy.types.Action]:
    idle_high = dict(IDLE_POSE)
    idle_high["UpperBody"] = (-1.0, 0.0, -2.5)
    idle_high["UpperBody2"] = (2.5, 0.0, 2.5)
    idle = make_action(
        armature,
        "idle",
        [(0, IDLE_POSE, 0.0), (30, idle_high, 0.012), (60, IDLE_POSE, 0.0)],
    )

    windup = dict(IDLE_POSE)
    windup.update(
        {
            "UpperBody": (0.0, 8.0, 8.0),
            "UpperBody2": (-4.0, 6.0, 8.0),
            "Arm_R": (-10.0, 48.0, -18.0),
            "Elbow_R": (12.0, -35.0, 8.0),
            "Wrist_R": (15.0, -15.0, 25.0),
            "Arm_L": (0.0, 35.0, 12.0),
            "Elbow_L": (0.0, 25.0, 0.0),
        }
    )
    raised = dict(windup)
    raised.update(
        {
            "UpperBody": (-3.0, 12.0, 12.0),
            "UpperBody2": (-7.0, 10.0, 12.0),
            "Arm_R": (-18.0, 78.0, -25.0),
            "Elbow_R": (18.0, -48.0, 10.0),
            "Wrist_R": (20.0, -35.0, 35.0),
        }
    )
    impact = dict(IDLE_POSE)
    impact.update(
        {
            "UpperBody": (4.0, -10.0, -12.0),
            "UpperBody2": (7.0, -8.0, -15.0),
            "Arm_R": (10.0, -42.0, 22.0),
            "Elbow_R": (-8.0, 18.0, -8.0),
            "Wrist_R": (-12.0, 42.0, -28.0),
            "Arm_L": (0.0, 62.0, -15.0),
            "Elbow_L": (0.0, 8.0, 0.0),
        }
    )
    follow = dict(impact)
    follow.update(
        {
            "UpperBody": (6.0, -6.0, -8.0),
            "UpperBody2": (8.0, -5.0, -10.0),
            "Arm_R": (12.0, -68.0, 28.0),
            "Elbow_R": (-10.0, 25.0, -10.0),
            "Wrist_R": (-15.0, 55.0, -32.0),
        }
    )
    attack = make_action(
        armature,
        "attack",
        [
            (0, IDLE_POSE, 0.0),
            (6, windup, -0.015),
            (12, raised, 0.02),
            (17, impact, -0.035),
            (23, follow, -0.025),
            (34, IDLE_POSE, 0.0),
        ],
    )
    return idle, attack


def point_camera(camera: bpy.types.Object, target: Vector) -> None:
    camera.rotation_euler = (target - camera.location).to_track_quat("-Z", "Y").to_euler()


def render_previews(
    armature: bpy.types.Object,
    attack: bpy.types.Action,
    preview_dir: Path,
) -> None:
    preview_dir.mkdir(parents=True, exist_ok=True)
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE_NEXT"
    scene.render.resolution_x = 512
    scene.render.resolution_y = 512
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.film_transparent = True
    scene.render.image_settings.color_mode = "RGBA"

    camera_data = bpy.data.cameras.new("PreviewCamera")
    camera_data.type = "ORTHO"
    camera_data.ortho_scale = 2.05
    camera = bpy.data.objects.new("PreviewCamera", camera_data)
    scene.collection.objects.link(camera)
    camera.location = Vector((0.0, -5.0, 0.88))
    point_camera(camera, Vector((0.0, 0.0, 0.88)))
    scene.camera = camera

    for name, location, energy, size in (
        ("KeyLight", (-3.0, -4.0, 5.0), 900.0, 4.0),
        ("FillLight", (3.0, -2.0, 2.0), 500.0, 3.0),
    ):
        light_data = bpy.data.lights.new(name, type="AREA")
        light_data.energy = energy
        light_data.shape = "DISK"
        light_data.size = size
        light = bpy.data.objects.new(name, light_data)
        scene.collection.objects.link(light)
        light.location = Vector(location)
        point_camera(light, Vector((0.0, 0.0, 0.9)))

    armature.animation_data.action = attack
    for frame in (1, 6, 12, 17, 23, 34):
        scene.frame_set(frame)
        scene.render.filepath = str((preview_dir / f"attack-{frame:02d}.png").resolve())
        bpy.ops.render.render(write_still=True)
    armature.animation_data.action = None


def export_glb(
    output: Path,
    character_root: bpy.types.Object,
    armature: bpy.types.Object,
    meshes: list[bpy.types.Object],
) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.object.select_all(action="DESELECT")
    for obj in [character_root, armature, *meshes]:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = armature
    result = bpy.ops.export_scene.gltf(
        filepath=str(output.resolve()),
        export_format="GLB",
        use_selection=True,
        export_animations=True,
        export_animation_mode="ACTIONS",
        export_force_sampling=True,
        export_frame_step=1,
        export_skins=True,
        export_all_influences=True,
        export_morph=False,
        export_materials="EXPORT",
        export_image_format="AUTO",
        export_cameras=False,
        export_lights=False,
        export_extras=False,
        export_yup=True,
    )
    if "FINISHED" not in result:
        raise RuntimeError(f"glTF export failed: {result}")
