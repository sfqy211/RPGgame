using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private float crystalExistTimer;
    public void SetupCrystal(float _crystalDuration)
    {
        crystalExistTimer = _crystalDuration;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;
        if (crystalExistTimer < 0)
        {
            SelfDestory();
        }
    }

    public void SelfDestory() => Destroy(gameObject);
}
