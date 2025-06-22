using UnityEngine;

public class RecoilComponent : MonoBehaviour
{
    [SerializeField] private float _recoilX;
    [SerializeField] private float _recoilY;
    [SerializeField] private float _recoilZ;

    [SerializeField] private float _snappiness;
    [SerializeField] private float _returnSpeed;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    private bool _enableRecoil;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_enableRecoil)
        {
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _returnSpeed * Time.deltaTime);
            _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, _snappiness * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(_currentRotation);

            if (_targetRotation == Vector3.zero)
            {
                _enableRecoil = false;
            }
        }
    }

    public void AddRecoil()
    {
        _targetRotation += new Vector3(_recoilX, Random.Range(-_recoilY, _recoilY), Random.Range(-_recoilZ, _recoilZ));
        _enableRecoil = true;
    }

    public void ResetRecoil()
    {
        _targetRotation = Vector3.zero;
        _enableRecoil = false;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
