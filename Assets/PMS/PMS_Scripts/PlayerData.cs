using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string nickname;     // 게임 내 닉네임
    public string playerId;
    public string firebaseUid;
    public int winCount;
    public int loseCount;
    public int maxHp;
    public int currentHp;
    public bool isAlive;
    public PlayerData(string nickname, string id, string uid, int winCount, int loseCount)
    {
        this.nickname = nickname;
        playerId = id;
        firebaseUid = uid;
        this.winCount = winCount;
        this.loseCount = loseCount;
        this.maxHp = 3;
        this.currentHp = maxHp;
        this.isAlive = true;
    }

    /*public PlayerData(string id, string uid, int winCount, int loseCount, int maxHp,int currentHp,bool isAlive)
    {
        playerId = id;
        firebaseUid = uid;
        this.winCount = winCount;
        this.loseCount = loseCount;
        this.maxHp = maxHp;
        this.currentHp = currentHp;
        this.isAlive = isAlive;
    }*/

    /// <summary>
    /// Firebase에서 불러온 데이터를 바탕으로 PlayerData 생성
    /// </summary>
    /// <param name="nickname"> 플레이어 이름 </param>
    /// <param name="playerId"> 플레이어ID </param>
    /// <param name="firebaseUid"> FirebaseUid </param>
    /// <param name="winCount"> Firebase에 저장된 winCount 값 </param>
    /// <param name="loseCount"> Firebase에 저장된 loseCount 값 </param>
    /// <returns></returns>
    public static PlayerData CreatePlayerDataFromFirebase(string nickname,string playerId, string firebaseUid, int winCount, int loseCount)
    {
        return new PlayerData(nickname, playerId, firebaseUid, winCount, loseCount);
    }
}
