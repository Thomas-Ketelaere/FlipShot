using System.Collections;
using UnityEngine;

public class RevertableBulletHole : RevertableBase
{
    private PlayerControlsComponent _playerMovementComponent;
    private BulletParticleSysComponent _bulletParticleSysComponent;

    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerMovementComponent = player.GetComponent<PlayerControlsComponent>();
        _bulletParticleSysComponent = GetComponent<BulletParticleSysComponent>();
    }

    public override void RevertObject()
    {
        if (!_isActive) return;
        
        //todo check if reverting is possible (is magazine full)
        GameObject bulletShell = BulletsManager.Instance.RequestBulletShellObject();
        Transform ejectionTransform = _playerMovementComponent.GetPlayerWeapon().GetEjectionPoint();

        Vector3 playerPos = _playerMovementComponent.transform.position;
        Vector3 rightOffset = _playerMovementComponent.transform.right * 0.75f; // Adjust 0.75f for how far right you want it
        Vector3 targetFloorPos = playerPos + rightOffset;
        targetFloorPos.y = 0.1f; //todo check if this is above ground or not

        bulletShell.transform.position = targetFloorPos;
        bulletShell.transform.rotation = Random.rotation;

        Rigidbody shellRb = bulletShell.GetComponentInChildren<Rigidbody>();
        if (shellRb != null)
        {
            shellRb.isKinematic = true;
        }

        // 5. Start the reverse physics animation
        StartCoroutine(AnimateShellReverse(bulletShell, ejectionTransform));

        //_playerMovementComponent.GetPlayerWeapon().AddBullet(); //this should happen when the bullet shell gets to the weapon, same for destroying this object
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

        _playerMovementComponent.GetPlayerWeapon().AddBullet();

        TempBulletComponent bulletShell = shell.GetComponent<TempBulletComponent>();
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
