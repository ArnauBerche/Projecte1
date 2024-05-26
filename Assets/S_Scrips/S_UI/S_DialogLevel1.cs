using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_DialogLevel1 : MonoBehaviour
{

    [SerializeField] private S_CMovement mainCharacter;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(2,4)] private string[] dialogueLines;

    private float typingTime;
    private float defaultTypingTime = 0.05f;
    public bool Cinematic;

    
    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    [SerializeField] private int[] lineFontSize;
    [SerializeField] private int[] maxTimeOnScreen;
    [SerializeField] private Sprite[] IMGToShow;
    [SerializeField] private bool[] youTalking;

    public GameObject YouOBJ;
    public GameObject NPCOBJ;    

    public Image You;
    public Image NPC;

    public float currentTimeOnScreen;

    void Awake()
    {
        typingTime = defaultTypingTime;
        if(!Cinematic)
        {
            mainCharacter = GameObject.Find("MainCharacter").GetComponent<S_CMovement>();
        }
    }

    void Update()
    {
        if (isPlayerInRange)
        {
            currentTimeOnScreen += Time.deltaTime;
            SetLineSize();
            SetIMG();
            if(!didDialogueStart)
            {
                StartDialogue();
            }
            else if((dialogueText.text == dialogueLines[lineIndex] && (Input.GetKeyDown("space")) && !Cinematic || (currentTimeOnScreen >= maxTimeOnScreen[lineIndex]) && maxTimeOnScreen[lineIndex] != 0 ))
            {
                typingTime = defaultTypingTime;
                NextDialogueLine();
            }
            else if ((dialogueText.text != dialogueLines[lineIndex] && Input.GetKeyDown("space")) && !Cinematic)
            {
                typingTime = 0;
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
        DisableMovement();
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        currentTimeOnScreen = 0;
        lineIndex++;
        if(lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart=false;
            dialoguePanel.SetActive(false);
            EnableMovement();
            YouOBJ.SetActive(false);
            NPCOBJ.SetActive(false);
        }
    }

    private void SetLineSize()
    {
        dialogueText.fontSize = lineFontSize[lineIndex];
    }
    
    private void SetIMG()
    {
        SetShowGameObject();
        if(youTalking[lineIndex])
        {
            You.sprite = IMGToShow[lineIndex];
        }
        else
        {
            NPC.sprite = IMGToShow[lineIndex];
        }

    }
    private void SetShowGameObject()
    {
        if(youTalking[lineIndex])
        {
            YouOBJ.SetActive(true);
            NPCOBJ.SetActive(false);
        }
        else
        {
            YouOBJ.SetActive(false);
            NPCOBJ.SetActive(true);
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

    public void DisableMovement() 
    {
        if(!Cinematic)
        {
            mainCharacter.movementEnabled = false;
        }
    }

    public void EnableMovement()
    {
        if(!Cinematic)
        {
            mainCharacter.movementEnabled = true;
        }
    }

}
