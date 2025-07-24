using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Util_LDH
    {
        /// <summary>
        /// index가 리스트/배열 등 순차 컬렉션 내에서 유효한 범위인지 확인
        /// </summary>
        /// <param name="index">검사할 인덱스</param>
        /// <param name="list">인덱스를 가진 컬렉션 (IList)</param>
        /// <returns>list가 null이 아니고, index가 0 이상이고 Count 미만이면 true</returns>
        public static bool IsValidIndex<T>(int index, IList<T> list)
        {
            return list != null && index >= 0 && index < list.Count;
        }
    }
}