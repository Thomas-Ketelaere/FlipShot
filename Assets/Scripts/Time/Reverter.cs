using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Reverter : MonoBehaviour
{
    private bool _inTemporalMode;
    private bool _isReverting = false;
    private const float SPHERE_CAST_RADIUS = 0.2f;
    private int _reverterLayerMask;
    private const float SPHERE_CAST_MAX_DISTANCE = 10f;
    private const string REVERTABLE_LAYER_NAME = "Revertable";
    private Transform _cameraTransform;

    private bool _hadHitLastFrame;
    private float _currentHitDistance;

    [Header("TemporalMode")]
    [SerializeField] private GameObject _overlayCamera;
    [SerializeField] private Volume _volume;
    [SerializeField] private Material _desaturationMat;
    private const float MaxChromaticAberration = 0.5f;
    private const float MinDesaturationStrength = 0.1f;
    private const float TemporalTransitionDuration = 0.15f;
    private ChromaticAberration _chromaticAberration;
    private Coroutine _temporalTransition;

    public bool InTemporalMode => _inTemporalMode;

    void Start()
    {
        _reverterLayerMask = LayerMask.GetMask(REVERTABLE_LAYER_NAME);
        _cameraTransform = Camera.main.transform;
        if (_volume.sharedProfile.TryGet<ChromaticAberration>(out ChromaticAberration CA))
        {
            _chromaticAberration = CA;
        }
         
    }

    private void FixedUpdate()
    {
        if (!_isReverting)
        {
            _hadHitLastFrame = false;
            return;
        }
        RaycastHit hit;
        if (Physics.SphereCast(_cameraTransform.position, SPHERE_CAST_RADIUS, _cameraTransform.forward, out hit, SPHERE_CAST_MAX_DISTANCE, _reverterLayerMask))
        {
            RevertableBase revertable = hit.transform.GetComponentInParent<RevertableBase>();
            if (revertable == null)
            {
                Debug.LogError($"[Reverter] Object: {hit.transform.gameObject.name} does not have an IRevertable!");
                return;
            }
            revertable.RevertObject();
            Debug.Log("Hit");
            _hadHitLastFrame = true;
            _currentHitDistance = hit.distance;
        }
        else
        {
            Debug.Log("No hit");
            _hadHitLastFrame = false;
        }
    }

    public void Revert(bool started)
    {
        if (started)
        {
            _isReverting = true;
        }
        else
        {
            _isReverting = false;
        }
    }

    public void TemporalMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _inTemporalMode = !_inTemporalMode;
            if (_inTemporalMode)
            {
                OnTemporalModeStart();
            }
            else
            {
                OnTemporalModeStop();
            }
        }
    }

    private void OnTemporalModeStart()
    {
        _overlayCamera.SetActive(true);
        if (_temporalTransition != null) StopCoroutine(_temporalTransition);
        _temporalTransition = StartCoroutine(LerpTemporalEffects(true, MaxChromaticAberration, MinDesaturationStrength));
    }

    private void OnTemporalModeStop()
    {
        if (_temporalTransition != null) StopCoroutine(_temporalTransition);
        _temporalTransition = StartCoroutine(LerpTemporalEffects(false, 0f, 1f));
    }

    private IEnumerator LerpTemporalEffects(bool entering, float targetCA, float targetDesat)
    {
        float startCA =_chromaticAberration.intensity.value;
        float startDesat = _desaturationMat.GetFloat("_DesaturationStrength");

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / TemporalTransitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            _chromaticAberration.intensity.value = Mathf.Lerp(startCA, targetCA, smoothT);
            _desaturationMat.SetFloat("_DesaturationStrength", Mathf.Lerp(startDesat, targetDesat, smoothT));

            yield return null;
        }

        if (!entering) _overlayCamera.SetActive(false);
    }

    private void OnDestroy()
    {
        _desaturationMat.SetFloat("_DesaturationStrength", 1);
        _chromaticAberration.intensity.value = 0f;
    }

    #region UnityOnly

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !_isReverting) return;

        Gizmos.color = _hadHitLastFrame ? Color.red : Color.green;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        float castLength = _hadHitLastFrame ? _currentHitDistance : SPHERE_CAST_MAX_DISTANCE;
        Vector3 endPoint = origin + direction * castLength;

        Gizmos.DrawWireSphere(origin, SPHERE_CAST_RADIUS);

        Gizmos.DrawWireSphere(endPoint, SPHERE_CAST_RADIUS);

        Vector3 upOffset = transform.up * SPHERE_CAST_RADIUS;
        Vector3 rightOffset = transform.right * SPHERE_CAST_RADIUS;

        Gizmos.DrawLine(origin + upOffset, endPoint + upOffset);
        Gizmos.DrawLine(origin - upOffset, endPoint - upOffset);

        Gizmos.DrawLine(origin + rightOffset, endPoint + rightOffset);
        Gizmos.DrawLine(origin - rightOffset, endPoint - rightOffset);
    }

    #endregion
}
