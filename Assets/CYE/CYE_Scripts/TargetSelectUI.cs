using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _targetSpawnPoint = new GameObject[2];
    [SerializeField] private GameObject[] _targetButtonArray = new GameObject[2];
    void Awake() => Init();
    private void Init()
    {
        Managers.Manager.Game.OnGameStart += InitializeNameTag;
    }

    public void InitializeNameTag()
    {
        foreach (GamePlayer player in GameObject.FindObjectsOfType<GamePlayer>())
        {
            if (Vector3.Distance(_targetSpawnPoint[0].transform.position, player.gameObject.transform.position) <= 0.001)
            {
                _targetButtonArray[0].GetComponent<TargetSelectButton>()?.SetTargetId(player.PlayerId, player.Nickname);
            }
            else
            { 
                _targetButtonArray[1].GetComponent<TargetSelectButton>()?.SetTargetId(player.PlayerId, player.Nickname);
            }
        }
    }
}
