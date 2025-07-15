using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy/EnemyData")]
public class Enemy_data : ScriptableObject
{
    [Header("이름, 코인, 무게, 분류")]
    public string Name;
    public int Coin;
    public int Weight;

    public enum Classification
    {
        Yokai,
        Ghost
    }
    public Classification classification;

    [Header("체력, 위험, 약점, 수량")]
    public int Hp;
    public int D_Point;

    public EnemyWeakness weakness;

    public int population;

    [Header("랜덤 값 설정")]
    public int CoinDeviation;
    public int HpDeviation;
}