using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayPoint
{
    // private string _playerId;
    // public string PlayerId { get { return _playerId; } }
    private int _winCount;
    public int WinCount { get { return _winCount; } }
    private int _loseCount;
    public int LoseCount { get { return _loseCount; } }

    public PlayPoint()//(string playerId)
    {
        // _playerId = playerId;
        _winCount = 0;
        _loseCount = 0;
    }
    public static byte[] Serialize(object customObject)
    {
        PlayPoint playerPointPair = (PlayPoint)customObject;

        MemoryStream memory = new MemoryStream(sizeof(int) + sizeof(int));
        memory.Write(BitConverter.GetBytes(playerPointPair._winCount), 0, sizeof(int));
        memory.Write(BitConverter.GetBytes(playerPointPair._loseCount), 0, sizeof(int));

        return memory.ToArray();
    }
    public static object Deserialize(byte[] bytes)
    {
        PlayPoint playerPointPair = new();

        playerPointPair._winCount = BitConverter.ToInt32(bytes, 0);
        playerPointPair._loseCount = BitConverter.ToInt32(bytes, sizeof(int));

        return playerPointPair;
    }
    public void IncreaseWinCount()
    {
        _winCount++;
    }
    public void IncreaseLoseCount()
    {
        _loseCount++;
    }
}
