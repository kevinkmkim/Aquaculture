using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyTaskTab : MonoBehaviour
{
    [SerializeField]
    private String[] taskDescriptions;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button previousButton;

    [SerializeField]
    private TextMeshProUGUI taskDescriptionText;
    private int currentTaskIndex = 0;

    private void Start()
    {
        if (taskDescriptions.Length == 0)
        {
            Debug.LogWarning("No task descriptions provided.");
            return;
        }

        UpdateTaskDescription();

        nextButton.onClick.AddListener(NextTask);
        previousButton.onClick.AddListener(PreviousTask);
    }

    private void UpdateTaskDescription()
    {
        taskDescriptionText.text = taskDescriptions[currentTaskIndex];

        if (currentTaskIndex == 0)
            previousButton.interactable = false;
        else
            previousButton.interactable = true;

        if (currentTaskIndex == taskDescriptions.Length - 1)
            nextButton.interactable = false;
        else
            nextButton.interactable = true;
    }

    private void NextTask()
    {
        currentTaskIndex++;
        if (currentTaskIndex >= taskDescriptions.Length)
        {
            currentTaskIndex = 0;
        }
        UpdateTaskDescription();
    }

    private void PreviousTask()
    {
        currentTaskIndex--;
        if (currentTaskIndex < 0)
        {
            currentTaskIndex = taskDescriptions.Length - 1;
        }
        UpdateTaskDescription();
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(NextTask);
        previousButton.onClick.RemoveListener(PreviousTask);
    }
}
