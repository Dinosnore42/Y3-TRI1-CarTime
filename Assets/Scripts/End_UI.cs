using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class End_UI : MonoBehaviour
{
    public Transform template;
    public GameObject raceManager;
    public List<placingData> finalPlacements;

    // Start is called before the first frame update
    void Start()
    {
        finalPlacements = raceManager.GetComponent<RacingManager>().placements;
        float templateHeight = 40f;

        // Order list of final placements based on end combined time
        finalPlacements.Sort((s1, s2) => s1.EndCompareTo(s2));



        for (int i = 0; i < finalPlacements.Count; i++)
        {
            Transform newEntry = Instantiate(template, transform);
            RectTransform newEntryRect = newEntry.GetComponent<RectTransform>();
            newEntryRect.anchoredPosition = new Vector2(0, -templateHeight * (i + 1));

            newEntry.Find("Name").GetComponent<TextMeshProUGUI>().text = finalPlacements[i].car.name;

            float totalTime = 0;
            int j = 0;

            // Add up the time the car took for each lap
            while (j < raceManager.GetComponent<RacingManager>().numOfLapsInRace)
            {
                totalTime += finalPlacements[i].bankedLaptimes[j];
                j++;
            }

            newEntry.Find("Time").GetComponent<TextMeshProUGUI>().text = totalTime.ToString();
            newEntry.Find("Penalty").GetComponent<TextMeshProUGUI>().text = finalPlacements[i].penalty.ToString();
            newEntry.Find("Final Position").GetComponent<TextMeshProUGUI>().text = i + " - " + (totalTime + finalPlacements[i].penalty).ToString();
        }
    }
}
