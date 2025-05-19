using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using NUnit.Framework.Constraints;
using System.Collections.Generic;

public class GraphController : MonoBehaviour
{
    private PlayerBot playerBot;
    private GameController gameController;
    private GameObject graphControllerObject;

    private RectTransform cLineTransform;
    public GameObject graphDot;
    public GraphDotBehaviour graphDotBehaviour;
    public Dictionary<int, GraphDotBehaviour> dotDictionary = new Dictionary<int, GraphDotBehaviour>();

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
        maxValue = int.MinValue;
        minValue = int.MaxValue;

        foreach (Transform child in transform)
        {
            var dotComponent = child.GetComponent<GraphDotBehaviour>();
            if (dotComponent != null)
            {
                float value = dotComponent.actualDifference;

                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }
        }

        if(maxValue < 10)
        {
            maxValue = 10;
        }

        if(minValue > -10)
        {
            minValue = -10;
        }

        float range = maxValue - minValue;
        if (Mathf.Approximately(range, 0f)) range = 1f;

        float cLinePosition = Mathf.Lerp(0, 500, (0 - minValue) / range);

        Vector2 pos = cLineTransform.anchoredPosition;
        pos.y = cLinePosition;
        cLineTransform.anchoredPosition = pos;

        GameObject dotObject = Instantiate(graphDot, this.transform);
    }

    public void RegisterDot(GraphDotBehaviour dot)
    {
        dotDictionary[dot.iD] = dot;
    }

    public void RemoveDot(GraphDotBehaviour dot)
    {
        if (dotDictionary.ContainsKey(dot.iD))
            dotDictionary.Remove(dot.iD);
    }
}
