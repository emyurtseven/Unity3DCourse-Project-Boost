using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    
    [SerializeField] float textFadeDistance = 2f;
    [Tooltip("Higher number means slower fade")]
    [SerializeField] float textFadeRate = 3f;


    float timer;
    float distanceModifier;

    List<GameObject> tutorialTips = new List<GameObject>();
    GameObject player;

    void Start()
    {
        foreach (Transform child in this.transform)
        {
            tutorialTips.Add(child.gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FadeTutorialTips());
    }

    IEnumerator FadeTutorialTips()
    {
        while (true)
        {
            foreach (GameObject tutorialTip in tutorialTips)
            {
                float distance = Vector2.Distance(tutorialTip.GetComponent<RectTransform>().position, player.transform.position);

                distanceModifier = (distance - textFadeDistance) / textFadeRate;
                float transparencyValue = 1 - distanceModifier;
                tutorialTip.gameObject.GetComponent<CanvasGroup>().alpha = transparencyValue;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}

