using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace PMS_Test
{
    //이친구를 생성을 하고 
    public class InGamePlayerManager : MonoBehaviourPunCallbacks
    {
        private PhotonView _pv;
        private string key = "GameStart";
        private bool flag = false;

        private void Awake()
        {
            _pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            //룸 프로터피의 값이 true로 바뀔 때 
            /*if (_pv.IsMine && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) &&
            (bool)PhotonNetwork.CurrentRoom.CustomProperties[key] == true) //&& isGameStart = true면 생성하도록 해보게 하기 //여기서 제어 한번 해보자
            {
                CreateController();
            }*/
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (_pv.IsMine && propertiesThatChanged.ContainsKey(key) &&
                (bool)propertiesThatChanged[key] == true && !flag)
            {
                CreateController();
                flag = true; 
            }
        }   

        private void CreateController()
        {
            //Instantiate player controller
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), Vector3.zero, Quaternion.identity);
            Debug.Log("호출");
        }
    }
}
