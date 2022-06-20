namespace GameFoundation.Utilities.Extension
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameFoundation.MVP;
    using GameFoundation.ScreenFlow.BaseScreen.View;
    using GameFoundation.Utilities.UIStuff;
    using Newtonsoft.Json;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    //<summary>
    //Manager all extension method
    //</summary>
    public static class ExtensionMethod
    {
        //Remove all Button Listener On View
        public static void OnRemoveButtonListener(this MonoBehaviour view)
        {
            var buttons = view.GetComponentsInChildren<Button>();
            foreach (var b in buttons)
            {
                b.onClick.RemoveAllListeners();
            }
        }

        //check Object trigger With other object
        public static bool CheckObjectOnBound(this BaseView view, Bounds bounds, Bounds g) { return bounds.Intersects(g); }

        public static void InstantiateUIPresenter<TPresenter, TView, TModel>(this IInstantiator instantiator, ref TPresenter presenter, TView view, TModel model)
            where TPresenter : IUIItemPresenter<TView, TModel> where TView : IUIView
        {
            if (presenter == null)
            {
                presenter = instantiator.Instantiate<TPresenter>();
                presenter.SetView(view);
            }

            presenter.BindData(model);
        }

        public static async Task<TPresenter> InstantiateUIPresenter<TPresenter, TModel>(this IInstantiator instantiator, Transform parentView, TModel model)
            where TPresenter : IUIItemPresenter<IUIView, TModel>
        {
            var presenter = instantiator.Instantiate<TPresenter>();
            await presenter.SetView(parentView);
            presenter.BindData(model);
            return presenter;
        }

        //FillChild Width with parent Width
        public static void FillChildWidthWithParentWidth(this IUIPresenter presenter, RectTransform childRect, RectTransform parentRect)
        {
            var v = childRect.sizeDelta;
            v.x                 = parentRect.rect.width;
            childRect.sizeDelta = v;
        }


        public static async void Add<TPresenter, TModel>(this List<TPresenter> listPresenter, TPresenter presenter, Transform parentView, TModel model)
            where TPresenter : IUIItemPresenter<IUIView, TModel>
        {
            await presenter.SetView(parentView);
            presenter.BindData(model);
            listPresenter.Add(presenter);
        }

        public static string ToJson<T>(this T obj) { return JsonConvert.SerializeObject(obj); }

        public static string GetPath(this Transform current)
        {
            if (current.parent == null)
                return current.name;
            return current.parent.GetPath() + "/" + current.name;
        }

        public static string Path(this Component component) { return GetPath(component.transform); }

        public static string Path(this GameObject gameObject) { return GetPath(gameObject.transform); }

        public static Vector2 AsUnityVector2(this System.Numerics.Vector2 v) { return new Vector2(v.X, v.Y); }

        public static Vector3 AsUnityVector3(this System.Numerics.Vector3 v) { return new Vector3(v.X, v.Y, v.Z); }


        //mechText extension
        public static void SetTextLocalization(this TextMeshProUGUI t, string key, Color color = default)
        {
            var mechTextMeshPro = t.GetComponent<MechTextMeshPro>();
            if (mechTextMeshPro == null)
            {
                Debug.Log($"{t.gameObject.name} have no MechTextPro");
                return;
            }

            mechTextMeshPro.SetTextWithLocalization(key, color);
        }
        public static void SetTextLocalization(this TMP_InputField t, string key)
        {
            var mechTextMeshPro = t.textComponent.GetComponent<MechTextMeshPro>();
            if (mechTextMeshPro == null)
            {
                return;
            }

            mechTextMeshPro.SetTextWithLocalization(key);
        }
    }
}