using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TargetState
{
    none,
    moving,
    scaling,
    destroy
}

public class KeyStroke : MonoBehaviour
{
    public bool isEnabled = true;

    public TargetState targetState;
    public int currentlyAtTarget = -1;
    public KeyCode keyCode;
    public float value;

    public float moveTime;
    private float scaleSize = 2f;
    private float scaleTime = 0.25f;

    private Image image;
    private GameManager gameManager;
    private RectTransform rectTrans;
    private Color greenColor = new Color(0.25f, 1, 0.25f);
    private Color redColor = new Color(1, 0.25f, 0.25f);

    private float startTime;
    public Vector3 oldPos;
    public Vector3 goalPos;
    public float speed;

    private Vector3 oldScale;
    private Vector3 goalScale;
    private Color oldColor;
    private Color goalColor;

    private void Update()
    {
        float elapsedTime = Time.time - startTime;
        switch (targetState) {
            case TargetState.moving:
                //transform.position = Vector3.Lerp(oldPos, goalPos, (elapsedTime / moveTime));
                transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;

                if (isEnabled)
                {
                    for (int i = 0; i < gameManager.lstRectGoals.Count; i++)
                    {
                        if ((rectTrans.localPosition.x < (gameManager.lstRectGoals[i].localPosition.x + gameManager.lstRectGoals[i].localScale.x * gameManager.lstRectGoals[i].sizeDelta.x / 2f)) &&
                            (rectTrans.localPosition.x > (gameManager.lstRectGoals[i].localPosition.x - gameManager.lstRectGoals[i].localScale.x * gameManager.lstRectGoals[i].sizeDelta.x / 2f)))
                        {
                            currentlyAtTarget = i;
                            if (currentlyAtTarget == gameManager.lstRectGoals.Count - 1)
                            {
                                Disable();
                                StartCoroutine(gameManager.CoCreateExplosion(4));
                            }
                            break;
                        }
                    }
                }
                break;
            case TargetState.scaling:
                if (elapsedTime < moveTime)
                {
                    transform.localScale = Vector3.Lerp(oldScale, goalScale, (elapsedTime / scaleTime));
                    image.color = Color.Lerp(oldColor, goalColor, (elapsedTime / scaleTime));
                } else
                {
                    targetState = TargetState.destroy;
                }
                break;
            case TargetState.destroy:
                gameManager.lstActiveTargets.Remove(gameObject);
                GameObject.Destroy(transform.gameObject);
                break;
        }
    }

    void StartScale()
    {
        targetState = TargetState.scaling;
        startTime = Time.time;
    }

     public void Press()
     {
        if (isEnabled)
        {
            isEnabled = false;
            StartScale();

            switch (currentlyAtTarget)
            {
                case 0: // too early
                    Disable();
                    StartCoroutine(gameManager.CoCreateExplosion(3));
                    break;
                case 1: // ok
                case 5:
                    StartCoroutine(gameManager.CoCreateExplosion(0));
                    break;
                case 2: // great
                case 4:
                    StartCoroutine(gameManager.CoCreateExplosion(1));
                    break;
                case 3: // perfect
                    StartCoroutine(gameManager.CoCreateExplosion(2));
                    break;
            }
        }
    }

    public void Disable()
    {
        image.color = redColor;
        isEnabled = false;
    }

    private void Start()
    {
        image = GetComponent<Image>();
        gameManager = FindObjectOfType<GameManager>();
        rectTrans = GetComponent<RectTransform>();

        image.enabled = !gameManager.hardMode;

        startTime = Time.time;
        oldScale = transform.localScale;
        goalScale = Vector3.one * scaleSize;
        oldColor = image.color;
        goalColor = Color.clear;

        /*oldPos = transform.position;
        goalPos = new Vector3(gameManager.transTargetGoal.position.x, transform.position.y, transform.position.z);
        float distance = Vector3.Distance(oldPos, goalPos);
        speed = -distance / moveTime;*/

        targetState = TargetState.moving;
    }    
}
