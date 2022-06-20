namespace GameFoundation.ScreenFlow.Signals
{
    using GameFoundation.ScreenFlow.BaseScreen.Presenter;

    public class PopupShowedSignal
    {
        public IScreenPresenter ScreenPresenter;
    }
    
    public class PopupHiddenSignal
    {
        public IScreenPresenter ScreenPresenter;
    }
    
    public class PopupBlurBgShowedSignal
    {
    }
}