using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoseUI : MonoBehaviour
{
    public GameObject container;
    void Start()
    {
        Guard.OnGuardSpottedPlayer += Guard_OnPlayerDetected;
    }
    void Guard_OnPlayerDetected()
    {
        container.SetActive(true);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable()
    {
        Guard.OnGuardSpottedPlayer -= Guard_OnPlayerDetected;

    }
}
