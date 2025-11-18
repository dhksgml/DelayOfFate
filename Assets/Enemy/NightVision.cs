using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightVision : MonoBehaviour
{
    public Sprite[] eye_sprites;
    public Image image;
    void Start()
    {
        image.sprite = eye_sprites[Random.Range(0, eye_sprites.Length)];
    }
}
