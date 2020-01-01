using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    opete,
    lolla
}

public class PlayerCharacter : MonoBehaviour
{
    public bool isEnabled = true;
    public Player player;

    public float goalX = 0f;

    private GameManager gameManager;

    IEnumerator CoMoveSine()
    {
        while (true)
        {
            if (isEnabled)
            {
                float newGoalX = Mathf.Sin(Time.time) / 10f + goalX;
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, newGoalX, Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
            }
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }
    }

    public IEnumerator CoScaleSine(float time, int sign)
    {
        float oldScaleX = transform.localScale.x;
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            float sine = Mathf.Sin(elapsedTime * 2f * Mathf.PI);
            transform.localScale = Vector3.one * (oldScaleX + sine * sign / 6f);
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }

        transform.localScale = Vector3.one * oldScaleX;

        yield return null;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(CoMoveSine());
    }
}
