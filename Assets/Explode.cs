using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private Vector3 originalScale;

    private GameManager gameManager;

    private IEnumerator CoScale(Vector3 goalScale, float time)
    {
        Vector3 oldScale = transform.localScale;
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < time + 0.01f)
        {
            transform.localScale = Vector3.Lerp(oldScale, goalScale, (elapsedTime / time));
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }

        transform.localScale = goalScale;

        yield return new WaitForSeconds(0.2f);

        while (elapsedTime < 0.2f)
        {
            transform.localScale = Vector3.Lerp(goalScale, Vector3.zero, (elapsedTime / time));
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }

        GameObject.Destroy(transform.gameObject);

        yield return null;
    }

    private IEnumerator CoRotate()
    {
        Vector3 localEulerAngles = transform.localEulerAngles;

        while (true)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 2f) * 10f);
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        originalScale = transform.localScale;
        transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);

        StartCoroutine(CoScale(originalScale, 0.2f));
        StartCoroutine(CoRotate());
    }
}
