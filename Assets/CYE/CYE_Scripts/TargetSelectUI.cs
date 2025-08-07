using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _targetSpawnPoint = new GameObject[2];
    [SerializeField] private GameObject[] _targetButtonArray = new GameObject[2];
    // private IEnumerator Start() {
    //     yield return new WaitUntil(() => GameObject.FindObjectsByType<GamePlayer>(FindObjectsSortMode.None).Length == _targetSpawnPoint.Length);
    //     Init();
    // }
    private void OnEnable()
    {
        InitializeNameTag();
    }

    public void InitializeNameTag()
    {
        // Debug.Log($"[TargetSelectUI] {GameObject.FindObjectsByType<GamePlayer>(FindObjectsSortMode.None).Length}");
        // foreach (GamePlayer player in GameObject.FindObjectsByType<GamePlayer>(FindObjectsSortMode.None))
        // {
        //     Debug.Log($"[TargetSelectUI] {_targetSpawnPoint[0].transform.position} / {player.gameObject.transform.position}");
        //     if (Vector3.Distance(_targetSpawnPoint[0].transform.position, player.gameObject.transform.position) <= 0.001)
        //     {
        //         _targetButtonArray[0].GetComponent<TargetSelectButton>()?.SetTargetId(player.PlayerId, player.Nickname);
        //     }
        //     else
        //     {
        //         _targetButtonArray[1].GetComponent<TargetSelectButton>()?.SetTargetId(player.PlayerId, player.Nickname);
        //     }
        // }
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (p == PhotonNetwork.LocalPlayer)
            {
                _targetButtonArray[1].GetComponent<TargetSelectButton>()?.SetTargetId(p.UserId, p.NickName);
            }
            else
            { 
                _targetButtonArray[0].GetComponent<TargetSelectButton>()?.SetTargetId(p.UserId, p.NickName);
            }
        }
    }
}
