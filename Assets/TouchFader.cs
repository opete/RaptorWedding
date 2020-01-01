using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchFader : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    public KeyCode keyCode;

    private Image image;
    private GameManager gameManager;

    IEnumerator CoFade()
    {
        float fadeTime = 0.25f;
        float startTime = Time.time;
        float elapsedTime = 0;
        float oldAlpha = image.color.a;

        while (elapsedTime < fadeTime)
        {
            elapsedTime = Time.time - startTime;
            image.color = new Color(1, 1, 1, Mathf.Lerp(oldAlpha, 0f, elapsedTime / fadeTime));
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }
        image.color = new Color(1, 1, 1, 0.1f);
        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager.gameState == GameState.Game)
        {
            image.color = new Color(1, 1, 1, 0.25f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameManager.gameState == GameState.Game)
        {
            StopAllCoroutines();
            image.color = new Color(1, 1, 1, 0.25f);
            StartCoroutine(CoFade());
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameManager.gameState == GameState.Game)
        {
            StopAllCoroutines();
            gameManager.ProcessKeyStroke(keyCode);
            image.color = new Color(1, 1, 1, 1f);
            StartCoroutine(CoFade());
        }
    }

    void Start()
    {
        image = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>();
    }
}
