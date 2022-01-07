### Introduction
At times you wonder if and why a grid vanished from your server. This basically is what this plugin allows you to track.

The Original DeleteTracker was created by rexxar for patreons only. Since that plugin only had basic functionality I asked for permission to create my own DeleteTracker and plublish it for everyone to have.

**There may be conflicts if you are using both this and rexxars DeleteTracker so make sure to remove the other one first.**

### What can it do?

Every time the server calls close() to a Grid it becomes subject to deletion at the end of the update cycle. This plugin takes advantage of that and records which Grid (ID and Name) will be deleted and who the owner & faction of that grid was. 

Also there is an Information. if the Owner was online at that time or not. 

### What are the Settings?

Currently there are 5 Options:

- Enable compact logging
 - Only logs the event itself. So which grid was deleted.
- Enable full logging
 -  Has the compact logging + the stacktrace of how it came to that deletion. 
 - Full Logging is useful for developers if you are familiar on how the game works to extract some information from it. 
 - Non developers can also use it, but probably have to ask on discord for help.
 - The main advantage is that if for example a mod deleted a grid it will be visible in that Stacktrace. 
- Log removal of grids on shutdown
 - Keen always closes every grid when the session unloads. So there may be a lot of false positives in the log. 
 - By default this plugin wont log events caused by a server shutdown. To prevent any confusion
- Log removal of nobody owned grids
 - Grids that are fully owned by nobody can spam the log pretty bad. Especially if you have things like random encounters, or cargo drops enabled. 
 - So there is an option to just disable them. It of course may also ignore a nobody grid you are interested in, so the option is enabled by default. 
- Log duplicates of a grid thats already scheduled for delete
 - Similar to what the name suggest. depending on the case the game may mark a grid for deleting multiple times in an update cycle. 
 - That means it may show up multiple times in the log. However since thats confusing there is an option to just ignore all subsequent delete requests in the logs. Which is the default. 

### How to set it up? 

The easiest part is the installation: Just add the Plugin ID found in your adresss bar to your torch.cfg file, If you download it with the server directly you wont have to do that of course.

The ID is: 2ea955b8-6dec-4f9b-8662-25f6d568addc

DeleteTracker has its own Log-Files. called deleted-&lt;Year&gt;-&lt;Month&gt;-&lt;Day&gt;.log and deleted-basic-&lt;Year&gt;-&lt;Month&gt;-&lt;Day&gt;.log and it wont output on the console or torch.log. Both console should not be spammed with unimportant stuff as it makes finding problems harder. At the same time it would be easier for you to look one logfile up instead of scrolling through an infinitely long torch.log

### How does a log look like?

The Basic Log looks like this:

- 21:02:15.4368 [INFO]   Entity 77998473415124400    owned by player                     LordTylus [On][ALE]       was closed! (Grid: Aky G-Sentry 01)

Basically It says Entity with ID; owned by player LordTylus (which is online and belongs to the ALE Faction) was marked for close. The Grid name is Aky G-Sentry 01.

The Full-Log adds:

-   at System.Environment.GetStackTrace(Exception e, Boolean needFileInfo)
-   at System.Environment.get_StackTrace()
-   at ALE_DeleteTracker.MyCubeGridPatch.OnCloseRequestImpl(MyEntity __instance)
-   at Patched_VRage.Game.Entity.MyEntityClose_0(Object )
-   at Sandbox.Game.Entities.MyCubeGrid.OnGridClosedRequest(Int64 entityId)
-   at VRage.Network.CallSite`7.Invoke(BitStream stream, Object obj, Boolean validate)
-   at VRage.Network.MyReplicationLayer.Invoke(CallSite callSite, BitStream stream, Object obj, EndpointId source, MyClientStateBase clientState, Boolean validate)
-   at VRage.Network.MyReplicationServer.OnEvent(MyPacketDataBitStreamBase data, CallSite site, Object obj, IMyNetObject sendAs, Nullable`1 position, EndpointId source)
-  at VRage.Network.MyReplicationLayer.OnEvent(MyPacketDataBitStreamBase data, NetworkId networkId, NetworkId blockedNetId, UInt32 eventId, EndpointId sender, Nullable`1 position)
-   at VRage.Network.MyReplicationLayer.ProcessEvent(MyPacketDataBitStreamBase data, EndpointId sender)
-   at VRage.Network.MyReplicationLayer.OnEvent(MyPacket packet)
-   at Sandbox.Engine.Multiplayer.MyTransportLayer.ProcessMessage(MyPacket p)
-   at Sandbox.Engine.Multiplayer.MyTransportLayer.HandleMessage(MyPacket p)
-   at Sandbox.Engine.Networking.MyReceiveQueue.Process(NetworkMessageDelegate handler)
-   at Sandbox.Engine.Networking.MyNetworkReader.Process()
-   at Sandbox.MySandboxGame.Update()
-   at Sandbox.Engine.Platform.Game.UpdateInternal()
-   at Patched_Sandbox.Engine.Platform.GameRunSingleFrame_0(Object )
-   at Sandbox.Engine.Platform.FixedLoop.&lt;&gt;c__DisplayClass11_0.&lt;Run&gt;b__0()
-   at Sandbox.Engine.Platform.GenericLoop.Run(VoidAction tickCallback)
-   at Sandbox.Engine.Platform.Game.RunLoop()
-   at Sandbox.MySandboxGame.Run(Boolean customRenderLoop, Action disposeSplashScreen)
-   at Torch.VRageGame.DoStart() in C:\jenkins\workspace\Torch_Torch_master\Torch\VRageGame.cs:line 241
-   at Torch.VRageGame.Run() in C:\jenkins\workspace\Torch_Torch_master\Torch\VRageGame.cs:line 117
-   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
-   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
-   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
-   at System.Threading.ThreadHelper.ThreadStart()

Which adds information on how exactly the deletion was reached. In this case MyReplicationServer.OnEvent shows that a player asked to delete it, probably using ctrl+x in creative mode. (Which btw I did :-))

### Github
[Find it here](https://github.com/LordTylus/SE-Torch-Plugin-ALE-DeleteTracker)
