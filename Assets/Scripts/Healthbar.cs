using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    public float updateSpeed = .5f;
    private Dictionary<Unit, CoroutineInfo> currentCoroutines;

    private struct CoroutineInfo {
        public HealthFillInfo healthFillInfo;
        public Coroutine coroutine;
        public CoroutineInfo (HealthFillInfo _healthFillInfo, Coroutine _coroutine) {
            healthFillInfo = _healthFillInfo;
            coroutine = _coroutine;
        }
    }
    private struct HealthFillInfo {
        public float newTotal;
        public float currentHealth;
        public Unit unit;
        public HealthFillInfo (float _newTotal, float _currentHealth, Unit _unit) {
            newTotal = _newTotal;
            currentHealth = _currentHealth;
            unit = _unit;
        }
    }

    // Use this for initialization
    void Start () {
        currentCoroutines = new Dictionary<Unit, CoroutineInfo> ();
        Unit.OnDamageTaken += OnDamageTaken;
    }

    void OnDestroy () {
        Unit.OnDamageTaken -= OnDamageTaken;
    }

    private void OnDamageTaken (Unit unit, int currentHealth, int maxHealth, int damageTaken) {
        if (currentCoroutines.ContainsKey (unit)) {
            CoroutineInfo temp = currentCoroutines[unit];
            StopCoroutine (temp.coroutine);
            currentCoroutines.Remove (unit);
        }
        float newTotal = (float) (currentHealth - damageTaken) / (float) maxHealth;
        if (newTotal < 0) {
            newTotal = 0;
        }
        HealthFillInfo info = new HealthFillInfo (newTotal, currentHealth, unit);
        Coroutine thisCoroutine = StartCoroutine ("AnimateHealthBar", info);
        CoroutineInfo coroutineInfo = new CoroutineInfo (info, thisCoroutine);
        currentCoroutines.Add (unit, coroutineInfo);
    }

    private IEnumerator AnimateHealthBar (HealthFillInfo info) {
        float newTotal = info.newTotal;
        Image foregroundImage = null;
        if (info.unit.transform.Find ("Health Canvas/Health Foreground").GetComponent<Image> ()) {
            foregroundImage = info.unit.transform.Find ("Health Canvas/Health Foreground").GetComponent<Image> ();
        } else {
            Debug.LogError ("Could not find healthbar image");
            yield break;
        }
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeed) {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp (preChangePct, newTotal, elapsed / updateSpeed);
            yield return null;
        }

        foregroundImage.fillAmount = newTotal;
        currentCoroutines.Remove (info.unit);
    }
}