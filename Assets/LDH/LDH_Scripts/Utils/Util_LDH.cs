using Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
        
        public static Component GetOrAddComponent(GameObject go, Type type)
        {
            if (go.TryGetComponent(type, out var component))
                return component;

            return go.AddComponent(type);
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
        
        
        #region RectTransform Control

        /// <summary>
        /// 주어진 RectTransform의 anchor, pivot, anchoredPosition, sizeDelta를 설정합니다.
        /// basePosition은 anchoredPosition의 기준 위치이며, offset이 있다면 추가됩니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="anchorMin">Anchor Min 값</param>
        /// <param name="anchorMax">Anchor Max 값</param>
        /// <param name="pivot">Pivot 기준</param>
        /// <param name="basePosition">기준 위치 (anchoredPosition)</param>
        /// <param name="sizeDelta">UI 크기 (width, height). 생략 시 기존 값 유지</param>
        /// <param name="offset">basePosition에 추가로 더해질 오프셋</param>
        public static void SetRectTransform(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 basePosition,
            Vector2? sizeDelta = null,
            Vector2? offset = null
        )
        {
            if (rect == null) return;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;

            // 위치 = 기준 위치 + 오프셋
            rect.anchoredPosition = basePosition + (offset ?? Vector2.zero);

            if (sizeDelta.HasValue)
                rect.sizeDelta = sizeDelta.Value;
        }

        /// <summary>
        /// 부모 영역 전체를 가득 채우는 Full Stretch UI로 설정합니다.
        /// (anchorMin = (0,0), anchorMax = (1,1), pivot = center)
        /// sizeDelta는 (0,0)으로 설정됩니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="offset">anchoredPosition에 적용할 오프셋</param>
        public static void SetFullStretch(RectTransform rect, Vector2? offset = null)
        {
            SetRectTransform(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero,
                offset);
        }

        /// <summary>
        /// 화면 정중앙 기준으로 위치시키며, 명시한 크기로 설정합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">기준 위치에서의 오프셋</param>
        public static void SetCenter(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 center = new Vector2(0.5f, 0.5f);
            SetRectTransform(rect, center, center, center, Vector2.zero, size, offset);
        }
        
        
        /// <summary>
        /// 오른쪽 위 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">오른쪽 위 기준 위치에서의 오프셋</param>
        public static void SetRightTop(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(1f, 1f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        
        /// <summary>
        /// 오른쪽 아래 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">오른쪽 아래 기준 위치에서의 오프셋</param>
        public static void SetRightBottom(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(1f, 0f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        

        /// <summary>
        /// 왼쪽 위 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">왼쪽 위 기준 위치에서의 오프셋</param>
        public static void SetLeftTop(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(0f, 1f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }

        
        /// <summary>
        /// 왼쪽 아래 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">왼쪽 아래 기준 위치에서의 오프셋</param>
        public static void SetLeftBottom(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(0f, 0f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        
        #endregion

        #region Load Scene

        /// <summary>
        /// 로그인 성공 후 씬 전환 코루틴
        /// </summary>
        public static IEnumerator LoadSceneWithDelay(string sceneName, float delay)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;
            
            // 모달 보여지는 시간 확보
            yield return new WaitForSeconds(delay);
            
            // 씬 전환
            op.allowSceneActivation = true;
        }


        #endregion


        #region Network

        
        /// <summary>
        /// 내부적으로 설정된 고유 닉네임(입력닉네임 + 구분자 + UserId)에서
        /// 사용자가 실제 입력한 닉네임 부분만 추출하여 반환합니다.
        /// </summary>
        /// <param name="player">Photon Player 객체</param>
        /// <returns>사용자가 입력한 닉네임 (UserId 제외)</returns>
        public static string GetUserNickname(Player player)
        {
            return GetUserNickname(player?.NickName);
        }

        /// <summary>
        /// 고유 닉네임 문자열(입력닉네임 + 구분자 + UserId)에서
        /// 사용자가 실제 입력한 닉네임 부분만 추출하여 반환합니다.
        /// </summary>
        /// <param name="rawNickname">고유 닉네임 문자열</param>
        /// <returns>입력한 닉네임 부분 (UserId 제외)</returns>
        public static string GetUserNickname(string rawNickname)
        {
            if (string.IsNullOrEmpty(rawNickname))
                return null;

            int index = rawNickname.IndexOf(Define_LDH.NicknameDelimiter, StringComparison.Ordinal);  // StringComparison.Ordinal : 문화권/로케일 영향을 받지 않고 바이트 값을 기준으로 비교 처리
            return index >= 0 ? rawNickname.Substring(0, index) : rawNickname;
        }


        public static void ClearAllPlayerProperty()
        {
            var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;

            var clearProperties = new ExitGames.Client.Photon.Hashtable();

            foreach (var key in customProperties.Keys)
            {
                clearProperties[key] = null;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(clearProperties);
        }
        #endregion
        
        

        public static IEnumerator ShowBulletCamereaEffect()
        {
            Debug.Log("PushCamera 실행");
            Manager.Camera.PushCamera("BulletDisplay");
            Manager.Sound.PlaySfxByKey("CameraChange");
            Debug.Log("1초 대기 시작");
            yield return new WaitForSeconds(2f);
            Debug.Log("PopCamera 실행");
            Manager.Camera.PopCamera();
            Manager.Sound.PlaySfxByKey("CameraChange");
            
        }
    }
    
}