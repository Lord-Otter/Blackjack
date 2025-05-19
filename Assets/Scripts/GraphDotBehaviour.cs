using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework.Constraints;
using System.Collections.Generic;

public class GraphDotBehaviour : MonoBehaviour
{
    public GameObject greenDot;
    public GameObject redDot;
    public GameObject grayDot;

    private GraphController graphController;
    private GameController gameController;
    private PlayerBot playerBot;
    private UnityEngine.UI.Image dotImage;
    private RectTransform dotTransform;
    public GameObject dotLine;
    public RectTransform lineTransform;
    private UnityEngine.UI.Image lineImage;
    public int stepsBeforeDespawn;
    public int dotOffset;
    public int dotStep;
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

        graphController.RegisterDot(this);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedRounds = gameController.currentRound + 1 - iD;

        if(elapsedRounds >= stepsBeforeDespawn)
        {
            Destroy(gameObject);
        }

        float dotYPosition = Mathf.Lerp(0, 500, (differenceAtTheTime + lastBetAtTheTime - graphController.minValue) / (graphController.maxValue - graphController.minValue));
        Vector2 pos = dotTransform.anchoredPosition;
        pos.y = dotYPosition;
        pos.x = 1000 + dotOffset - dotStep * elapsedRounds;
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

        if (graphController.dotDictionary.TryGetValue(iD + 1, out GraphDotBehaviour nextDot))
        {
            RectTransform nextTransform = nextDot.GetComponent<RectTransform>();
            
            // Direction vector in local (UI) space
            Vector2 direction = nextTransform.anchoredPosition - dotTransform.anchoredPosition;

            // Calculate angle (in degrees) using Atan2
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation (Z-axis for 2D UI rotation)
            dotTransform.rotation = Quaternion.Euler(0, 0, angle);
            
            if(gameObject.transform.childCount <= 0 && elapsedRounds > 1)
            {
                GameObject dotObject = Instantiate(dotLine, this.transform);
                lineTransform = dotObject.GetComponent<RectTransform>();
                lineImage = dotObject.GetComponent<UnityEngine.UI.Image>();

                if(dotImage.color == Color.green)
                {
                    Instantiate(greenDot, this.transform);
                }
                else if(dotImage.color == Color.red)
                {
                    Instantiate(redDot, this.transform);
                }
                else
                {
                    Instantiate(grayDot, this.transform);  
                }
                dotImage.enabled = false;
            }

            nextTransform = nextDot.GetComponent<RectTransform>();
            RectTransform thisTransform = GetComponent<RectTransform>();

            // Distance in UI (anchored) space
            float distance = Vector2.Distance(thisTransform.anchoredPosition, nextTransform.anchoredPosition);
            
            float newWidth = distance; // or whatever value you want (e.g., distance between dots)
            Vector2 size = lineTransform.sizeDelta;
            size.x = newWidth;
            lineTransform.sizeDelta = size;
            lineImage.enabled = true;

        }

        
    }

    void OnDestroy()
    {
        if (graphController != null)
        {
            graphController.RemoveDot(this);
        }
    }
}
