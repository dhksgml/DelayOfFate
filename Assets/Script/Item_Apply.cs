using UnityEngine;

public class Item_Apply : MonoBehaviour
{
    public static bool[] A; // 역량 7개
    public static bool[] B; // 재련 5개
    public static bool[] C; // 탐욕 4개
    public static bool[] D; // 맷집 7개
    public static bool[] E; // 기만 5개
    public static bool[] F; // 성장 6개
    public static bool[] G; // 극복 8개

    private void Awake()
    {
        A = new bool[7];
        B = new bool[5];
        C = new bool[4];
        D = new bool[7];
        E = new bool[5];
        F = new bool[6];
        G = new bool[8];
    }
}
