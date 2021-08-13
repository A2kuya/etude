using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerForTitle : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Map");
    }

    public void LoadGame()
    {
        GameManager.Instance.Load("test");
    }
}
