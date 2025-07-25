using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string nickname;     // 게임 내 닉네임
    public string playerId;     // firebaseUid
    public int winCount;
    public int loseCount;

    public PlayerData(string nickname, string firebaseUID, int winCount, int loseCount)
    {
        this.nickname = nickname;
        playerId = firebaseUID;
        this.winCount = winCount;
        this.loseCount = loseCount;
    }

    /// <summary>
    /// Firebase에서 불러온 데이터를 바탕으로 PlayerData 생성
    /// </summary>
    /// <param name="nickname"> 플레이어 이름 </param>
    /// <param name="playerId"> 플레이어ID(FirebaseUid) </param>
    /// <param name="winCount"> Firebase에 저장된 winCount 값 </param>
    /// <param name="loseCount"> Firebase에 저장된 loseCount 값 </param>
    /// <returns></returns>
    public static PlayerData CreatePlayerDataFromFirebase(string nickname, string firebaseUID, int winCount, int loseCount)
    {
        return new PlayerData(nickname, firebaseUID, winCount, loseCount);
    }

    public void WinCountUpdate()
    {
        winCount += 1;
    }

    public void LoseCountUpdate()
    {
        loseCount += 1;
    }
}

