using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPattern;

public class GameManager : Singleton<GameManager>
{
    // private List<Player> _players
    // private LinkedList<int> _순번리스트
    // 
    private void Awake() => SingletonInit();

    private void StartGame()
    {
        // SceneManager.LoadScene("게임 Scene")
        // StartRound()
    }
    private void EndGame()
    {
        // 각 플레이어의 승패를 통해 결과 저장
        // SceneManager.LoadScene("결과창 Scene")
    }

    private void StartRound()
    {
        // 해당 라운드에 사용할 랜덤 순번 지정
        // StartTurn()
    }
    private void EndRound()
    {

    }

    private void StartTurn()
    {
        // 해당 턴의 플레이어 상태 = 활성
        // 해당하지 않는 플레이어 상태 = 비활성
    }
    private void EndTurn()
    {
        // 해당 턴의 플레이어 상태 = 비활성
        // 
        // if(플레이어들의 체력 == 0)
        //  EndRound()
        //  return
        // 
        // 순번 변경
        // StartRound()
    }

    

}
