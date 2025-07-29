using Test;
using UnityEngine;

namespace Managers
{
    /// ===================================================
    /// 역할
    /// - 프로젝트 내 Manager 클래스들을 중앙에서 관리
    /// - @Manager 프리팹 하나만으로 모든 매니저 컴포넌트 로드 및 DontDestroyOnLoad
    ///   - static 프로퍼티로 각 매니저 접근 가능
    ///
    /// 
    /// 매니저 등록 방법
    /// 1. Manager.cs에 static 프로퍼티 추가
    ///     public static TestManager Test => TestManager.Instance;
    /// 2.각 매니저 클래스는 Singleton<T> 상속 + Awake에서 SingletonInit() 호출해주기
    ///     public class TestManager : Singleton<TestManager>
    ///     {
    ///         private void Awake() => SingletonInit();
    ///     }
    ///
    /// 3. 해당 매니저 컴포넌트는 @Manager 프리팹에 다음 중 하나의 방식으로 등록한다 :
    ///     - 프리팹 에디터에서 직접 컴포넌트를 추가 (프리팹 위치 : Assets/Resources/Prefabs/@Manager)
    ///     - 또는 Manager.cs의 Initialize() 내에서 manager.AddComponent<T>로 동적으로 추가
    ///
    /// 사용 예시
    /// Manager.Test.DebugTest();
    /// 
    /// 주의사항
    /// - @Manager 오브젝트는 게임 실행 시 자동으로 초기화되기 때문에 씬에 배치할 필요가 없다.
    /// 
    /// ===================================================
    
    public static class Manager
    {
        public static GameObject manager;
        
        
        //---- 접근용 프로퍼티 등록 ----- //
        //예시) TestManager
        public static TestManager Test => TestManager.Instance;

        public static SoundManager Sound => SoundManager.Instance;      // 사운드
      
        public static UIManager UI => UIManager.Instance;               //  UI

        public static GunManager Gun => GunManager.Instance;            // 총 매니저

        public static PlayerManager PlayerManager => PlayerManager.Instance;   // 플레이어 매니저

        public static InGameManager Game => InGameManager.Instance;         // 게임 매니저

        public static ItemBoxSpawnerManager ItemBoxSpawner => ItemBoxSpawnerManager.Instance; // 아이템 박스 스폰 매니저
        public static ItemDatabaseManager ItemDatabase => ItemDatabaseManager.Instance; // 아이템 데이터베이스 매니저
        public static ItemManager Item => ItemManager.Instance; // 아이템 매니저
        public static DeskUIManager DeskUI => DeskUIManager.Instance; // 아이템 슬롯 UI 매니저
        public static ItemSyncManager ItemSync => ItemSyncManager.Instance; // 아이템 싱크 매니저

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/@Manager");
            manager = GameObject.Instantiate(prefab);
            manager.gameObject.name = "@Manager";
            GameObject.DontDestroyOnLoad(manager);
                    
            //각각의 매니저 스크립트를 프리팹에 스크립트를 직접 추가해두거나 아래와 같이 AddComponent로 동적으로 추가한다.
            manager.AddComponent<TestManager>();
            manager.AddComponent<SoundManager>();
            manager.AddComponent<PlayerManager>();
            manager.AddComponent<UIManager>();
            manager.AddComponent<ItemSyncManager>();
        }
    }
}