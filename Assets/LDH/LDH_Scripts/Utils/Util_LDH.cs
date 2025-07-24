using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Util_LDH
    {

        #region Component

        /// <summary>
        /// GameObject에 해당 컴포넌트가 있으면 반환, 없으면 새로 추가해서 반환
        /// </summary>
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            if (go.TryGetComponent<T>(out T component))
                return component;
            return go.AddComponent<T>();
        }

        #endregion
        
        
        #region Validation
        ///<summary>
        /// index가 리스트/배열 등 순차 컬렉션 내에서 유효한 범위인지 확인
        /// </summary>
        /// <param name="index">검사할 인덱스</param>
        /// <param name="list">인덱스를 가진 컬렉션 (IList)</param>
        /// <returns>list가 null이 아니고, index가 0 이상이고 Count 미만이면 true</returns>
        public static bool IsValidIndex<T>(int index, IList<T> list)
        {
            return list != null && index >= 0 && index < list.Count;
        }

        #endregion


        #region Resource

        public static T Instantiate<T>(string prefabPath, Transform parent = null) where T : UnityEngine.Object
        {
            T prefab = Resources.Load<T>(prefabPath);
            if (prefabPath == null)
            {
                Debug.Log($"[Util_LDH] 프리팹을 가져올 수 없습니다. : {prefabPath}");
                return null;
            }
            
            T go = Object.Instantiate(prefab, parent);
            go.name = prefab.name;

            return go;
        }


        #endregion
    }
    
}