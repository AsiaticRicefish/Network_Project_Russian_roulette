using TMPro;
using UnityEngine;

namespace GameUI
{
    /// <summary>
    /// 인벤토리 아이템의 이름 및 설명을 표시하는 팝업 UI
    /// </summary>
    public class UI_InventoryInfo : UI_Popup
    {
        [Header("UI Elements")] [SerializeField]
        private GameObject _panel;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _itemDescription;

        /// <summary>
        /// 팝업 표시. 추가적인 연출이 필요한 경우 오버라이드하여 확장 가능
        /// </summary>
        public override void Show()
        {
            base.Show();
            
            //override 원하는 경우 추가
        }

        /// <summary>
        /// 아이템 이름 및 설명 텍스트 설정
        /// </summary>
        public void SetData(string itemName, string itemDescription)
        {
            _itemName.text = itemName;
            _itemDescription.text = itemDescription;
        }
        
        
        
    }
}