using UnityEngine;

public class GraphController : MonoBehaviour
{
    private PlayerBot playerBot;
    private GameController gameController;
    private GameObject graphControllerObject;

    private RectTransform cLineTransform;
    public GameObject graphDot;

    public float maxValue;
    public float minValue;

    void Awake()
    {
        playerBot = GameObject.Find("PlayerBot").GetComponent<PlayerBot>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        graphControllerObject = GameObject.Find("GraphController");

        cLineTransform = GameObject.Find("CLine").GetComponent<RectTransform>();
    }

    void Start()
    {
        maxValue = 100;
        minValue = -100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGraph()
    {
        if(playerBot.actualDifference > maxValue)
        {
            maxValue = playerBot.actualDifference;
        }

        if(playerBot.actualDifference < minValue)
        {
            minValue = playerBot.actualDifference;
        }

        float cLinePosition = Mathf.Lerp(0, 500, (0 - minValue) / (maxValue - minValue));

        Vector2 pos = cLineTransform.anchoredPosition;
        pos.y = cLinePosition;
        cLineTransform.anchoredPosition = pos;

        GameObject dotObject = Instantiate(graphDot, this.transform);
    }
}
