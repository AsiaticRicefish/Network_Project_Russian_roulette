using System;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;

//문제점 : 플레이어 프로퍼티가 흩어져있으면 각각 스크립트에 OnPlayerPropertiesUpdate 콜백함수를 override하고 써줘여함.

//장점 :
// 1.코드가 간편해진다(기존의 OnPlayerPropertiesUpdate를 안쓸수 있다), 옵져버 등록,해제만 하면 됨. 

//단점 :
// 1.생성 타이밍이 Room에 들어왔을 때 해당 객체가 생성해야한다. 그렇지 않다면 생성은 가능해도 접근을 막아줘야함. -> Onjoinroom으로 프리팹으로 생성?
// 2.룸을 떠났을 때 객체 파괴 및 Instance를 null처리 <- 완료

public class PlayerPropertyObserver : MonoBehaviourPunCallbacks
{
    private static PlayerPropertyObserver _instance;
    public static PlayerPropertyObserver Instance => _instance;

    //싱글톤으로 제작
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private Dictionary<string, Action<Player, object>> _observers = new();

    public void RegisterObserver(string key, Action<Player, object> callback)
    {
        if (_observers.ContainsKey(key))
            _observers[key] += callback;
        else
            _observers[key] = callback;
    }

    public void UnregisterObserver(string key, Action<Player, object> callback)
    {
        if (_observers.ContainsKey(key))
        {
            _observers[key] -= callback;
            if (_observers[key] == null)
                _observers.Remove(key);
        }
    }

    #region PUN 콜백 함수사용
    //해시 테이블을 한곳에서 관리
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        foreach (var prop in changedProps)
        {
            string key = prop.Key.ToString();
            if (_observers.TryGetValue(key, out var callback))
            {
                callback?.Invoke(targetPlayer, prop.Value);
            }
        }
    }

    //플레이어가 나갈 때 모든 구독 해제 처리
    public override void OnLeftRoom()
    {
        _observers.Clear();
        Destroy(gameObject);
        _instance = null;
    }
    #endregion

    // 플레이어 프로퍼티가 있는지 없는지 - 개인의 프로퍼티만 확인가능
    public object GetPlayerProperty(string key)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(key, out object value))
        {
            return value;
        }
        return null;
    }
}
