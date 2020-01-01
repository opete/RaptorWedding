using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float time;
    private GameManager gameManager;

    /*public float moveTime;
    private float startTime;
    public Vector3 oldPos;
    public Vector3 goalPos;*/
    public float moveTime;
    //public float distance;
    public float speed;

    private void Update()
    {
        transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;

        /*elapsedTime = Time.time - startTime;
        if (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(oldPos, goalPos, (elapsedTime / time));
        } else
        {
            gameManager.lstActiveObstacles.Remove(gameObject);
            GameObject.Destroy(transform.gameObject);
        }*/
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        /*oldPos = gameManager.transObstacleStart.position;
        goalPos = gameManager.transObstacleEnd.position;
        startTime = Time.time;
        elapsedTime = 0;*/
    }
}
