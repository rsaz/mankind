using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    #region Properties

    [Header("Body part to be changed")]
    public SpriteRenderer part;

    [Header("Sprites to select")]
    public List<Sprite> options = new List<Sprite>();

    //SpriteRenderer currentPart = part;
    public int currentOption = 0;

    #endregion

    #region Methods

    public void NextOption()
    {
        currentOption++;
        if (currentOption >= options.Count)
        {
            currentOption = 0;
        }

        part.sprite = options[currentOption];
    }

    public void PreviousOption()
    {
        currentOption--;
        if (currentOption <= 0)
        {
            currentOption = options.Count - 1;
        }
        part.sprite = options[currentOption];
    }
    public void RandomCustomization(){    
        currentOption = Random.Range(0, options.Count - 1);
        part.sprite = options[currentOption];
    }

    #endregion
}
