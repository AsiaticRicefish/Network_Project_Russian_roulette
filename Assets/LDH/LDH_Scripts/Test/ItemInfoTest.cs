using GameUI;
using Managers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Test
{
    public class ItemInfoTest : MonoBehaviour
    {
        [SerializeField] private GameObject _itemObject;
        [SerializeField] private string _itemName;
        [SerializeField] private string _itemDescription;

        private void OnMouseEnter()
        {
            Debug.Log("Mouse Hovered!");
            ShowInfo();
        }

        private void OnMouseExit()
        {
            Debug.Log("Mouse Left!");
            HideInfo();
        }

        public void ShowInfo()
        {
            Debug.Log("aafasfasfaf");
            //ui manager로 부터 UI_InventoryInfo 전역 ui를 받아온다.
            var inventoryInfoUI = Manager.UI.GetGlobalUI<UI_InventoryInfo>();
            
            // ui data를 변경해주고
            inventoryInfoUI.SetData(_itemName, _itemDescription);
            
            // show를 호출해서 나타나게 한다.
            Manager.UI.ShowGlobalUI(Define_LDH.GlobalUI.UI_InventoryInfo);
        }
        

        public void HideInfo()
        {
            Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_InventoryInfo);
        }
        
    }
}