namespace GameFoundation.Utilities.UIStuff
{
    using UnityEngine.EventSystems;

    public class MechPointerEvent : BaseMechSFX,IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.OnPlaySfx();
        }
    }
}
