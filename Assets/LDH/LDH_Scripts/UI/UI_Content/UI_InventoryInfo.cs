using TMPro;
using UnityEngine;

namespace GameUI
{
    public class UI_InventoryInfo : UI_Popup
    {
        [Header("UI Element")] [SerializeField]
        private GameObject _panel;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _itemDescription;


        public override void Show()
        {
            base.Show();
            
            //override 원하는 경우 추가
        }


        public void SetData(string itemName, string itemDescription)
        {
            _itemName.text = itemName;
            _itemDescription.text = itemDescription;
        }
        
        
        
    }
}