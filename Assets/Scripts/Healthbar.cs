using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{

    public Image foregroundImage;
    Unit unit;
    public float updateSpeed = .5f;
    private Coroutine CoroutineHandler;
    private struct HealthFillInfo
    {
        public float newTotal;
        public Unit unit;
        public HealthFillInfo(float _newTotal, Unit _unit)
        {
            newTotal = _newTotal;
            unit = _unit;
        }
    }

    // Use this for initialization
    void Start()
    {
        Unit.OnDamageTaken += OnDamageTaken;
    }

    private void OnDamageTaken(Unit unit, int currentHealth, int maxHealth, int damageTaken)
    {
        float newTotal = (float)(currentHealth - damageTaken) / (float)maxHealth;
        if (newTotal < 0)
        {
            newTotal = 0;
        }
        if (CoroutineHandler != null)
        {
            StopCoroutine(CoroutineHandler);
        }
        HealthFillInfo info = new HealthFillInfo(newTotal, unit);
        CoroutineHandler = StartCoroutine(AnimateHealthBar(info));
    }

    private IEnumerator AnimateHealthBar(HealthFillInfo info)
    {
        float newTotal = info.newTotal;
        if (info.unit.transform.Find("Health Canvas/Health Foreground").GetComponent<Image>())
        {
            foregroundImage = info.unit.transform.Find("Health Canvas/Health Foreground").GetComponent<Image>();
        } else {
            Debug.LogError("Could not find healthbar image");
        }
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeed)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, newTotal, elapsed / updateSpeed);
            yield return null;
        }
        foregroundImage.fillAmount = newTotal;
    }
}
