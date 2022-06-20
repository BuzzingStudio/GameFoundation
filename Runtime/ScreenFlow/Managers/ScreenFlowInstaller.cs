namespace GameFoundation.ScreenFlow.Managers
{
    using GameFoundation.ScreenFlow.Signals;
    using Zenject;

    public class ScreenFlowInstaller: Installer<ScreenFlowInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<SceneDirector>().AsCached();
            this.Container.BindInterfacesAndSelfTo<ScreenManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            this.Container.DeclareSignal<StartLoadingNewSceneSignal>();
            this.Container.DeclareSignal<FinishLoadingNewSceneSignal>();
            this.Container.DeclareSignal<ScreenCloseSignal>();
            this.Container.DeclareSignal<ScreenShowSignal>();
            this.Container.DeclareSignal<ScreenHideSignal>();
            this.Container.DeclareSignal<ManualInitScreenSignal>();
            this.Container.DeclareSignal<ScreenSelfDestroyedSignal>();
            this.Container.DeclareSignal<PopupShowedSignal>();
            this.Container.DeclareSignal<PopupHiddenSignal>();
            this.Container.DeclareSignal<PopupBlurBgShowedSignal>();
        }
    }
}