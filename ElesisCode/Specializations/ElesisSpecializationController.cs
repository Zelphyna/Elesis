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
    private static bool _isRegistered;

    public static void Register()
    {
        if (_isRegistered)
        {
            return;
        }

        RunManager.Instance.RoomEntered += OnRoomEntered;
        _isRegistered = true;
    }

    private static void OnRoomEntered()
    {
        TaskHelper.RunSafely(OnRoomEnteredAsync());
    }

    private static async Task OnRoomEnteredAsync()
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

        var emblem = player.Relics.OfType<BelderKnightEmblem>().FirstOrDefault();
        if (emblem == null)
        {
            return;
        }

        var completedNodeCount = state.TotalFloor;
        if (completedNodeCount <= 0 || emblem.LastProcessedNodeCount >= completedNodeCount)
        {
            return;
        }

        var historyEntry = state.CurrentMapPointHistoryEntry;
        if (historyEntry == null)
        {
            return;
        }

        emblem.LastProcessedNodeCount = completedNodeCount;
        emblem.GainExperience(ExperienceFor(historyEntry.Rooms.Select(room => room.RoomType)));

        if (emblem.ShouldOpenSpecializationChoice(SpecializationThreshold))
        {
            emblem.SpecializationChoicePending = true;
            await runManager.EnterRoomWithoutExitingCurrentRoom(new EventRoom(ModelDb.Event<ElesisSpecializationEvent>()), fadeToBlack: true);
        }
    }

    private static int ExperienceFor(IEnumerable<RoomType> roomTypes)
    {
        var rooms = roomTypes.ToList();
        if (rooms.Contains(RoomType.Elite))
        {
            return 3;
        }

        if (rooms.Contains(RoomType.Monster))
        {
            return 2;
        }

        return 1;
    }
}
