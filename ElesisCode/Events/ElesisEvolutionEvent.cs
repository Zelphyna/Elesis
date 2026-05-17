using BaseLib.Abstracts;
using Elesis.ElesisCode.Relics;
using Elesis.ElesisCode.Specializations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Events;

public abstract class ElesisEvolutionEvent : CustomEventModel
{
    protected abstract int TargetTier { get; }

    public override bool IsAllowed(IRunState runState) => false;

    public override string? CustomInitialPortraitPath
    {
        get
        {
            var emblem = GetEmblem();
            return emblem == null ? null : ElesisSpecializationVisuals.EventPortraitPathFor(emblem.Specialization, TargetTier);
        }
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, ConfirmEvolution, $"{Id.Entry}.pages.INITIAL.options.CONFIRM")
        ];
    }

    private Task ConfirmEvolution()
    {
        var emblem = GetEmblem();
        emblem?.UnlockEvolution(TargetTier);

        SetEventFinished(new LocString("events", $"{Id.Entry}.pages.DONE.description"));
        return Task.CompletedTask;
    }

    private BelderKnightEmblem? GetEmblem()
    {
        return Owner?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
    }
}

public sealed class ElesisSecondEvolutionEvent : ElesisEvolutionEvent
{
    protected override int TargetTier => 2;
}

public sealed class ElesisFinalEvolutionEvent : ElesisEvolutionEvent
{
    protected override int TargetTier => 3;
}
