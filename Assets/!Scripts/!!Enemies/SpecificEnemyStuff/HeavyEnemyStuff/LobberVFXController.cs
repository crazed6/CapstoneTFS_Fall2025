using UnityEngine;
using UnityEngine.VFX;

public class LobberVFXController : EnemyVFXController
{
    [Header("Attack VFX")]
    [SerializeField] private VisualEffect rockThrowChargeUpPrefab;
    [SerializeField] private Transform rockThrowChargeUpPoint;
    [SerializeField] private VisualEffect slamVFXPrefab;
    [SerializeField] private Transform slamVFXPoint;

    private VisualEffect activeChargeUpVFX;

    public void PlayRockThrowChargeUpVFX()
    {
        if (rockThrowChargeUpPrefab != null && rockThrowChargeUpPoint != null)
        {
            activeChargeUpVFX = Instantiate(
                rockThrowChargeUpPrefab,
                rockThrowChargeUpPoint.position,
                rockThrowChargeUpPoint.rotation,
                rockThrowChargeUpPoint
            );

            activeChargeUpVFX.Play();
        }
    }

    public void StopRockThrowChargeUpVFX()
    {
        if (activeChargeUpVFX != null)
        {
            activeChargeUpVFX.Stop();
            Destroy(activeChargeUpVFX.gameObject);
            activeChargeUpVFX = null;
        }
    }

    public void PlaySlamVFX()
    {
        if (slamVFXPrefab != null && slamVFXPoint != null)
        {
            VisualEffect slamVFX = Instantiate(
                slamVFXPrefab,
                slamVFXPoint.position,
                slamVFXPoint.rotation
            );
            slamVFX.Play();

            Destroy(slamVFX.gameObject, 5f);
        }
    }
}
