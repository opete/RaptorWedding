using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    public bool isSelected = false;
    public Player player;

    private GameManager gameManager;
    private Renderer rend;

    private float brightness = 0.75f;
    public AudioSource selectSound;

    private void OnMouseEnter()
    {
        isSelected = true;
        selectSound.Play();
    }

    private void OnMouseExit()
    {
        isSelected = false;
    }

    void OnMouseDown()
    {
        if (gameManager.gameState == GameState.Menu)
        {
            if (isSelected)
            {
                gameManager.StartStartGame(player);
            }
        }
    }

    void Update()
    {
        if (gameManager.gameState == GameState.Menu)
        {
            if (isSelected)
            {
                brightness = Mathf.Lerp(brightness, 1.25f, Time.deltaTime * 10f);
            } else
            {
                brightness = Mathf.Lerp(brightness, 0.75f, Time.deltaTime * 10f);
            }

            rend.material.SetFloat("_BrightnessAmount", brightness);

        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rend = GetComponent<Renderer>();
    }

}
