using UnityEngine;

public class Item_Apply : MonoBehaviour
{
    public static bool[] A; // ���� 7��
    public static bool[] B; // ��� 5��
    public static bool[] C; // Ž�� 4��
    public static bool[] D; // ���� 7��
    public static bool[] E; // �⸸ 5��
    public static bool[] F; // ���� 6��
    public static bool[] G; // �غ� 8��

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
