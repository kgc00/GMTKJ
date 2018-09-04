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

    // Use this for initialization
    void Start()
    {
        foregroundImage = transform.Find("Health Foreground").GetComponent<Image>();
        unit = GetComponentInParent<Unit>();
        unit.OnDamageTaken += OnDamageTaken;
        foregroundImage.fillAmount = 1;
    }

    private void OnDamageTaken(int currentHealth, int maxHealth, int damageTaken)
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
        CoroutineHandler = StartCoroutine(AnimateHealthBar(newTotal));
    }

    private IEnumerator AnimateHealthBar(float newTotal)
    {
		float preChangePct = foregroundImage.fillAmount;
		float elapsed = 0f;

		while (elapsed < updateSpeed){
			elapsed += Time.deltaTime;
			foregroundImage.fillAmount = Mathf.Lerp(preChangePct, newTotal, elapsed / updateSpeed);
			yield return null;
		}
		foregroundImage.fillAmount = newTotal;
    }
}
