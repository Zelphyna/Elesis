using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Elesis.ElesisCode.Patches;

/// <summary>
/// Bridges STS2 combat triggers to Elesis' Godot-native sprite animation.
/// Vanilla only creates a CreatureAnimator when the scene's Visuals node is a SpineSprite.
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class ElesisCombatAnimationPatch
{
    public static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Entity.Player?.Character is not global::Elesis.ElesisCode.Character.Elesis)
        {
            return;
        }

        Node2D body = __instance.Body;
        if (body.HasMethod("play_combat_animation"))
        {
            body.Call("play_combat_animation", trigger);
        }
    }
}
