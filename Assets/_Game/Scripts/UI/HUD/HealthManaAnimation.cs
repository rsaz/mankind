using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManaAnimation : MonoBehaviour
{
    [SerializeField] float timerDefinedHealth = 10f;
    [SerializeField] float timerDefinedMana = 10f;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject mana;

    private void Start()
    {
        var timerHealth = timerDefinedHealth;
        var timerMana = timerDefinedMana;
        InvokeRepeating("HeartAnimation", 3, timerHealth);
        InvokeRepeating("ManaAnimation", 6, timerMana);
    }

    private void HeartAnimation()
    {
        heart.GetComponent<Animator>().Play("healthbar-heart");
    }

    private void ManaAnimation()
    {
        mana.GetComponent<Animator>().Play("mana");
    }
}
