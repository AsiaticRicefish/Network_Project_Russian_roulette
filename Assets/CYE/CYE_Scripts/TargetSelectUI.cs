using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 총 발사 타겟 지정 UI(동기화가 필요없으므로 DeskUI를 상속받지 않음)
/// </summary>
public class TargetSelectUI : MonoBehaviour
{
    [SerializeField] private List<Transform> _playerSelectBtnPos;
    [SerializeField] private GameObject _targetBtnPrefab;
    private Button _btnComponent;

    void Awake() => Init();
    void Init()
    {
        _btnComponent = GetComponent<Button>();
        _btnComponent.onClick.AddListener(SelectTarget);
    }

    public void AddTargetUserBtn(string userId)
    {
        // userId(photon id, nickname 등등)로 유저 정보 가져옴
        // 해당 유저의 방향으로 버튼 생성(_playerSelectBtnPos에서 위치 가져와서 _targetBtnPrefab Initialize)
        // 생성된 버튼과 유저 매핑(id 연결)
    }

    public void SelectTarget()
    {
        // 클릭된 버튼과 연결된 유저 id를 가져옴
        // Fire(target Id)
    }
}
