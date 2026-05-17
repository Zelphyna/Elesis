using Elesis.ElesisCode.Events;
using Elesis.ElesisCode.Relics;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Specializations;

public static class ElesisSpecializationController
{
    private const int SpecializationThreshold = 15;
    private const int SecondEvolutionThreshold = 35;
    private const int FinalEvolutionThreshold = 55;

    public static async Task TryOpenPendingProgressionEvent(BelderKnightEmblem emblem)
    {
        var runManager = RunManager.Instance;
        var state = runManager.DebugOnlyGetState();
        if (state?.CurrentRoom is not MapRoom)
        {
            return;
        }

        var player = LocalContext.GetMe(state);
        if (player?.Character.Id.Entry != Character.Elesis.CharacterId)
        {
            return;
        }

        if (player != emblem.Owner)
        {
            return;
        }

        if (emblem.ShouldOpenSpecializationChoice(SpecializationThreshold))
        {
            emblem.SpecializationChoicePending = true;
            await runManager.EnterRoomWithoutExitingCurrentRoom(new EventRoom(ModelDb.Event<ElesisSpecializationEvent>()), fadeToBlack: true);
            return;
        }

        if (emblem.ShouldOpenEvolution(SecondEvolutionThreshold, 2))
        {
            emblem.PendingEvolutionTier = 2;
            await runManager.EnterRoomWithoutExitingCurrentRoom(new EventRoom(ModelDb.Event<ElesisSecondEvolutionEvent>()), fadeToBlack: true);
            return;
        }

        if (emblem.ShouldOpenEvolution(FinalEvolutionThreshold, 3))
        {
            emblem.PendingEvolutionTier = 3;
            await runManager.EnterRoomWithoutExitingCurrentRoom(new EventRoom(ModelDb.Event<ElesisFinalEvolutionEvent>()), fadeToBlack: true);
            return;
        }
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
