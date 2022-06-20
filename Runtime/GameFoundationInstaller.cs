namespace GameFoundation
{
    using DarkTonic.MasterAudio;
    using GameFoundation.AssetLibrary;
    using GameFoundation.BlueprintFlow.BlueprintControlFlow;
    using GameFoundation.GameManager;
    using GameFoundation.Models;
    using GameFoundation.Network;
    using GameFoundation.Network.Authentication;
    using GameFoundation.ScreenFlow.Managers;
    using GameFoundation.Utilities;
    using GameFoundation.Utilities.ApplicationServices;
    using GameFoundation.Utilities.Extension;
    using GameFoundation.Utilities.GameQueueAction;
    using GameFoundation.Utilities.LogService;
    using GameFoundation.Utilities.ObjectPool;
    using I2.Loc;
    using Zenject;

    public class GameFoundationInstaller : Installer<GameFoundationInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(this.Container);

            this.Container.Bind<IGameAssets>().To<GameAssets>().AsCached();
            this.Container.Bind<ObjectPoolManager>().AsCached().NonLazy();

            //CreateMasterAudio
            this.Container.Bind<MasterAudio>().FromComponentInNewPrefabResource("MechMasterAudio").AsCached().NonLazy();
            this.Container.BindInterfacesTo<MasterMechSoundManager>().AsCached().NonLazy();

            //Localization services
            this.Container.Bind<SetLanguage>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
            this.Container.Bind<LocalizationService>().AsCached().NonLazy();

            //Service
            this.Container.Bind<ILogService>().To<LogService>().AsSingle().NonLazy();

            //Game Manager
            this.Container.Bind<HandleLocalDataServices>().AsCached().NonLazy();
            this.Container.Bind<GameFoundationLocalData>().FromResolveGetter<HandleLocalDataServices>(services => services.Load<GameFoundationLocalData>()).AsCached();

            //Player state
            this.Container.Bind<PlayerState>().AsCached();


            //Genarate fps
            this.Container.Bind<Fps>().FromNewComponentOnNewGameObject().AsCached().NonLazy();

            //Installer
            BlueprintServicesInstaller.Install(this.Container);
            NetworkServicesInstaller.Install(this.Container);
            ScreenFlowInstaller.Install(this.Container);
            ServicesLoginInstaller.Install(this.Container);
            ApplicationServiceInstaller.Install(this.Container);
            GameQueueActionInstaller.Install(this.Container);
        }
    }
}