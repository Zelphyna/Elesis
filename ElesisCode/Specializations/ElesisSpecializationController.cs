using Elesis.ElesisCode.Events;
using Elesis.ElesisCode.Relics;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Specializations;

public static class ElesisSpecializationController
{
    private const int SpecializationThreshold = 15;
    private const int SecondEvolutionThreshold = 35;
    private const int FinalEvolutionThreshold = 55;
    private static bool _isRegistered;
    private static bool _isOpeningProgressionEvent;

    public static void Register()
    {
        if (_isRegistered)
        {
            return;
        }

        RunManager.Instance.RoomEntered += OnRoomEntered;
        RunManager.Instance.ActEntered += OnActEntered;
        _isRegistered = true;
    }

    private static void OnRoomEntered()
    {
        TaskHelper.RunSafely(ProcessCurrentMapEntry());
    }

    private static void OnActEntered()
    {
        TaskHelper.RunSafely(ProcessCurrentMapEntry(requireMapRoom: false));
    }

    public static async Task ProcessCurrentMapEntry(BelderKnightEmblem? emblem = null, bool requireMapRoom = true)
    {
        emblem ??= CurrentElesisEmblem(requireMapRoom);
        if (emblem != null)
        {
            ProcessMapExperienceFallback(emblem);
            await TryOpenPendingProgressionEvent(emblem, requireMapRoom);
        }
    }

    public static async Task TryOpenPendingProgressionEvent(BelderKnightEmblem emblem, bool requireMapRoom = true)
    {
        if (_isOpeningProgressionEvent)
        {
            MainFile.Logger.Info($"Elesis progression check skipped: progression event already opening. xp={emblem.Experience}");
            return;
        }

        var runManager = RunManager.Instance;
        var state = runManager.DebugOnlyGetState();
        if (state == null)
        {
            return;
        }

        if (requireMapRoom && state.CurrentRoom is not MapRoom)
        {
            MainFile.Logger.Info($"Elesis progression check deferred: current room is {state.CurrentRoom?.GetType().Name ?? "null"}, xp={emblem.Experience}");
            return;
        }

        var player = LocalContext.GetMe(state);
        if (player == null)
        {
            MainFile.Logger.Info("Elesis progression check skipped: current player is null.");
            return;
        }

        if (!player.Relics.OfType<BelderKnightEmblem>().Contains(emblem))
        {
            MainFile.Logger.Info($"Elesis progression check skipped: current player {player.Character.Id.Entry} does not own this Belder Knight Emblem.");
            return;
        }

        if (emblem.ShouldOpenSpecializationChoice(SpecializationThreshold))
        {
            MainFile.Logger.Info($"Opening Elesis specialization event. xp={emblem.Experience} threshold={SpecializationThreshold}");
            await OpenProgressionEvent(
                () => emblem.SpecializationChoicePending = true,
                () => emblem.SpecializationChoicePending = false,
                new EventRoom(ModelDb.Event<ElesisSpecializationEvent>()));
            return;
        }

        if (emblem.ShouldOpenEvolution(SecondEvolutionThreshold, 2))
        {
            MainFile.Logger.Info($"Opening Elesis second evolution event. xp={emblem.Experience} threshold={SecondEvolutionThreshold} specialization={emblem.Specialization}");
            await OpenProgressionEvent(
                () => emblem.PendingEvolutionTier = 2,
                () => emblem.PendingEvolutionTier = 0,
                new EventRoom(ModelDb.Event<ElesisSecondEvolutionEvent>()));
            return;
        }

        if (emblem.ShouldOpenEvolution(FinalEvolutionThreshold, 3))
        {
            MainFile.Logger.Info($"Opening Elesis final evolution event. xp={emblem.Experience} threshold={FinalEvolutionThreshold} specialization={emblem.Specialization}");
            await OpenProgressionEvent(
                () => emblem.PendingEvolutionTier = 3,
                () => emblem.PendingEvolutionTier = 0,
                new EventRoom(ModelDb.Event<ElesisFinalEvolutionEvent>()));
            return;
        }
    }

    private static async Task OpenProgressionEvent(Action markPending, Action clearPending, EventRoom room)
    {
        _isOpeningProgressionEvent = true;
        markPending();

        try
        {
            await RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(room, fadeToBlack: true);
        }
        catch
        {
            clearPending();
            throw;
        }
        finally
        {
            _isOpeningProgressionEvent = false;
        }
    }

    private static void ProcessMapExperienceFallback(BelderKnightEmblem emblem)
    {
        var state = RunManager.Instance.DebugOnlyGetState();
        if (state?.CurrentRoom is not MapRoom)
        {
            return;
        }

        var completedNodeCount = state.TotalFloor;
        if (completedNodeCount <= 0 || emblem.LastExperienceAwardedNodeCount >= completedNodeCount)
        {
            return;
        }

        var historyEntry = state.CurrentMapPointHistoryEntry;
        if (historyEntry == null)
        {
            MainFile.Logger.Info($"Elesis map XP check skipped: no map history entry. floor={completedNodeCount} xp={emblem.Experience}");
            return;
        }

        var roomTypes = historyEntry.Rooms.Select(room => room.RoomType).ToList();
        var amount = ExperienceFor(roomTypes);
        if (amount <= 0)
        {
            emblem.LastExperienceAwardedNodeCount = completedNodeCount;
            return;
        }

        if (emblem.CombatExperienceClaimedAwaitingMap)
        {
            MainFile.Logger.Info($"Elesis combat XP reward already claimed. floor={completedNodeCount} amount={amount} totalXp={emblem.Experience}");
        }
        else
        {
            MainFile.Logger.Info($"Elesis combat XP reward was not claimed before map return; awarding fallback XP. floor={completedNodeCount} amount={amount} previousXp={emblem.Experience}");
            emblem.GainExperience(amount);
        }

        emblem.CombatExperienceClaimedAwaitingMap = false;
        emblem.LastExperienceAwardedNodeCount = completedNodeCount;
    }

    private static BelderKnightEmblem? CurrentElesisEmblem(bool requireMapRoom = true)
    {
        var state = RunManager.Instance.DebugOnlyGetState();
        if (state == null || (requireMapRoom && state.CurrentRoom is not MapRoom))
        {
            return null;
        }

        var player = LocalContext.GetMe(state);
        return player?.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
    }

    public static int ExperienceFor(IEnumerable<RoomType> roomTypes)
    {
        var rooms = roomTypes.ToList();
        if (rooms.Contains(RoomType.Boss))
        {
            return 6;
        }

        if (rooms.Contains(RoomType.Elite))
        {
            return 4;
        }

        if (rooms.Contains(RoomType.Monster))
        {
            return 3;
        }

        return 0;
    }

    public static int ExperienceFor(RoomType roomType)
    {
        return ExperienceFor([roomType]);
    }

    public static bool IsCombatExperienceNode(IEnumerable<RoomType> roomTypes)
    {
        var rooms = roomTypes.ToList();
        return rooms.Contains(RoomType.Monster) || rooms.Contains(RoomType.Elite) || rooms.Contains(RoomType.Boss);
    }

    public static bool IsCombatExperienceNode(RoomType roomType)
    {
        return roomType is RoomType.Monster or RoomType.Elite or RoomType.Boss;
    }
}
