using System.Collections;
using TMPro;
using UnityEngine;



public class sPlotScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    // When you're a wizard, trapped in the Netherworld, and its demon overlord 
    // discovers which dimensions you've just come from, you have good reason to 
    // wonder how long that dimension has left before said demon transforms 
    // it into an ornament for this throne room.
    [TextArea]
    [SerializeField] private string plotExposition = "When you're a wizard, trapped in the Netherworld, " +
        "and its demon overlord discovers which dimensions you've just come from, you have good reason " +
        "to wonder how long that dimension has left before said demon transforms it into an ornament for his " +
        "throne room. \n\n\n Press [Space] To Continue";

    private void Start()
    {
        StartCoroutine(TypeWriterCoroutine(text, plotExposition, 0.05f));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Scene 2 is Level_1
            SessionManager.instance.LoadLevel(2);
        }
    }

    IEnumerator TypeWriterCoroutine(TextMeshProUGUI text, string stringToDisplay, float delayBetweenCharacters)
    {
        // Cache the yield instruction for GC optimization
        WaitForSeconds _delayBetweenCharactersYieldInstruction = new WaitForSeconds(delayBetweenCharacters);

        // Iterating(looping) through the string's characters
        for (int i = 0; i < stringToDisplay.Length; i++)
        {
            // Retrieves part of the text from string[0] to string[i]
            text.text = stringToDisplay.Substring(0, i);

            // We wait x seconds between characters before displaying them
            yield return _delayBetweenCharactersYieldInstruction;
        }
    }
}
