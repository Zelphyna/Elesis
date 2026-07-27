#!/usr/bin/env python3
"""Import the local TDA Empire Sword PMX and prepare it for Godot.

Run this script through Blender, not the system Python. The source archive and
all generated model files remain local because the model rules prohibit
redistributing edited versions.
"""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
from empire_sword_animation import (
    attach_claymore,
    build_animations,
    convert_materials,
    export_glb,
    find_object,
    remove_shape_keys,
    render_previews,
)



def parse_args() -> argparse.Namespace:
    arguments = sys.argv[sys.argv.index("--") + 1 :] if "--" in sys.argv else []
    parser = argparse.ArgumentParser()
    parser.add_argument("--source-dir", type=Path, required=True)
    parser.add_argument("--model-file", default="TDA ES.pmx")
    parser.add_argument("--addon-dir", type=Path, required=True)
    parser.add_argument("--output-glb", type=Path, required=True)
    parser.add_argument("--output-blend", type=Path)
    parser.add_argument("--inspect-json", type=Path)
    parser.add_argument("--inspect-only", action="store_true")
    parser.add_argument("--preview-dir", type=Path)
    return parser.parse_args(arguments)


def enable_mmd_tools(addon_dir: Path) -> None:
    addon_parent = addon_dir.resolve().parent
    opencc_dir = addon_dir / "wheels" / "opencc_unpacked"
    sys.path.insert(0, str(addon_parent))
    if not opencc_dir.is_dir():
        raise FileNotFoundError(f"Extract the bundled OpenCC wheel to {opencc_dir}")
    sys.path.insert(0, str(opencc_dir.resolve()))

    import mmd_tools

    mmd_tools.register()


def reset_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.armatures, bpy.data.materials):
        for datablock in list(datablocks):
            if datablock.users == 0:
                datablocks.remove(datablock)


def import_pmx(path: Path) -> None:
    result = bpy.ops.mmd_tools.import_model(
        filepath=str(path.resolve()),
        types={"MESH", "ARMATURE", "MORPHS"},
        scale=0.08,
        clean_model=True,
        remove_doubles=False,
        fix_bone_order=True,
        fix_ik_links=False,
        apply_bone_fixed_axis=False,
        rename_bones=True,
        dictionary="INTERNAL",
        use_mipmap=True,
        log_level="INFO",
        save_log=False,
    )
    if "FINISHED" not in result:
        raise RuntimeError(f"MMD Tools failed to import {path}: {result}")


def scene_inventory() -> dict[str, object]:
    objects = []
    armatures = []
    for obj in sorted(bpy.context.scene.objects, key=lambda item: item.name):
        objects.append(
            {
                "name": obj.name,
                "type": obj.type,
                "parent": obj.parent.name if obj.parent else None,
                "dimensions": [round(value, 5) for value in obj.dimensions],
                "location": [round(value, 5) for value in obj.location],
                "bounds": [
                    [round(value, 5) for value in corner] for corner in obj.bound_box
                ],
            }
        )
        if obj.type == "ARMATURE":
            armatures.append(
                {
                    "name": obj.name,
                    "bones": [
                        {
                            "name": bone.name,
                            "mmd_name_j": getattr(bone.mmd_bone, "name_j", ""),
                            "mmd_name_e": getattr(bone.mmd_bone, "name_e", ""),
                            "parent": bone.parent.name if bone.parent else None,
                            "head": [round(value, 5) for value in bone.head],
                            "tail": [round(value, 5) for value in bone.tail],
                        }
                        for bone in obj.pose.bones
                    ],
                }
            )
    return {"objects": objects, "armatures": armatures}


def main() -> None:
    args = parse_args()
    source_dir = args.source_dir.resolve()
    model_path = source_dir / args.model_file
    if not model_path.is_file():
        raise FileNotFoundError(f"Missing Empire Sword model: {model_path}")

    enable_mmd_tools(args.addon_dir)
    reset_scene()
    import_pmx(model_path)

    text = ""
    if args.inspect_json or args.inspect_only:
        text = json.dumps(scene_inventory(), ensure_ascii=False, indent=2)
    if args.inspect_json:
        args.inspect_json.parent.mkdir(parents=True, exist_ok=True)
        args.inspect_json.write_text(text + "\n", encoding="utf-8")
    if text:
        print(text)

    if args.inspect_only:
        return

    if args.model_file != "TDA ES.pmx":
        raise ValueError("Full conversion requires the default TDA ES.pmx model")

    character_root = bpy.data.objects.get("TDA ES")
    character_armature = find_object("ARMATURE", "TDA ES_arm")
    character_mesh = find_object("MESH", "TDA ES_mesh")
    if character_root is None:
        raise RuntimeError("MMD Tools did not create the TDA ES root")

    sword_mesh = attach_claymore(source_dir, character_armature, import_pmx)
    remove_shape_keys(character_mesh)
    remove_shape_keys(sword_mesh)
    convert_materials([character_mesh, sword_mesh])
    _, attack = build_animations(character_armature)

    if args.preview_dir:
        render_previews(character_armature, attack, args.preview_dir.resolve())

    if args.output_blend:
        args.output_blend.parent.mkdir(parents=True, exist_ok=True)
        bpy.ops.wm.save_as_mainfile(filepath=str(args.output_blend.resolve()))

    export_glb(
        args.output_glb,
        character_root,
        character_armature,
        [character_mesh, sword_mesh],
    )
    print(f"Exported Godot model to {args.output_glb.resolve()}")


if __name__ == "__main__":
    main()
