using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;
using Utils;
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
    
    
    private Player _player;
    
    private void Init()
    {
        nicknameText.text = "Waiting...";
        readyText?.gameObject.SetActive(false);
        
        
        if (_rawImage != null)
            _rawImage.gameObject.SetActive(false);
        if (_waitingText != null)
            _waitingText.SetActive(true);
        
        _image.color = _emptyColor;
        
    }

    public void Reset()
    {
        Init();
    }
    

    // 플레이어 UI 초기화
    public void SetData(Player player)
    {
        _player = player;
        
        // 닉네임 설정
        nicknameText.text = Util_LDH.GetUserNickname(player);
        
        // waiting 표시 제거 및 이미지 적용
        if (!player.IsMasterClient)
        {
            _rawImage.gameObject.SetActive(true);
            _waitingText.SetActive(false);
        }
        else
        {
            if (player.IsLocal)
            {
                ReadyPropertyUpdate(true);
            }
        }
        _image.color = _occupiedColor;
        
        
        ReadyCheck(player);
        
        
    }

    // 준비버튼 클릭 함수
    public void ReadyButtonClick()
    {
        if (_player != null && _player.IsLocal)
        {
            bool isReady = _player.CustomProperties.TryGetValue("Ready", out object value) && (bool)value;
            ReadyPropertyUpdate(!isReady);
        }
    }
    //준비상태 저장
    public void ReadyPropertyUpdate(bool value)
    {
        Hashtable playerProperty = new Hashtable
        {
            { "Ready", value }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    // 다른 플레이어 준비 상태 체크
    public void ReadyCheck(Player player)
    {
        if (player == null) return;

        if (player.CustomProperties.TryGetValue("Ready", out object value))
        {
            readyText?.gameObject.SetActive((bool)value);
        }
    }
}
