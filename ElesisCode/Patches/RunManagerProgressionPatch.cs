using Elesis.ElesisCode.Specializations;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;

namespace Elesis.ElesisCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.ProceedFromTerminalRewardsScreen))]
public static class RunManagerProgressionPatch
{
    public static void Postfix(Task __result)
    {
        TaskHelper.RunSafely(CheckElesisProgressionAfterRewards(__result));
    }

    private static async Task CheckElesisProgressionAfterRewards(Task proceedTask)
    {
        await proceedTask;
        MainFile.Logger.Info("Elesis progression check after terminal rewards screen.");
        await ElesisSpecializationController.ProcessCurrentMapEntry(requireMapRoom: false);
    }
}
