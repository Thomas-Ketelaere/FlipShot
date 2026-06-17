using System;
using System.Collections;
using UnityEngine;

//todo code cleanup
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

    private MeshCollider _grenadeMeshCollider;
    private Rigidbody _rb;

    private void Awake()
    {
        //_grenadeVisualMesh.enabled = false; //doing it here so it's still visible when making the levels
        _safetyLever.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        _rb = GetComponent<Rigidbody>();
        _grenadeMeshCollider = _grenadeVisualMesh.GetComponent<MeshCollider>();
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
    }

    public void RollToPlayer()
    {
        _grenadeVisualMesh.enabled = true;
        _rb.isKinematic = true;

        StartCoroutine(ExecuteReverseMovement());
    }

    private IEnumerator ExecuteReverseMovement()
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < _rollToPlayerTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _rollToPlayerTime;

            float acceleratingT = Mathf.Pow(t, 2);

            Vector3 currentPlayerPos = _playerTransform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, currentPlayerPos);

            if (distanceToPlayer > 2.0f)
            {
                Vector3 targetFloorPos = currentPlayerPos;
                targetFloorPos.y = startPosition.y;

                transform.position = Vector3.Lerp(startPosition, targetFloorPos, acceleratingT);
                transform.Rotate(Vector3.right * (500f * Time.deltaTime), Space.Self);
            }
            else
            {
                break;
            }

            yield return null;
        }

        Vector3 airStartPosition = transform.position;
        Quaternion airStartRotation = transform.rotation;

        float airDuration = 0.4f; 
        float airElapsed = 0f;
        _safetyLever.gameObject.SetActive(true);
        Vector3 leverStartLocalPos = _safetyLever.localPosition;
        Quaternion leverStartLocalRot = _safetyLever.localRotation;
        _grenadeMeshCollider.enabled = false;
        while (airElapsed < airDuration)
        {
            airElapsed += Time.deltaTime;
            float airT = airElapsed / airDuration;
            float smoothAirT = Mathf.SmoothStep(0f, 1f, airT);

            Vector3 targetViewPos = _cameraTransform.position + (_cameraTransform.forward * 0.6f) + (_cameraTransform.up * -0.2f);

            Vector3 midPoint = Vector3.Lerp(airStartPosition, targetViewPos, 0.5f) + Vector3.up * 0.5f;

            Vector3 m1 = Vector3.Lerp(airStartPosition, midPoint, smoothAirT);
            Vector3 m2 = Vector3.Lerp(midPoint, targetViewPos, smoothAirT);
            transform.position = Vector3.Lerp(m1, m2, smoothAirT);
            transform.rotation = Quaternion.Slerp(airStartRotation, _cameraTransform.rotation, smoothAirT);

            if (_safetyLever != null)
            {
                //lever should become also visible and be spawned somewhere randomly close to the grenade, (this needs to guaranteed be in the same room, if grenade is close to wall, should not be on opposite side of wall)
                _safetyLever.localRotation = Quaternion.Slerp(leverStartLocalRot, Quaternion.identity, smoothAirT);
                _safetyLever.localPosition = Vector3.Lerp(leverStartLocalPos, Vector3.zero, smoothAirT);
            }

            yield return null;
        }

        transform.SetParent(_cameraTransform.parent, true); //temp set it to follow camera so player sees what happens
        if (_safetyPin != null)
        {
            //Should be animation here
            //so
            //1. make pin visible (under player, same as mag)
            //2. play animation
            _safetyPin.gameObject.SetActive(true);
            _animator.Play("GrenadePin", 0, 0f);
        }

        //todo add to inventory
    }

    public void DestroyGrenade()
    {
        Destroy(gameObject);
    }
}

