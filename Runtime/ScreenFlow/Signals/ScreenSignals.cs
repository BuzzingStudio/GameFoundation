namespace GameFoundation.ScreenFlow.Signals
{
    using GameFoundation.ScreenFlow.BaseScreen.Presenter;

    public class ScreenCloseSignal
    {
        public IScreenPresenter ScreenPresenter;
    }

    public class ScreenHideSignal
    {
        public IScreenPresenter ScreenPresenter;
    }

    public class ScreenShowSignal
    {
        public IScreenPresenter ScreenPresenter;
    }

    public class ManualInitScreenSignal
    {
        public IScreenPresenter ScreenPresenter;
    }

    public class ScreenSelfDestroyedSignal
    {
        public IScreenPresenter ScreenPresenter;
    }

    public class ForceDestroyScreenSignal
    {
        public IScreenPresenter ScreenPresenter;
    }
}