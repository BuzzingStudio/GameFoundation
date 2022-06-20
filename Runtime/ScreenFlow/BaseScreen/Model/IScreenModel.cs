namespace GameFoundation.ScreenFlow.BaseScreen.Model
{
    using GameFoundation.MVP;

    /// <summary>
    /// The traditional Model:
    /// - Holds no view data nor view state data.
    /// - Is accessed by the Presenter and other Models only
    /// - Will trigger events to notify external system of changes.
    /// </summary>
    public interface IScreenModel : IUIModel
    {
        
    }
}