using System;
using System.Collections;
using UnityEngine;

public class RevertableGrenade : RevertableBase
{
    [SerializeField] private ParticleSystem _explosionVFX;
    [SerializeField] private MeshRenderer _grenadeVisualMesh; //Todo also safety pin and lever
    [SerializeField] private Transform _safetyPin; //should revert to grenade AFTER rolled back to player 
    [SerializeField] private Transform _safetyLever; //should revert to grenade WHILE rolled back to player 
    private Animator _animator;
    private float _explosionTime = 5f; //5 seconds till grenade explodes
    private float _rollToPlayerTime = 2f; //3 seconds throw time, means 2 seconds on floor

    private Transform _playerTransform;
    private Transform _cameraTransform;

    private void Awake()
    {
        //_grenadeVisualMesh.enabled = false; //doing it here so it's still visible when making the levels
        _safetyLever.gameObject.SetActive(false);
    }

    private void Start()
    {

        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    public override void RevertObject()
    {
        if (!_isActive) return;
        _isActive = false;
        
        _explosionVFX.Play();
        float time = _explosionVFX.main.duration; //doesnt work
        float onGroundTime = time + _explosionTime - _rollToPlayerTime;
        Debug.Log("ground time: " +  onGroundTime);
        Invoke(nameof(RollToPlayer), 2f);
        //_animator.Play("")
    }

    public void RollToPlayer()
    {
        //after "implosion" happened, should roll to player since they "threw" it
        _grenadeVisualMesh.enabled = true;
        //should make lever also visible again
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) 
            rb.isKinematic = true;

        StartCoroutine(ExecuteReverseMovement());
    }

    private IEnumerator ExecuteReverseMovement()
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        // Phase 1: Ground Roll (Accelerating backward over time)
        while (elapsed < _rollToPlayerTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _rollToPlayerTime;

            // Invert the easing: Starts slow (flat), accelerates quickly at the end (steep)
            float acceleratingT = Mathf.Pow(t, 2);

            // Track player dynamically since they are sprinting around linearly
            Vector3 currentPlayerPos = _playerTransform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);

            // Check if we are outside the 2-meter air-launch threshold
            if (distanceToPlayer > 2.0f)
            {
                // Roll along the floor towards the player's feet
                Vector3 targetFloorPos = currentPlayerPos;

                // Keep it locked to the ground level during the roll
                targetFloorPos.y = startPosition.y;

                transform.position = Vector3.Lerp(startPosition, targetFloorPos, acceleratingT);

                // Spin the grenade along its moving direction to simulate rolling
                transform.Rotate(Vector3.right * (500f * Time.deltaTime), Space.Self);
            }
            else
            {
                // Break out of the rolling loop early! We are close enough to launch into the air
                break;
            }

            yield return null;
        }

        // Phase 2: Air Arc to Camera (Handles both standard approach or if spawned closer than 2m)
        Vector3 airStartPosition = transform.position;
        Quaternion airStartRotation = transform.rotation;

        float airDuration = 0.4f; // Quick snap up to the face
        float airElapsed = 0f;
        _safetyLever.gameObject.SetActive(true);
        Vector3 leverStartLocalPos = _safetyLever.localPosition;
        Quaternion leverStartLocalRot = _safetyLever.localRotation;

        while (airElapsed < airDuration)
        {
            airElapsed += Time.deltaTime;
            float airT = airElapsed / airDuration;
            float smoothAirT = Mathf.SmoothStep(0f, 1f, airT);

            // Target a point slightly forward and down from the center eye line of the camera
            Vector3 targetViewPos = _cameraTransform.position + (_cameraTransform.forward * 0.6f) + (_cameraTransform.up * -0.2f);

            // Create a slight upward leap arc
            Vector3 midPoint = Vector3.Lerp(airStartPosition, targetViewPos, 0.5f) + Vector3.up * 0.5f;

            Vector3 m1 = Vector3.Lerp(airStartPosition, midPoint, smoothAirT);
            Vector3 m2 = Vector3.Lerp(midPoint, targetViewPos, smoothAirT);
            transform.position = Vector3.Lerp(m1, m2, smoothAirT);

            // Orient smoothly to align cleanly with the camera view angle at the end
            transform.rotation = Quaternion.Slerp(airStartRotation, _cameraTransform.rotation, smoothAirT);

            // WHILE flying up, animate the safety lever snapping back onto the side of the grenade body
            if (_safetyLever != null)
            {
                //lever should become also visible and be spawned somewhere randomly close to the grenade, (this needs to guaranteed be in the same room, if grenade is close to wall, should not be on opposite side of wall)
                _safetyLever.localRotation = Quaternion.Slerp(leverStartLocalRot, Quaternion.identity, smoothAirT);
                _safetyLever.localPosition = Vector3.Lerp(leverStartLocalPos, Vector3.zero, smoothAirT);
            }

            yield return null;
        }

        // Phase 3: Arrival & Cleanup
        // AFTER reaching the player's view completely, click the safety pin back inside
        transform.SetParent(_cameraTransform.parent, true); //temp set it to follow camera so player sees what happens
        if (_safetyPin != null)
        {
            //Should be animation here
            //so
            //1. make pin visible (under player, same as mag)
            //2. play animation
            _safetyPin.gameObject.SetActive(true); // Or pop it back into place via rotation/position
            _animator.Play("GrenadePin", 0, 0f);
        }

        // Give the grenade item back to weapon inventory
        // _playerMovementComponent.GetPlayerWeapon().AddGrenade();

        
    }

    public void DestroyGrenade()
    {
        Destroy(gameObject);
    }
}

