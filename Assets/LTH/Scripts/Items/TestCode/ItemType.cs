using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTH
{
    public enum ItemType
    {
      Cigarette,        // 담배 : 플레이어 체력 1 회복
      Cuffs,            // 수갑 : 상대 플레이어 한 턴 건너뛰기
      MagnifyingGlass,  // 돋보기 : 실탄인지 공포탄인지 구분
      Saw,              // 톱 : 총 2배로 데미지 증가
      Cellphone,        // 휴대폰 : 장전된 탄환에 대한 미래 예지
    }
}