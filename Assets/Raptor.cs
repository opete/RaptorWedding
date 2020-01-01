using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raptor : MonoBehaviour
{
    private GameManager gameManager;
    public Animator animator;

    public float goalX = -1f;

    IEnumerator CoMoveSine()
    {
        while (true)
        {
            float newGoalX = Mathf.Sin(Time.time) / 10f + goalX;
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, newGoalX, Time.deltaTime), -0.54f, 0);

            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }
    }

    IEnumerator CoComeIn(float time)
    {
        Vector3 oldPos = new Vector3(-2, -0.54f, 0);
        Vector3 goalPos = new Vector3(-0.87f, -0.54f, 0);
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(oldPos, goalPos, (elapsedTime / time));
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(gameManager.coRoutineInc);
        }

        StartCoroutine(CoMoveSine());
        yield return null;
    }


    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        transform.localPosition = new Vector3(-2, -0.54f, 0);
        //StartCoroutine(CoComeIn(2f));
        StartCoroutine(CoMoveSine());
    }
}
