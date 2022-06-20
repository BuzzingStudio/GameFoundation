namespace GameFoundation.BlueprintFlow.BlueprintControlFlow
{
    using GameFoundation.BlueprintFlow.BlueprintReader;
    using GameFoundation.BlueprintFlow.Signals;
    using GameFoundation.Utilities.Extension;
    using Zenject;

    /// <summary>
    /// Binding all services of the blueprint control flow at here
    /// </summary>
    public class BlueprintServicesInstaller : Installer<BlueprintServicesInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<BlueprintDownloader>().WhenInjectedInto<BlueprintReaderManager>();
            this.Container.Bind<BlueprintReaderManager>().AsCached();
            this.Container.Bind<BlueprintConfig>().AsCached();

            this.Container.BindAllTypeDriveFrom<IGenericBlueprintReader>();

            this.Container.DeclareSignal<LoadBlueprintDataSuccessedSignal>();
            this.Container.DeclareSignal<LoadBlueprintDataProgressSignal>();
        }
    }
}