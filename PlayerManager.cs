using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class PlayerManager : MonoBehaviour
{
    public Transform player;
    private int numberOfStickmans;
    [SerializeField] private TextMeshPro CounterText;

    [SerializeField] private GameObject stickMan;

    //**************************************

    [Range(0f, 1f)] [SerializeField] private float DistanceFactor, Radius;

    //******** move the player ************

    public bool moveByTouch, gameState;
    private Vector3 mouseStartPos, playerStartPos;
    public float playerSpeed, roadSpeed;
    public Camera myCamera;

    [SerializeField] private Transform road;

    private void Awake()
    {
        // Initialize DOTween
        DOTween.Init(false); // 'false' here refers to useSafeMode: False

        // Set recycling and logBehaviour options separately
        DOTween.defaultRecyclable = false; // Equivalent to recycling: OFF
        DOTween.defaultAutoPlay = AutoPlay.All; // You can set AutoPlay options if needed
        DOTween.logBehaviour = LogBehaviour.Verbose;
    }
    private void Start()
    {
        player = transform;

        numberOfStickmans = transform.childCount - 1;
        CounterText.text = numberOfStickmans.ToString();

        
    }

    private void Update()
    {
        MoveThePlayer();
    }

    void MoveThePlayer()
    {
        if(Input.GetMouseButtonDown(0) && gameState)
        {
            moveByTouch = true;
            var plane = new Plane(Vector3.up, 0f);
            var ray = myCamera.ScreenPointToRay(Input.mousePosition);

            if(plane.Raycast(ray, out var distance))
            {
                mouseStartPos = ray.GetPoint(distance + 1f);
                playerStartPos = transform.position;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            moveByTouch = false;
        }

        if(moveByTouch)
        {
            var plane = new Plane(Vector3.up, 0f);
            var ray = myCamera.ScreenPointToRay(Input.mousePosition);

            if(plane.Raycast(ray, out var distance))
            {
                var mousePos = ray.GetPoint(distance + 1f);
                var move = mousePos - mouseStartPos;
                var control = playerStartPos + move;

                control.x = Mathf.Clamp(control.x, -1.1f, 1.1f);

             transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * playerSpeed), 
             transform.position.y, transform.position.z);
            }
        }

        if(gameState)
        {
            road.Translate(road.forward * Time.deltaTime * roadSpeed);
        }
    }

    private void FormatStickMan()
    {
        for(int i = 1; i < player.childCount; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var NewPos = new Vector3(x, -2.02f, z);

            player.transform.GetChild(i).DOLocalMove(NewPos, 1f).SetEase(Ease.OutBack);
        }
    }

    private void MakeStickMan(int number)
    {
        for(int i = 0; i < number; i++)
        {
            Instantiate(stickMan, transform.position, Quaternion.identity, transform);
        }

        numberOfStickmans = transform.childCount - 1;

        CounterText.text = numberOfStickmans.ToString();

        FormatStickMan();


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Gate"))
        {
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false;//gate1
            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false;//gate2

            var gateManager = other.GetComponent<GateManager>();

            if(gateManager.multiply)
            {
                MakeStickMan(numberOfStickmans * gateManager.randomNumber);
            }
            else
            {
                MakeStickMan(numberOfStickmans + gateManager.randomNumber);
            }
        }
    }
}
