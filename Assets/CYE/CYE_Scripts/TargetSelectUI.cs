using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 총 발사 타겟 지정 UI(동기화가 필요없으므로 DeskUI를 상속받지 않음)
/// </summary>
public class TargetSelectUI : MonoBehaviour
{
    private const int MAX_USER_COUNT = 4;

    [SerializeField] private Transform[] _playerSelectBtnPos = new Transform[MAX_USER_COUNT];
    [SerializeField] private GameObject _targetBtnPrefab;

    void Start() => Init();
    void Init()
    {
        // _btnComponent = GetComponent<Button>();
        // _btnComponent.onClick.AddListener(SelectTarget);
        GamePlayer[] gamePlayers = SearchPlayerObjects();
        
        foreach (GamePlayer temp in gamePlayers)
        {
            Debug.Log($"{temp.gameObject.name}");
        }
    }

    public void AddTargetUserBtn(int userIndex, string userId)
    {
        GameObject selectButton = Instantiate(_targetBtnPrefab, _playerSelectBtnPos[userIndex]);
        selectButton.transform.parent = transform;
        if (selectButton.GetComponent<TargetSelectButton>() != null)
        {
            selectButton.GetComponent<TargetSelectButton>().UserId = userId;
        }
    }

    private GamePlayer[] SearchPlayerObjects()
    {
        return GameObject.FindObjectsByType<GamePlayer>(FindObjectsSortMode.None);
    }
}
