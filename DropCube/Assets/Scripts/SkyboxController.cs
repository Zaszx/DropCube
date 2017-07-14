using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    private Material _skyMaterial;
    private float _baseExponent;
    private float _failExponent;

    private bool _isEffectRunning;

    void Start()
    {
        // Create new instance, don't mess with the asset itself
        _skyMaterial = new Material(Camera.main.GetComponent<Skybox>().material);
        Camera.main.GetComponent<Skybox>().material = _skyMaterial;

        _baseExponent = _skyMaterial.GetFloat("_Exponent");
        _failExponent = 0.91f;
    }

    public void FailEffect()
    {
        StartCoroutine(FailEffectCoroutine());
    }

    private IEnumerator FailEffectCoroutine()
    {
        if (_isEffectRunning)
        {
            yield break;
        }

        _isEffectRunning = true;

        const float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Curve.Instance.SkyColorChangeFail.Evaluate(elapsedTime / duration);

            _skyMaterial.SetFloat("_Exponent", Mathf.Lerp(_baseExponent, _failExponent, t));

            yield return null;
        }
        _isEffectRunning = false;
    }

    public void UndoEffect()
    {
        StartCoroutine(UndoEffectCoroutine());
    }

    private IEnumerator UndoEffectCoroutine()
    {
        if (_isEffectRunning)
        {
            yield break;
        }

        _isEffectRunning = true;

        const float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Curve.Instance.SkyColorChangeUndo.Evaluate(elapsedTime / duration);

            _skyMaterial.SetFloat("_Exponent", Mathf.Lerp(_failExponent, _baseExponent, t));

            yield return null;
        }
        _isEffectRunning = false;
    }
}
