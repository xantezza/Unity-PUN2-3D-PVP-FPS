using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _ammoLabel;
    [SerializeField] private Text _health;
    [SerializeField] private Text _killsDeathsStat;

    private int _deaths = 0;
    private int _kills = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateHealth(int currentHealth)
    {
        _health.text = currentHealth.ToString();
    }

    public void UpdateAmmo(int bullets, int clips)
    {
        _ammoLabel.text = $"{bullets}/{clips}";
    }

    public void Death()
    {
        _deaths++;
        UpdateKillsDeathsStat();
    }

    public void Kill()
    {
        _kills++;
        UpdateKillsDeathsStat();
    }

    private void UpdateKillsDeathsStat()
    {
        _killsDeathsStat.text = $"{ _kills / _deaths}";
    }
}