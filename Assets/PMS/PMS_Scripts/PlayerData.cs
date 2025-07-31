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

    public static PlayerData CreatePlayerDataFromFirebase(string nickname, string firebaseUID, int winCount, int loseCount)
    {
        return new PlayerData(nickname, firebaseUID, winCount, loseCount);
    }

    /*public void WinCountUpdate()
    {
        winCount += 1;
    }

    public void LoseCountUpdate()
    {
        loseCount += 1;
    }*/
}

