using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LTH
{
    public class Player : MonoBehaviour
    {
        public string FirebaseUID;
        public int currentHp;
        public int maxHp;
        public bool isAlive;
        public List<ItemData> itemslot = new();

        public UnityEvent OnGameOver = new();

        public void Init()
        {
            maxHp = 4;
            currentHp = maxHp;
            isAlive = true;
            itemslot.Clear();
        }

        public void SetHp(int hp)
        {
            currentHp = Mathf.Clamp(hp, 0, maxHp);
            if (currentHp <= 0) Die();
        }

        public void TakeDamage(int amount)
        {
            SetHp(currentHp - amount);
        }

        private void Die() // �ӽ� ����
        {
            if (!isAlive) return;
            isAlive = false;
            OnGameOver?.Invoke();
        }

        // Dictionary Ű�� ����ϱ� ���� ��� ��
        public override bool Equals(object obj)
        {
            return obj is Player other && FirebaseUID == other.FirebaseUID;
        }

        public override int GetHashCode()
        {
            return FirebaseUID?.GetHashCode() ?? 0;
        }
    }
}