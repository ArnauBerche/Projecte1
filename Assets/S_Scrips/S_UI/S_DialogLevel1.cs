using System.Collections;
using UnityEngine;
using TMPro;

public class S_DialogLevel1 : MonoBehaviour
{

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(2,4)] private string[] dialogueLines;

    private float typingTime = 0.05f;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;



    void Update()
    {
       if (isPlayerInRange)
        {
            if(!didDialogueStart)
            {
                StartDialogue();
            }
            else if(dialogueText.text == dialogueLines[lineIndex] && Input.GetKeyDown("o"))
            {
                NextDialogueLine();
            }
        }
        if (lineIndex == dialogueLines.Length) 
        {
            this.enabled = false;
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true); 
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if(lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart=false;
            dialoguePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }



    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        
        foreach (char ch in dialogueLines[lineIndex]) 
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Se puede iniciar un dialogo");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("No se puede iniciar un dialogo");
        }         
    }
}
