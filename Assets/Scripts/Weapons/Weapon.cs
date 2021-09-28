using System.Collections;
using UnityEngine;

public abstract class Weapon : Ammunition
{
    [SerializeField] private protected Vector3 _localPositionForViewFromTheShoulder;
    [SerializeField] private protected Vector3 _localPositionForViewFromSight;
    [SerializeField] private protected int _clips;
    [SerializeField] private protected int _maxAmmunitionInClip;
    [SerializeField] private protected float _timeInSecondsBetweenShots;

    private protected UI _ui;
    private protected Transform _cameraTransform;
    private protected int _currentAmmunition;
    private protected bool _isReloading;
    private protected string _ammoData;
    private protected Transform _transform;

    public bool InHands { get; private set; }
    private bool _isViewFromSight;
    private float _timer = 0F;

    private protected abstract IEnumerator RecoilAnimation();

    private protected abstract IEnumerator ReloadAnimation();

    private protected abstract IEnumerator ShootCoroutine();

    private void Awake()
    {
        _currentAmmunition = _maxAmmunitionInClip;
        _transform = transform;
        _ui = FindObjectOfType<UI>();
    }

    public void Shoot()
    {
        if (_timer > _timeInSecondsBetweenShots)
        {
            _timer = 0;
            StartCoroutine(ShootCoroutine());
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public void ChangeView()
    {
        StartCoroutine(ChangeViewCoroutine());
    }

    public void ChangeOwnerOfWeapon(Transform parent)
    {
        InHands = true;
        _transform.parent = parent;
        _cameraTransform = parent;
        _transform.localPosition = _localPositionForViewFromTheShoulder;
        _transform.localEulerAngles = Vector3.zero;
        if (photonView.IsMine)
        {
            _ui.UpdateAmmo(_currentAmmunition, _clips);
        }
    }

    private IEnumerator ChangeViewCoroutine()
    {
        _isViewFromSight = !_isViewFromSight;
        var targetPosition = _isViewFromSight ? _localPositionForViewFromSight : _localPositionForViewFromTheShoulder;

        var framesForAnimation = 20;
        for (var i = 0; i < framesForAnimation; i++)
        {
            _transform.localPosition += Vector3.ClampMagnitude(targetPosition - _transform.localPosition, 1f / framesForAnimation);
            yield return null;
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        if (_clips > 0 && !_isReloading)
        {
            _isReloading = true;
            yield return StartCoroutine(ReloadAnimation());
            _clips--;
            _currentAmmunition = _maxAmmunitionInClip;
            _isReloading = false;
            _ui.UpdateAmmo(_currentAmmunition, _clips);
        }
    }
}