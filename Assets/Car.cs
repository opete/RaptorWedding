using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Vector3 goal;

    public Vector3 start;
    public Vector3 end;
    public Animator animator;
    private GameManager gameManager;
    private float smoothTime = 20f;

    private void Update()
    {
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, goal + new Vector3(0, Mathf.Sin(Time.time) / 20f, 0), ref velocity, smoothTime * Time.deltaTime);
    }

    public void Initialize()
    {
        goal = start;
        transform.position = goal;
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        start = new Vector3(2f, -0.36f, -7);
        end = new Vector3(0.5f, -0.36f, -7);
        Initialize();
    }
}
