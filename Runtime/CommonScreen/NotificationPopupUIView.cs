namespace GameFoundation.CommonScreen
{
    using System;
    using GameFoundation.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.ScreenFlow.BaseScreen.View;
    using GameFoundation.Utilities;
    using GameFoundation.Utilities.Extension;
    using GameFoundation.Utilities.LogService;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public enum NotificationType
    {
        Close,
        Option,
    }

    public class NotificationPopupUIView : BaseView
    {
        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TextMeshProUGUI txtContent;
        [SerializeField] private Button          btnOk;
        [SerializeField] private Button          btnOkNotice;
        [SerializeField] private Button          btnCancel;
        [SerializeField] private GameObject      noticeObj;
        [SerializeField] private GameObject      closeObj;

        public TextMeshProUGUI TxtTitle    => this.txtTitle;
        public TextMeshProUGUI TxtContent  => this.txtContent;
        public Button          BtnOk       => this.btnOk;
        public Button          BtnOkNotice => this.btnOkNotice;
        public Button          BtnCancel   => this.btnCancel;
        public GameObject      NoticeObj   => this.noticeObj;
        public GameObject      CloseObj    => this.closeObj;
    }

    [PopupInfo("UIPopupNotice", isEnableBlur: true, isCloseWhenTapOutside: false, isOverlay: true)]
    public class NotificationPopupPresenter : BasePopupPresenter<NotificationPopupUIView, NotificationPopupModel>
    {
        private readonly IMechSoundManager mechSoundManager;
        public NotificationPopupPresenter(SignalBus signalBus, ILogService logService, IMechSoundManager mechSoundManager) : base(signalBus, logService) { this.mechSoundManager = mechSoundManager; }

        public override void BindData(NotificationPopupModel popupPopupModel)
        {
            this.Init();
            this.SetNotificationContent();
            this.SwitchMode();
        }

        private void Init()
        {
            this.View.BtnOk.onClick.AddListener(this.OkAction);
            this.View.BtnOkNotice.onClick.AddListener(this.OkNoticeAction);
            this.View.BtnCancel.onClick.AddListener(this.CloseView);
        }

        private void SwitchMode()
        {
            this.View.NoticeObj.SetActive(this.Model.Type == NotificationType.Option);
            this.View.CloseObj.SetActive(this.Model.Type == NotificationType.Close);
        }

        public override void CloseView()
        {
            this.mechSoundManager.PlaySound("button_click");
            base.CloseView();
            this.Model.CloseAction?.Invoke();
            this.Model.CancelAction?.Invoke();
        }

        private void OkAction()
        {
            this.mechSoundManager.PlaySound("button_click");
            this.CloseView();
            this.Model.OkAction?.Invoke();
        }
        
        private void OkNoticeAction()
        {
            this.mechSoundManager.PlaySound("button_click");
            this.CloseView();
            this.Model.OkNoticeAction?.Invoke();
        }

        private void SetNotificationContent()
        {
            this.View.TxtTitle.SetTextLocalization(this.Model.Title);
            this.View.TxtContent.SetTextLocalization(this.Model.Content);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.View.BtnOk.onClick.RemoveListener(this.OkAction);
            this.View.BtnCancel.onClick.RemoveListener(this.CloseView);
        }
    }

    public class NotificationPopupModel
    {
        public string           Title;
        public string           Content;
        public NotificationType Type;

        public Action OkAction       { get; set; }
        public Action OkNoticeAction { get; set; }
        public Action CancelAction   { get; set; }
        public Action CloseAction    { get; set; }
    }
}