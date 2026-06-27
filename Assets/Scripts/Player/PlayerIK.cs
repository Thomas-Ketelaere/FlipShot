using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    private Animator _animator;

    [Header("Right Hand")]
    [SerializeField] private Transform _rightHandTarget;
    [SerializeField][Range(0, 1)] private float _rightHandWeight = 1f;

    [Header("Left Hand")]
    [SerializeField] private Transform _leftHandTarget;   
    [SerializeField][Range(0, 1)] private float _leftHandWeight = 1f;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rightHandWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightHandWeight);

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftHandWeight);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftHandWeight);

        _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandTarget.position);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandTarget.rotation);

        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandTarget.position);
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandTarget.rotation);
    }
}
