namespace GameFoundation.Utilities.ApplicationServices
{
    using GameFoundation.Utilities.UIStuff;
    using Zenject;

    public class ApplicationServiceInstaller : Installer<ApplicationServiceInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<MinimizeAppService>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            this.Container.DeclareSignal<ApplicationPauseSignal>();
            this.Container.DeclareSignal<UpdateTimeAfterFocusSignal>();

            this.Container.BindIFactory<AutoCooldownTimer>().FromPoolableMemoryPool();
        }
    }
}