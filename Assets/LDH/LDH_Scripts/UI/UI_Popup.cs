using Managers;
using UnityEngine;

namespace GameUI
{
    public class UI_Popup : UI_Base
    {
        protected override void Init()
        {
            Manager.UI.SetCanvas(gameObject, true);
        }

        public virtual void ClosedPopupUI()
        {
            Manager.UI.ClosePopupUI(this);

        }
    }
}