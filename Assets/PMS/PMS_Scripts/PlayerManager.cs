using DesignPattern;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
    // TODO - 나중에 Manager 등록하게 해줘야한다.
    private void Awake() => SingletonInit();

    private List<Player> _players;

    //set할 때 자동 동기화 될 수 있도록 하는방법? 
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
