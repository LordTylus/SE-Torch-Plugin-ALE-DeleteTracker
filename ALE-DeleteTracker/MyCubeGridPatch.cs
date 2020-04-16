using NLog;
using NLog.Config;
using NLog.Targets;
using Sandbox.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torch.Managers.PatchManager;
using Torch.Utils;
using VRage.Game.Entity;

namespace ALE_DeleteTracker {

    [PatchShim]
    public static class MyCubeGridPatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static readonly Logger FULL_LOGGER = LogManager.GetLogger("DeleteTrackerFull");
        public static readonly Logger SHORT_LOGGER = LogManager.GetLogger("DeleteTrackerBasic");

        [ReflectedMethodInfo(typeof(MyEntity), "Close")]
        private static readonly MethodInfo OnCloseRequest;

        internal static readonly MethodInfo patchOnCloseRequest =
            typeof(MyCubeGridPatch).GetMethod(nameof(OnCloseRequestImpl), BindingFlags.Static | BindingFlags.Public) ??
            throw new Exception("Failed to find patch method");

        static MyCubeGridPatch() {

            var basicLogTarget = new FileTarget {
                FileName = "Logs/deleted-basic-${shortdate}.log",
                Layout = "${var:logStamp} ${var:logContent}"
            };

            var fullLogTarget = new FileTarget {
                FileName = "Logs/deleted-${shortdate}.log",
                Layout = "${var:logStamp} ${var:logContent}"
            };

            LogManager.Configuration.AddTarget("deleter", fullLogTarget);
            LogManager.Configuration.AddTarget("smalldeleter", basicLogTarget);

            var fullRule = new LoggingRule("DeleteTrackerFull", LogLevel.Debug, fullLogTarget) {
                Final = true
            };

            var basicRule = new LoggingRule("DeleteTrackerBasic", LogLevel.Debug, basicLogTarget) {
                Final = true
            };

            LogManager.Configuration.LoggingRules.Insert(0, fullRule);
            LogManager.Configuration.LoggingRules.Insert(0, basicRule);

            LogManager.Configuration.Reload();
        }

        public static void Patch(PatchContext ctx) {

            ctx.GetPattern(OnCloseRequest).Prefixes.Add(patchOnCloseRequest);

            Log.Debug("Patched MyCubeGrid!");
        }

        public static void OnCloseRequestImpl(MyEntity __instance) {

            MyCubeGrid grid = __instance as MyCubeGrid;
            if (grid == null)
                return;

            try {

                /* Never log Projections... */
                if (grid.Physics == null)
                    return;

                DeleteTrackerPlugin plugin = DeleteTrackerPlugin.Instance;
                DeleteConfig config = plugin.Config;

                /* No logging? then skip everything */
                if (!config.EnableFullLogging && !config.EnableCompactLogging)
                    return;

                /* Grid already closed, skip if we dont want duplicates */
                if (grid.MarkedForClose && !config.LogDuplicates)
                    return;

                string stacktrace = Environment.StackTrace;

                /* If we dont want closes on shutdown skip that. We detect it by looking for Unload() Method on MySession */
                if (!config.RemovalOnShutdown)
                    if (stacktrace.Contains("at Sandbox.Game.World.MySession.Unload()"))
                        return;

                var gridOwnerList = grid.BigOwners;
                var ownerCnt = gridOwnerList.Count;
                var gridOwner = 0L;
                var hasNobodyOwner = false;

                if (ownerCnt > 0 && gridOwnerList[0] != 0)
                    gridOwner = gridOwnerList[0];
                else if (ownerCnt > 1) {
                    hasNobodyOwner = true;
                    gridOwner = gridOwnerList[1];
                }

                /* If owned by nobody, and we dont want them, skip */
                if (gridOwner == 0L && !config.RemovalNobodyGrids)
                    return;

                bool isOnline = PlayerUtils.isOnline(gridOwner);
                string onlineString = "[Off]";
                if (isOnline)
                    onlineString = "[On]";

                string ownerName = PlayerUtils.GetPlayerNameById(gridOwner);
                string factionTag = PlayerUtils.GetFactionTagStringForPlayer(gridOwner);

                string playerNameString = ownerName + " " + onlineString + factionTag;
                string messageString;

                string gridId = grid.EntityId.ToString().PadRight(20);

                if (hasNobodyOwner || gridOwner == 0) {

                    if (gridOwner != 0)
                        messageString = "Entity " + gridId + " owned by nobody" + " but has blocks from" + " " + playerNameString.PadRight(25) + " was closed! (Grid: " + grid.DisplayName + ")";
                    else
                        messageString = "Entity " + gridId + " owned by nobody" + "".PadRight(20) + " " + "".PadRight(25) + " was closed! (Grid: " + grid.DisplayName + ")";

                } else {

                    messageString = "Entity " + gridId + " owned by player" + "".PadRight(20) + " " + playerNameString.PadRight(25) + " was closed! (Grid: " + grid.DisplayName + ")";
                }

                if (config.EnableFullLogging)
                    FULL_LOGGER.Info(messageString + "\n" + stacktrace);

                if (config.EnableCompactLogging)
                    SHORT_LOGGER.Info(messageString);

            } catch(Exception e) {
                Log.Error(e, "Error on DeleteTracking Grid!");
            }
        }
    }
}
