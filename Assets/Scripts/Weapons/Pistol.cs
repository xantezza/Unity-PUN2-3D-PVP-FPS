using System.Collections;
using UnityEngine;

public class Pistol : Weapon
{
    private protected override IEnumerator RecoilAnimation()
    {
        for (var i = 0; i > -15; i -= 3)
        {
            _cameraTransform.localEulerAngles += Vector3.left;
            _cameraTransform.localEulerAngles += Vector3.up * Random.Range(-1f, 1f);
            yield return null;
        }
    }

    private protected override IEnumerator ReloadAnimation()
    {
        for (var i = 0; i < 720; i += 10)
        {
            _transform.localEulerAngles = Vector3.right * i;
            yield return null;
        }
        _transform.localEulerAngles = Vector3.zero;
    }

    private protected override IEnumerator ShootCoroutine()
    {
        yield return null;
        if (_currentAmmunition > 0 && !_isReloading)
        {
            _currentAmmunition--;

            var rayFromPistol = new Ray(transform.position, transform.forward * 100f);
            if (Physics.Raycast(rayFromPistol, out var hitInfo))
            {
                if (hitInfo.collider.TryGetComponent(out Player player))
                {
                    player.TakeDamage(20);
                }
            }
            if (photonView.IsMine)
            {
                _ui.UpdateAmmo(_currentAmmunition, _clips);
            }
            StartCoroutine(RecoilAnimation());
        }
    }
}