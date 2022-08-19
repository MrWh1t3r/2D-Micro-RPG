using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI interactText;
    public Image healthBarFill;
    public Image xpBarFill;
    
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    public void UpdateLevelText()
    {
        levelText.text = "Lvl\n" + _player.curLevel;
    }

    public void UpdateHealthBar()
    {
        healthBarFill.fillAmount = (float)_player.curHp / (float)_player.maxHp;
    }

    public void UpdateXpBar()
    {
        xpBarFill.fillAmount = (float)_player.curXp / (float)_player.xpToNextLevel;
    }

    public void SetInteractText(Vector3 pos, string text)
    {
        interactText.gameObject.SetActive(true);
        interactText.text = text;

        interactText.transform.position = Camera.main.WorldToScreenPoint(pos + Vector3.up);
    }

    public void DisableInteractText()
    {
        if(interactText.gameObject.activeInHierarchy)
            interactText.gameObject.SetActive(false);
    }

    public void UpdateInventoryText()
    {
        interactText.text = "";

        foreach (var item in _player.inventory)
        {
            inventoryText.text += item + "\n";
        }
    }
}
