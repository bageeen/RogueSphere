using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{

    public Slider slider;
    public Color color;
    public Image fill;
    public bool isFadeOut = false;

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public void SetAmmo(float ammo)
    {
        slider.value = ammo;
        fill.color = this.color;
    }

    public void SetMaxAmmo(float ammo)
    {
        slider.maxValue = ammo;
        slider.value = ammo;

        fill.color = this.color;
    }
}