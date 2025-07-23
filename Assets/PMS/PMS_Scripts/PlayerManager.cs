using DesignPattern;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
    // TODO - ���߿� Manager ����ϰ� ������Ѵ�.
    private void Awake() => SingletonInit();

    private List<Player> _players;

    //set�� �� �ڵ� ����ȭ �� �� �ֵ��� �ϴ¹��? 
    //public List<Player> Players { get { return _players} set { } };

    public void AddPlayer(Player player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        if (_players.Contains(player))
        {
            _players.Remove(player);
        }
    }
}
