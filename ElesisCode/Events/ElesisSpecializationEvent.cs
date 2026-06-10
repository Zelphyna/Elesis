using BaseLib.Abstracts;
using Elesis.ElesisCode.Relics;
using Elesis.ElesisCode.Specializations;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Events;

public sealed class ElesisSpecializationEvent : CustomEventModel
{
    private const string InitialPageKey = "INITIAL";
    private const string PreviewPageKey = "PREVIEW";
    private const string BaseEventPortraitPath = $"{MainFile.ResPath}/images/specializations/base/elesis_shop.png";

    private ElesisSpecialization _pendingSpecialization = ElesisSpecialization.None;

    public override bool IsAllowed(IRunState runState) => false;

    public override string? CustomInitialPortraitPath => CurrentPortraitPath;

    public LocString CurrentPageDescription => new("events", $"{Id.Entry}.pages.{CurrentPageKey}.description");

    private string CurrentPageKey => _pendingSpecialization == ElesisSpecialization.None ? InitialPageKey : PreviewPageKey;

    private string? CurrentPortraitPath => _pendingSpecialization == ElesisSpecialization.None
        ? BaseEventPortraitPath
        : ElesisSpecializationVisuals.EventPortraitPathFor(_pendingSpecialization, 1);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        _pendingSpecialization = ElesisSpecialization.None;
        return InitialOptions();
    }

    private IReadOnlyList<EventOption> InitialOptions()
    {
        var id = Id.Entry;
        return
        [
            new EventOption(this, PreviewSaberKnight, $"{id}.pages.INITIAL.options.SABER_KNIGHT"),
            new EventOption(this, PreviewPyroKnight, $"{id}.pages.INITIAL.options.PYRO_KNIGHT"),
            new EventOption(this, PreviewDarkKnight, $"{id}.pages.INITIAL.options.DARK_KNIGHT"),
            new EventOption(this, PreviewSoarKnight, $"{id}.pages.INITIAL.options.SOAR_KNIGHT")
        ];
    }

    private IReadOnlyList<EventOption> PreviewOptions()
    {
        var id = Id.Entry;
        return
        [
            new EventOption(this, Confirm, $"{id}.pages.PREVIEW.options.CONFIRM"),
            new EventOption(this, CancelPreview, $"{id}.pages.PREVIEW.options.CANCEL")
        ];
    }

    private Task PreviewSaberKnight() => Preview(ElesisSpecialization.SaberKnight);

    private Task PreviewPyroKnight() => Preview(ElesisSpecialization.PyroKnight);

    private Task PreviewDarkKnight() => Preview(ElesisSpecialization.DarkKnight);

    private Task PreviewSoarKnight() => Preview(ElesisSpecialization.SoarKnight);

    private Task Preview(ElesisSpecialization specialization)
    {
        _pendingSpecialization = specialization;
        SetCurrentDescription();
        ReplaceCurrentOptions(PreviewOptions());
        return Task.CompletedTask;
    }

    private Task CancelPreview()
    {
        _pendingSpecialization = ElesisSpecialization.None;
        SetCurrentDescription();
        ReplaceCurrentOptions(InitialOptions());
        return Task.CompletedTask;
    }

    private void SetCurrentDescription()
    {
        AccessTools.PropertySetter(typeof(EventModel), nameof(Description))?.Invoke(this, [CurrentPageDescription]);
    }

    private Task Confirm()
    {
        if (_pendingSpecialization == ElesisSpecialization.None)
        {
            return CancelPreview();
        }

        var emblem = Owner?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        if (emblem != null)
        {
            emblem.SelectSpecialization(_pendingSpecialization);
        }

        SetEventFinished(new LocString("events", $"{Id.Entry}.pages.DONE.description"));
        return Task.CompletedTask;
    }

    private void ReplaceCurrentOptions(IReadOnlyList<EventOption> options)
    {
        ClearCurrentOptions();
        if (CurrentOptions is List<EventOption> currentOptions)
        {
            currentOptions.AddRange(options);
            return;
        }

        MainFile.Logger.Error("Elesis specialization event could not replace current options; STS2 EventModel CurrentOptions no longer exposes its backing list.");
    }
}

[HarmonyPatch(typeof(NEventRoom), "RefreshEventState")]
public static class ElesisSpecializationEventRoomPatch
{
    private const float PortraitScale = 0.7f;
    private static readonly Vector2 PortraitOffset = new(260.0f, 0.0f);

    public static void Postfix(NEventRoom __instance, EventModel eventModel)
    {
        if (eventModel is not ElesisSpecializationEvent specializationEvent)
        {
            return;
        }

        __instance.SetPortrait(specializationEvent.CreateInitialPortrait());
        AdjustPortrait(__instance);
        AccessTools.Method(typeof(NEventRoom), "SetDescription")?.Invoke(__instance, [specializationEvent.CurrentPageDescription]);
    }

    private static void AdjustPortrait(NEventRoom eventRoom)
    {
        if (AccessTools.Field(typeof(NEventLayout), "_portrait")?.GetValue(eventRoom.Layout) is not TextureRect portrait)
        {
            return;
        }

        portrait.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        portrait.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        portrait.Scale = Vector2.One * PortraitScale;
        portrait.Position = PortraitOffset;
    }
}
