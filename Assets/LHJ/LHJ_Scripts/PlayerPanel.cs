using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerPanel : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI readyText;
    
    [Header("PlayerImage")]
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _rawImage;
    [SerializeField] private GameObject _waitingText;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private Color _occupiedColor;
    
    
    
    private bool isReady;
    
    public void Start() => Init();
    private void Init()
    {
        Debug.Log("Init 호출.....");
        nicknameText.text = "Waiting...";
        readyText?.gameObject.SetActive(false);
        
        
        if (_rawImage != null)
            _rawImage.gameObject.SetActive(false);
        if (_waitingText != null)
            _waitingText.SetActive(true);
        
        _image.color = _emptyColor;
        
        isReady = false;
    }

    public void Reset()
    {
        Init();
        ReadyPropertyUpdate();
    }
    

    // 플레이어 UI 초기화
    public void SetData(Player player)
    {
        Debug.Log(player.NickName);
        // 닉네임 설정
        nicknameText.text = player.NickName;
        
        // waiting 표시 제거 및 이미지 적용
        if (!player.IsMasterClient)
        {
            _rawImage.gameObject.SetActive(true);
            _waitingText.SetActive(false);
        }
        _image.color = _occupiedColor;
        
        // ready 상태 초기화 및 설정
        isReady = player.IsMasterClient;
        
        
        ReadyPropertyUpdate();
        
    }

    // 준비버튼 클릭 함수
    public void ReadyButtonClick()
    {
        isReady = !isReady;
        readyText?.gameObject.SetActive(isReady);

       
        ReadyPropertyUpdate();
    }
    //준비상태 저장
    public void ReadyPropertyUpdate()
    {
        ExitGames.Client.Photon.Hashtable playerProperty = new Hashtable();
        playerProperty["Ready"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    // 다른 플레이어 준비 상태 체크
    public void ReadyCheck(Player player)
    {
        if (player.CustomProperties.TryGetValue("Ready", out object value))
        {
            readyText?.gameObject.SetActive((bool)value);
        }
    }
}
