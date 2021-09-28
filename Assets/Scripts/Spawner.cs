using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviourPun
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject[] _weaponsPrefabs;
    [SerializeField] private GameObject[] _healthBoostersPrefabs;

    private Dictionary<Transform, GameObject> _presenceOfAnObjectInSpawnPoint = new Dictionary<Transform, GameObject>();

    private GameObject _player;
    private bool _isSpawning = true;
    private void Start()
    {
        var _spawnPoints = GetComponentsInChildren<Transform>();

        foreach (var spawnPoint in _spawnPoints)
        {
            _presenceOfAnObjectInSpawnPoint.Add(spawnPoint, null);
        }
        SpawnPlayer();
        StartCoroutine(SpawnAmmunitionRandomly());
    }
    
    private IEnumerator SpawnAmmunitionRandomly()
    {
        while (_isSpawning)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameObject[] prefabs;
                prefabs = Random.value > 0.5f ? _weaponsPrefabs : _healthBoostersPrefabs;
                var prefab = prefabs[Random.Range(0, prefabs.Length)];
                SpawnOnEmptyOrRandom(prefab);
            }

            yield return new WaitForSeconds(5);
        }
    }


    public void PickupFromSpawnpoint(Ammunition ammunition)
    {
        var ammunitionTransform = _presenceOfAnObjectInSpawnPoint.FirstOrDefault(x => x.Value == ammunition.gameObject).Key;
        if (ammunitionTransform != null)
        {
            _presenceOfAnObjectInSpawnPoint[ammunitionTransform] = null;
        }
    }

    private Vector3 SpawnOnEmptyOrRandom(GameObject prefabToSpawn)
    {
        var randomizedPositions = _presenceOfAnObjectInSpawnPoint.OrderBy(x => Random.value);
        var targetTransform = randomizedPositions.FirstOrDefault(s => s.Value == null).Key;
        if (targetTransform == null)
        {
            targetTransform = randomizedPositions.ToArray()[0].Key;
            PhotonNetwork.Destroy(_presenceOfAnObjectInSpawnPoint[targetTransform].gameObject);
        }
        _presenceOfAnObjectInSpawnPoint[targetTransform] = PhotonNetwork.Instantiate(prefabToSpawn.name, targetTransform.position, Quaternion.identity);
        return targetTransform.position;
    }

    public void SpawnPlayer()
    {
        var preparedPosition = SpawnOnEmptyOrRandom(_weaponsPrefabs[Random.Range(0, _weaponsPrefabs.Length)]);
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, preparedPosition, Quaternion.identity);
    }

    public void DespawnPlayer()
    {
        PhotonNetwork.Destroy(_player);
        SpawnPlayer();
    }
    
}