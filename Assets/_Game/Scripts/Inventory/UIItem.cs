using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public Item item;
    private Image spriteImage;

    private void Awake()
    {
        spriteImage = GetComponent<Image>();
        Debug.Log(spriteImage.name);
        UpdateItem(null); //Always a one empty slot
    }

    public void UpdateItem(Item item)
    {
        this.item = item;
        if(this.item != null)
        {
            spriteImage.sprite = this.item.icon;
            spriteImage.color = Color.white;
        }
        else
        {
            spriteImage.color = Color.clear;
        }
    }
}
