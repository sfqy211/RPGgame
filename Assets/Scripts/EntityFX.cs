using System;
using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    //特效文件
    private SpriteRenderer sr;

    [Header("Flash FX 特效参数")] [SerializeField]
    private float flashDuration; // 闪烁时间

    [SerializeField] private Material hitMat; // 闪烁材质
    private Material originalMat; // 原始材质

    [Header("Ailment colors 受伤颜色")] [SerializeField]
    private Color[] chillColor; // 冰冻

    [SerializeField] private Color[] igniteColor; // 燃烧
    [SerializeField] private Color[] shockColor; // 感电

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }
    public void MakeTransparent(bool _transparent)
    {
        sr.color = _transparent ? Color.clear : Color.white;
    }
    public IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    public void RedColorBlink()
    {
        sr.color = (sr.color != Color.white) ? Color.white : Color.red;
    }

    public void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    // public void IgniteFxFor(float _seconds)
    // {
    //     InvokeRepeating(nameof(IgniteColorFx), 0, .3f);
    //     Invoke(nameof(CancelColorChange), _seconds);
    // }
    //
    // public void ChillFxFor(float _seconds)
    // {
    //     InvokeRepeating(nameof(ChillColorFx), 0, .3f);
    //     Invoke(nameof(CancelColorChange), _seconds);
    // }
    //
    // public void ShockFxFor(float _seconds)
    // {
    //     InvokeRepeating(nameof(ShockColorFx), 0, .3f);
    //     Invoke(nameof(CancelColorChange), _seconds);
    // }
    public void FxFor(Action _colorFx, float _seconds)
    {
        InvokeRepeating(_colorFx.Method.Name, 0, .3f);
        Invoke(nameof(CancelColorChange), _seconds);
    }

    public void IgniteColorFx()
    {
        sr.color = (sr.color != igniteColor[0]) ? igniteColor[0] : igniteColor[1];
    }
    public void ChillColorFx()
    {
        sr.color = (sr.color != chillColor[0]) ? chillColor[0] : chillColor[1];
    }
    public void ShockColorFx()
    {
        sr.color = (sr.color != shockColor[0]) ? shockColor[0] : shockColor[1];
    }
}