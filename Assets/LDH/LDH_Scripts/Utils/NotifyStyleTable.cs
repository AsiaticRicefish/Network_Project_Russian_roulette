using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(menuName = "UI/Notify Style Table", fileName = "NotifyStyleTable")]
    public class NotifyStyleTable : ScriptableObject
    {
        [System.Serializable]
        public class NotifyStyle
        {
            public Define_LDH.NotifyType notifyType;
            public Sprite icon;
            public Color backgroundColor;
            public Color textColor;
            public AudioClip sfx;
        }
        
        
        [SerializeField] private List<NotifyStyle> styles;

        
        private Dictionary<Define_LDH.NotifyType, NotifyStyle> _lookup;

        public void Init()
        {
            if (_lookup != null) return;
            _lookup = new();
            foreach (var style in styles)
                _lookup[style.notifyType] = style;
        }

        public NotifyStyle GetStyle(Define_LDH.NotifyType type)
        {
            Init();
            return _lookup.TryGetValue(type, out var style) ? style : null;
        }
        
        
    }
}