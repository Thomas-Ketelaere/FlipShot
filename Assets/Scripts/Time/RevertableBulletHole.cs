using System.Collections;
using UnityEngine;

public class RevertableBulletHole : RevertableBase
{
    private ParticleSystemSingleObject _bulletParticleSysComponent;

    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerControlsComponent = player.GetComponent<PlayerControlsComponent>();
        _bulletParticleSysComponent = GetComponent<ParticleSystemSingleObject>();
    }

    public override void RevertObject()
    {
        if (!_isActive) return;
        
        //todo check if reverting is possible (is magazine full) and if wall is inbetween (should be in revertableChecker)
        GameObject bulletShell = ObjectPool.Instance.RequestBulletShellObject();
        Transform ejectionTransform = _playerControlsComponent.GetPlayerWeapon().GetEjectionPoint();

        Vector3 playerPos = _playerControlsComponent.transform.position;
        Vector3 rightOffset = _playerControlsComponent.transform.right * 0.75f; // Adjust 0.75f for how far right you want it
        Vector3 targetFloorPos = playerPos + rightOffset;
        targetFloorPos.y = 0.1f; //todo check if this is above ground or not

        bulletShell.transform.position = targetFloorPos;
        bulletShell.transform.rotation = Random.rotation;

        Rigidbody shellRb = bulletShell.GetComponentInChildren<Rigidbody>();
        if (shellRb != null)
        {
            shellRb.isKinematic = true;
        }

        StartCoroutine(AnimateShellReverse(bulletShell, ejectionTransform));

        //weapon Visuals
        _isActive = false;
    }

    private IEnumerator AnimateShellReverse(GameObject shell, Transform targetEjectionPoint)
    {
        Vector3 startPosition = shell.transform.position;
        Quaternion startRotation = shell.transform.rotation;

        float duration = 0.35f; // How long the reverse flight takes in seconds
        float elapsed = 0f;

        // Generate a random spin direction for the reverse tumble
        Vector3 rotationAxis = Random.onUnitSphere;
        float totalRotationDegrees = Random.Range(360f, 720f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float smoothedT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 currentTargetPos = targetEjectionPoint.position; //using transform since player can move so needs to be pointer

            Vector3 midPoint = Vector3.Lerp(startPosition, currentTargetPos, 0.5f) + Vector3.up * 1.2f;

            //Bezier formula to calculate the arc position
            Vector3 m1 = Vector3.Lerp(startPosition, midPoint, smoothedT);
            Vector3 m2 = Vector3.Lerp(midPoint, currentTargetPos, smoothedT);
            shell.transform.position = Vector3.Lerp(m1, m2, smoothedT);

            Quaternion currentSpin = Quaternion.AngleAxis(Mathf.Lerp(totalRotationDegrees, 0f, smoothedT), rotationAxis);
            shell.transform.rotation = currentSpin * Quaternion.Slerp(startRotation, targetEjectionPoint.rotation, smoothedT);

            yield return null;
        }
        _playerControlsComponent.GetPlayerWeapon().RevertShoot(transform.position);

        SingleObject bulletShell = shell.GetComponent<SingleObject>();
        bulletShell.SetInactive();
        if(_bulletParticleSysComponent  != null)
        {
            _bulletParticleSysComponent.SetInactive();
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
