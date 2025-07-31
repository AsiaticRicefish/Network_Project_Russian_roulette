using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_ObservableButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _buttonImage;
        
        private bool _interactable = true;
        private Color _originColor;

        public Button button => _button; 
        
        private void Awake()
        {
            if (_buttonImage != null)
                _originColor = _buttonImage.color;
        }
        
        

        public void SetInteractable(bool value)
        {
            if (_interactable != value)
            {
                _interactable = value;
                _button.interactable = value;
                OnInteractableChanged(value);
            }
        }

        private void OnInteractableChanged(bool isInteractable)
        {
            if (_buttonImage != null)
            {
               
                Color color = _originColor;
                if (!_interactable)
                    color.a = 0.3f;
            }
               
            
            if (_text != null)
                _text.alpha = isInteractable ? 1f : 0.3f;
            
        }
        
        
    }
}