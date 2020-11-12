using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizationWithLock : CharacterCustomization
{
    [Header("Toggle responsable to Lock and Unlock")]
    public Toggle lockUnlock;

    [Header("Symmetric body part")]
    public GameObject correspondentPart;

    [Header("Symmetric body sprite")]
    public SpriteRenderer correspondentSprite;

    [Header("Correspondent Sprites to select")]
    public List<Sprite> correspondentOptions = new List<Sprite>();

    Button leftButton;
    Button rightButton;
    

    // Start is called before the first frame update
    void Start()
    {
        //lockUnlock.isOn = false;
        Button[] buttons = correspondentPart.GetComponentsInChildren<Button>();

        leftButton = buttons[0];
        rightButton = buttons[1]; 
    }

    public void DisableButtons(bool isLocked)
    {
        NextOptionBoth(isLocked);
        PreviousOptionBoth(isLocked);

        if (isLocked) 
        {
            leftButton.interactable = false;
            rightButton.interactable = false;
        }else
        {
            leftButton.interactable = true;
            rightButton.interactable = true;
        }
    }

    public void NextOptionBoth(bool isLocked)
    {
        currentOption++;
        if (isLocked && currentOption >= options.Count)
        {
            currentOption = 0;
        }

        part.sprite = options[currentOption];
        correspondentSprite.sprite = correspondentOptions[currentOption];
    }

    public void PreviousOptionBoth(bool isLocked)
    {
        currentOption--;
        if (isLocked && currentOption <= 0)
        {
            currentOption = options.Count - 1;
        }
        part.sprite = options[currentOption];
        correspondentSprite.sprite = correspondentOptions[currentOption];
    }


}
