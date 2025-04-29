using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework.Constraints;

public class GraphDotBehaviour : MonoBehaviour
{
    private GraphController graphController;
    private GameController gameController;
    private PlayerBot playerBot;
    private UnityEngine.UI.Image dotImage;
    private RectTransform dotTransform;

    public int iD;
    public int elapsedRounds = 0;
    [HideInInspector] public int differenceAtTheTime;
    [HideInInspector] public int lastBetAtTheTime;
    public int actualDifference;

    public float maxValue;
    public float minValue;


    void Awake()
    {
        graphController = GameObject.Find("GraphController").GetComponent<GraphController>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        playerBot = GameObject.Find("PlayerBot").GetComponent<PlayerBot>();

        dotTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        iD = gameController.currentRound;dotImage = GetComponent<UnityEngine.UI.Image>();
        differenceAtTheTime = playerBot.difference;
        lastBetAtTheTime = playerBot.lastBet;
        actualDifference = differenceAtTheTime + lastBetAtTheTime;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedRounds = gameController.currentRound + 1 - iD;

        if(elapsedRounds >= 27)
        {
            Destroy(gameObject);
        }

        float dotYPosition = Mathf.Lerp(0, 500, (differenceAtTheTime + lastBetAtTheTime - graphController.minValue) / (graphController.maxValue - graphController.minValue));
        Vector2 pos = dotTransform.anchoredPosition;
        pos.y = dotYPosition;
        pos.x = 1040 - 40 * elapsedRounds;
        dotTransform.anchoredPosition = pos;
        



        if(differenceAtTheTime + lastBetAtTheTime > 0)
        {
            dotImage.color = Color.green;
        }
        else if(differenceAtTheTime + lastBetAtTheTime < 0)
        {
            dotImage.color = Color.red;
        }
        else
        {
            dotImage.color = Color.gray;
        }
        dotImage.enabled = true;
    }

    /*void UpdateMinMaxValues() // Try this
    {
        int childCount = transform.childCount;

        if (childCount == 0)
        {
            maxValue = 0;
            minValue = 0;
            return;
        }

        // Initialize with the first child's actualDifference
        GraphDotBehaviour firstDot = transform.GetChild(0).GetComponent<GraphDotBehaviour>();
        maxValue = firstDot.actualDifference;
        minValue = firstDot.actualDifference;

        // Loop through all children
        for (int i = 1; i < childCount; i++)
        {
            GraphDotBehaviour dot = transform.GetChild(i).GetComponent<GraphDotBehaviour>();
            int diff = dot.actualDifference;

            if (diff > maxValue) maxValue = diff;
            if (diff < minValue) minValue = diff;
        }
    }*/
}
