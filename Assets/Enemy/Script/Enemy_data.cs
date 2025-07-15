using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy/EnemyData")]
public class Enemy_data : ScriptableObject
{
    [Header("�̸�, ����, ����, �з�")]
    public string Name;
    public int Coin;
    public int Weight;

    public enum Classification
    {
        Yokai,
        Ghost
    }
    public Classification classification;

    [Header("ü��, ����, ����, ����")]
    public int Hp;
    public int D_Point;

    public EnemyWeakness weakness;

    public int population;

    [Header("���� �� ����")]
    public int CoinDeviation;
    public int HpDeviation;
}