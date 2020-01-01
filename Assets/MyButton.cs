using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public enum ButtonTypes
{
    language,
    endless,
    hardcore,
    quality
}

public class MyButton : MonoBehaviour
{
    public bool isSelected = false;
    public ButtonTypes buttonType;

    public SpriteRenderer otherFlag;

    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private PostProcessLayer postProcessLayer;

    public int buttonNo;
    public AudioSource selectSound;
    public TextMeshPro txt;

    void RefreshText()
    {
        txt.text = "";
        switch (buttonType)
        {
            case ButtonTypes.endless:
                txt.text += gameManager.lstString[10] + " ";
                if (gameManager.endlessMode)
                {
                    txt.text += gameManager.lstString[14];
                }
                else
                {
                    txt.text += gameManager.lstString[15];
                }
                break;
            case ButtonTypes.hardcore:
                txt.text += gameManager.lstString[11] + " ";
                if (gameManager.hardMode)
                {
                    txt.text += gameManager.lstString[14];
                }
                else
                {
                    txt.text += gameManager.lstString[15];
                }
                break;
            case ButtonTypes.language:
                txt.text += gameManager.lstString[12] + " ";
                if (gameManager.hunLang)
                {
                    txt.text += "magyar";
                }
                else
                {
                    txt.text += "English";
                }
                break;
            case ButtonTypes.quality:
                txt.text += gameManager.lstString[13] + " ";
                if (postProcessLayer.enabled)
                {
                    txt.text += gameManager.lstString[14];
                }
                else
                {
                    txt.text += gameManager.lstString[15];
                }
                break;

        }
    }

    public void OnMouseEnter()
    {
        if (gameManager.gameState == GameState.Menu || gameManager.gameState == GameState.Intro)
        {           
            isSelected = true;
            RefreshText();
            selectSound.Play();
            if (spriteRenderer.color.a > 0.1f)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.9f);
            }
        }
    }

    public void OnMouseExit()
    {
        if (gameManager.gameState == GameState.Menu || gameManager.gameState == GameState.Intro)
        {
            isSelected = false;
            txt.text = "";
            if (spriteRenderer.color.a > 0.1f && spriteRenderer.color.a < 1f)
            {
                if ((buttonType == ButtonTypes.quality && postProcessLayer.enabled) ||
                    (buttonType == ButtonTypes.endless && gameManager.endlessMode) ||
                    (buttonType == ButtonTypes.hardcore && gameManager.hardMode))
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                } else
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
                }
            }
        }
    }

    public void OnMouseDown()
    {
        if (gameManager.gameState == GameState.Menu || gameManager.gameState == GameState.Intro)
        {
            if (isSelected)
            {
                switch (buttonType)
                {
                    case ButtonTypes.language:
                        gameManager.hunLang = !gameManager.hunLang;
                        gameManager.ChangeLanguage(gameManager.hunLang);
                        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
                        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
                        if ((gameManager.hunLang && gameObject.name == "btnHun") || (!gameManager.hunLang && gameObject.name != "btnHun"))
                        {
                            otherFlag.gameObject.SetActive(false);
                            gameObject.SetActive(true);
                        }
                        else
                        {
                            otherFlag.gameObject.SetActive(true);
                            gameObject.SetActive(false);
                        }
                        break;
                    case ButtonTypes.endless:
                        gameManager.endlessMode = !gameManager.endlessMode;
                        if (gameManager.endlessMode)
                        {
                            spriteRenderer.color = Color.white;
                        }
                        else
                        {
                            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                        }
                        break;
                    case ButtonTypes.hardcore:
                        gameManager.hardMode = !gameManager.hardMode;
                        if (gameManager.hardMode)
                        {
                            spriteRenderer.color = Color.white;
                        }
                        else
                        { 
                            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                        }
                        break;
                    case ButtonTypes.quality:
                        postProcessLayer.enabled = !postProcessLayer.enabled;
                        if (postProcessLayer.enabled)
                        {
                            spriteRenderer.color = Color.white;
                        }
                        else
                        {
                            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                        }
                        break;
                }
            }
        }
        RefreshText();
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
    }
}
