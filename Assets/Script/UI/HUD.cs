using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TMP_Text coin_text;
    private Player player;
    // Update is called once per frame

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }
    void Update()
    {
        //coin_text.text = player.coin.ToString() + " Àü"; ;
    }
}
