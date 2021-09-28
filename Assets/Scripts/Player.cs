using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviourPun
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private bool _isOnGround;
    [SerializeField] private float _sensetivity = 2f;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private Material _materialBase;
    [SerializeField] private Material _newMaterial;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _transform;
    [SerializeField] private int _health = 100;
    [SerializeField] private UI _ui;
    [SerializeField] private Spawner _spawner;

    private void Awake()
    {
        _transform = transform;

        _rigidbody = GetComponent<Rigidbody>();

        _ui = FindObjectOfType<UI>();

        _newMaterial = new Material(_materialBase);

        GetComponent<Renderer>().material = _newMaterial;

        _spawner = FindObjectOfType<Spawner>();

        if (photonView.IsMine)
        {
            _ui.UpdateHealth(_health);
        }
        else
        {
            _cameraTransform.GetComponent<Camera>().enabled = false;
        }
       
    }

    [PunRPC]
    public void TakeDamage(int ammount)
    {
        if (_health > 0)
        {
            _health -= ammount;
            StartCoroutine(TakeDamageAnimation());
            if (photonView.IsMine)
            {
                _ui.UpdateHealth(_health);
            }
        }
        else
        {
            _ui.Death();
            _spawner.DespawnPlayer();
        }
    }

    private IEnumerator TakeDamageAnimation()
    {
        _newMaterial.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _newMaterial.color = _materialBase.color;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleInputKeyDowns();
            ChangePlayerRotation();
            ChangePlayerPosition();
        }
    }

    private void ChangePlayerRotation()
    {
        var mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
            );

        _cameraTransform.localEulerAngles += Vector3.right * -mouseDelta.y * _sensetivity;
        _transform.localEulerAngles += Vector3.up * mouseDelta.x * _sensetivity;
    }

    private void ChangePlayerPosition()
    {
        if (_isOnGround)
        {
            var currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _speed * 1.5f : _speed;

            var translation = Input.GetAxis("Vertical") * _transform.forward * currentSpeed;
            var straffe = Input.GetAxis("Horizontal") * _transform.right * currentSpeed / 2f;

            _rigidbody.velocity = translation + straffe;
        }
    }

    private void Jump()
    {
        _isOnGround = false;
        _rigidbody.AddForce(_transform.up * _jumpForce);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Ground ground))
        {
            _isOnGround = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Weapon weapon))
        {
            if (!weapon.InHands)
            {
                _spawner.PickupFromSpawnpoint(weapon);

                if (_weapon != null)
                {
                    PhotonNetwork.Destroy(_weapon.gameObject);
                }

                _weapon = weapon;
                _weapon.ChangeOwnerOfWeapon(_cameraTransform);
            }
        }
        if (other.TryGetComponent(out HealthBooster healthBooster))
        {
            _spawner.PickupFromSpawnpoint(healthBooster);
            _health = 120;
            if (photonView.IsMine)
            {
                _ui.UpdateHealth(_health);
            }
            PhotonNetwork.Destroy(healthBooster.gameObject);
        }
    }

    private void HandleInputKeyDowns()
    {
        if (Input.GetMouseButton(0))
        {
            _weapon.Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _weapon.ChangeView();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _weapon.Reload();
        }

        if (_isOnGround && Input.GetKey(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}