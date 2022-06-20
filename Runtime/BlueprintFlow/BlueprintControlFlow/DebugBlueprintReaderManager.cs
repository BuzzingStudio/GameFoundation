namespace GameFoundation.BlueprintFlow.BlueprintControlFlow
{
    using GameFoundation.GameManager;
    using GameFoundation.Network.WebService;
    using GameFoundation.Utilities.LogService;
    using Zenject;

    public class DebugBlueprintReaderManager : BlueprintReaderManager
    {
        public DebugBlueprintReaderManager(SignalBus signalBus, ILogService logService, DiContainer diContainer, GameFoundationLocalData localData, HandleLocalDataServices handleLocalDataServices, IHttpService httpService, BlueprintConfig blueprintConfig) : base(signalBus, logService, diContainer, localData, handleLocalDataServices, httpService, blueprintConfig)
        {
        }

        protected override bool IsLoadLocalBlueprint(string url, string hash) => true;
    }
}