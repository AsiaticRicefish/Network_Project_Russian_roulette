using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerId;
    public string firebaseUid;
    public int winCount;
    public int loseCount;
    public int maxHp;
    public int currentHp;
    public bool isAlive;

    private PlayerData(string id, string uid, int winCount = 0, int loseCount = 0, int maxHp = 3)
    {
        playerId = id;
        firebaseUid = uid;
        this.winCount = winCount;
        this.loseCount = loseCount;
        this.maxHp = maxHp;
        this.currentHp = maxHp;
        this.isAlive = true;
    }

    /// <summary>
    /// Firebase���� �ҷ��� �����͸� �������� PlayerData ����
    /// </summary>
    /// <param name="playerId"> �÷��̾�ID </param>
    /// <param name="firebaseUid"> FirebaseUid </param>
    /// <param name="winCount"> Firebase�� ����� winCount �� </param>
    /// <param name="loseCount"> Firebase�� ����� loseCount �� </param>
    /// <returns></returns>
    public static PlayerData CreatePlayerDataFromFirebase(string playerId, string firebaseUid, int winCount, int loseCount)
    {
        return new PlayerData(playerId, firebaseUid, winCount, loseCount);
    }
}
