using BaseLib.Abstracts;
using Elesis.ElesisCode.Relics;
using Elesis.ElesisCode.Specializations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Events;

public sealed class ElesisSpecializationEvent : CustomEventModel
{
    public override bool IsAllowed(IRunState runState) => false;

    public override string? CustomInitialPortraitPath => $"{MainFile.ResPath}/images/events/elesis_specialization_choice_portrait.png";

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var id = Id.Entry;
        return
        [
            new EventOption(this, () => Choose(ElesisSpecialization.SaberKnight), $"{id}.pages.INITIAL.options.SABER_KNIGHT"),
            new EventOption(this, () => Choose(ElesisSpecialization.PyroKnight), $"{id}.pages.INITIAL.options.PYRO_KNIGHT"),
            new EventOption(this, () => Choose(ElesisSpecialization.DarkKnight), $"{id}.pages.INITIAL.options.DARK_KNIGHT"),
            new EventOption(this, () => Choose(ElesisSpecialization.SoarKnight), $"{id}.pages.INITIAL.options.SOAR_KNIGHT")
        ];
    }

    private Task Choose(ElesisSpecialization specialization)
    {
        var emblem = Owner?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        if (emblem != null)
        {
            emblem.SelectSpecialization(specialization);
        }

        SetEventFinished(new LocString("events", $"{Id.Entry}.pages.DONE.description"));
        return Task.CompletedTask;
    }
}
