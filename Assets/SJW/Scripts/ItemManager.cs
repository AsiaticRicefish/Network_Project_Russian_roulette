using DesignPattern;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LTH;
using Managers;

public class ItemManager : Singleton<ItemManager>
{
    public void UseItem(ItemData item, GamePlayer user, GamePlayer target)
    {
        Debug.Log($"[UseItem 디버그] item: {(item == null ? "null" : item.itemId)}, isUsed: {item?.isUsed}");

        if (item == null || item.isUsed)
        {
            Debug.LogWarning("아이템이 null이거나 이미 사용됨");
            return;
        }
        
        //사운드 재생
        Manager.Sound.PlaySfxByKey("ItemUse");
        
        ApplyEffect(item.itemType, user, target);
        SetItemUsed(item);
    }

    public void ApplyEffect(ItemType itemType, GamePlayer user, GamePlayer target)
    {
        Debug.Log($"[ApplyEffect 디버그] 전달된 itemType: {itemType}");
        switch (itemType)
        {
            case ItemType.Cigarette:
                user.GetComponent<PhotonView>().RPC("RPC_InCreasePlayerCurrentHp", RpcTarget.MasterClient, user.PlayerId, 1);
                Debug.Log("담배 사용 → 피1 회복");
                break;

            case ItemType.Saw:
                Debug.Log("[아이템] Saw 진입");

                //if (GunManager.Instance == null)
                //{
                //    Debug.LogError("GunManager.Instance가 null");
                //    break;
                //}
                //if (GunManager.Instance.PV == null)
                //{
                //    Debug.LogError("GunManager.Instance.PV가 null");
                //    break;
                //}

                GunManager.Instance.PV.RPC("RPC_SetEnhanced", RpcTarget.All, true);
                Debug.Log("톱 사용 → 이번턴 총기 데미지 2배");
                break;

            case ItemType.Cuffs:
                target.IsCuffedThisTurn = true;
                Debug.Log("수갑 사용 → 대상의 다음 턴 스킵 예약됨");
                break;

            case ItemType.Dial:
                GunManager.Instance.PV.RPC("RPC_SwitchNextBullet", RpcTarget.All);
                Debug.Log("다이얼 사용 → 현재 탄 타입 변환");
                break;

            case ItemType.Cellphone:
                if (user.GetComponent<PhotonView>().IsMine)
                {
                    Queue<BulletType> magazine = GunManager.Instance.Magazine;
                    int magCount = magazine.Count;

                    if (magCount == 0)
                    {
                        Debug.Log("[휴대폰] 탄창이 비어있음");
                        break;
                    }

                    int randomIndex = UnityEngine.Random.Range(0, magCount);
                    BulletType[] magazineArray = magazine.ToArray();
                    BulletType randomBullet = magazineArray[randomIndex];

                    Debug.Log($"[휴대폰] ({randomIndex + 1}번째 탄): {randomBullet}");

                }
                else
                {
                    Debug.Log("[휴대폰] 상대가 사용함 (내가 아닌 플레이어)");
                }
                break;

            case ItemType.MagnifyingGlass:
                if (user.GetComponent<PhotonView>().IsMine)
                {
                    BulletType loadedBullet = GunManager.Instance.LoadedBullet;
                    Debug.Log($"[돋보기] 현재 장전된 탄: {loadedBullet}");

                    MagnifyingGlassUI ui = FindObjectOfType<MagnifyingGlassUI>();
                    if (ui != null)
                    {
                        ui.ShowBulletInfo(loadedBullet);
                    }
                }
                else
                {
                    // 마스터 입장에서는 내가 아닌 상대가 사용했을 경우 → RPC로 해당 클라이언트에 지시
                    user._pv.RPC("RPC_ShowBulletInfo", user._pv.Owner, (int)GunManager.Instance.LoadedBullet);
                }
                break;


            default:
                Debug.LogWarning("정의되지 않은 아이템 타입");
                break;
        }
    }

    public void SetItemUsed(ItemData item)
    {
        item.isUsed = true;
    }
}
