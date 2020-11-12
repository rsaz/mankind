using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterCustomizationManager : MonoBehaviour
{
    public List<CharacterCustomization> characterCustomization = new List<CharacterCustomization>();  
    public void RandomOption()
    {
        foreach (CharacterCustomization option in characterCustomization)
        {
            option.RandomCustomization();
        }
    }

}
