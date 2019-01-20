using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct UserQuestion
{
    public enum CorrectAnswer
    {
        A,
        B,
        C,
        D
    }

    public string question;
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    
    public CorrectAnswer correctAnswer;
}

[System.Serializable]
public struct MastermindGame
{
    public enum ColorOption
    {
        G,
        R,
        B,
        Y,
        Count
    }


    public ColorOption[] code;
    public int numGuessesRemaining;
}


public class ConsoleInput : MonoBehaviour {
    [Header("Disinfection Questions")]
    [SerializeField]
    UserQuestion[] userQuestions;

    UserQuestion currentQuestion;

    [Space]
    [Header("Mastermind Variables")]
    [SerializeField]
    MastermindGame mastermindGame;
    [SerializeField]
    int codeLength = 3;
    [SerializeField]
    int numGuesses = 10;
    [SerializeField]
    bool useArray = true;
    [SerializeField]
    char arrayStarter = '[';
    [SerializeField]
    char arrayFinisher = ']';
    [SerializeField]
    string mastermindQuitCommand = "quit";

    [Space]
    [Header("Messages")]
    public string scanOpeningMessage = "Disinfection commencing...  Antivirus tamper!";
    public string disInfectingPieceMessage = "Disinfected piece ";
    public string disInfectingWrongAnswerMessage = "Wrong answer! VIRUS STILL INTACT";
    public string uninfectedPieceMessage = "Piece is not infected: ";
    public string mastermindWrongSyntaxMessage = "Wrong syntax for code injection";
    public string mastermindInvalidCodeLengthMessage = "Array of wrong size for successful code injection";
    public string mastermindQuitMessage = "Quitting injection process";
    public string beginCodeInjectionMessage = "Security hole located, inject code array:";
    public string repairingPieceMessage = "Repairing piece ";
    [Space]
    [Header("UI attributes and inputs")]

    public GameObject UIParent;
    public Transform singleMonitorLocation;

    public DisplayScript displayScript;

    public Text[] textFields;

    public InputField userInput;
    public Text userInputTextField;

    public float timeBetweenFlashes = 0.5f;
    public float timeToFlash = 0.2f;
    bool inputIndicatorIsVisible = true;
    float cursorTimer;

    public char delimiter = ' ';
    

    [Space]
    [Header("Mech Pieces")]
    public MechPiece[] mechPieces;
    MechPiece currentPiece;

    

    enum ConsoleStates
    {
        normal,
        processing,
        repair,
        scan,
        delete,
        mastermind
    }

    ConsoleStates state;

    [Space]
    [Header("User Commands")]
    string repair = "repair";
    string scan = "scan";
    string disinfect = "disinfect";
    string delete = "delete";

    [Space]
    [Header("Console Messages")]
    public string semicolonError = "Command not ended properly (;)";
    public string invalidPieceError = "is not a valid piece name.";
    public string invalidCommandError = "Not a valid command: ";

    // Use this for initialization
    void Start() {
        //if (!displayScript.isMultiMonitor)
        //{
            UIParent.transform.position = singleMonitorLocation.position;
        //}

        textFields[0].text = "Initiating hyperdrive...";
        textFields[1].text = "Vehicle online....";
        textFields[2].text = "Weapons online.... Welcome Commander ;)";

        userInput.Select();
        userInputTextField.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    // Update is called once per frame
    void Update() {

        FlashCursor();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitUserConsoleInput(userInput.text);
        }




    }

    void DisplayNewMessage(string newMessage)
    {
        for(int i =0; i<textFields.Length-1; i++)
        {
            textFields[i].text = textFields[i + 1].text;
        }
        textFields[textFields.Length-1].text = newMessage;
    }

    void SubmitUserConsoleInput(string userCommand)
    {
        DisplayNewMessage(userCommand);

        userInput.text = "";
        userInput.ActivateInputField();
        userInput.Select();

        ParseInput(userCommand);
    }

    void FlashCursor()
    {

        if (userInput.text == "")
        {
            if (inputIndicatorIsVisible)
            {
                if (Time.time - cursorTimer > timeBetweenFlashes)
                {
                    userInput.placeholder.GetComponent<Text>().text = "";
                    cursorTimer = Time.time;
                    inputIndicatorIsVisible = false;
                }
            }
            else
            {
                if (Time.time - cursorTimer > timeToFlash)
                {
                    userInput.placeholder.GetComponent<Text>().text = ">";
                    //userInputTextField.text = userInput.text;
                    cursorTimer = Time.time;
                    inputIndicatorIsVisible = true;
                }
            }
        }
    }

    void ParseInput(string userCommand)
    {
        if (state == ConsoleStates.scan) {
            ScanUserInput(userCommand);
        } else if (state == ConsoleStates.mastermind) {
            MastermindUserInput(userCommand);
        } else if (userCommand == "") {

        } else {
            string[] words = userCommand.Split(delimiter);
            string finalWord = words[words.Length - 1];
            char finalWordFinalChar = finalWord[finalWord.Length - 1];

            if (finalWordFinalChar == ';')
            {
                state = ConsoleStates.processing;
                ProcessInput(words);
            }
            else
            {
                ShowInputError();
            }
        }
    }

    void ShowInputError()
    {
        DisplayNewMessage(semicolonError);
        state = ConsoleStates.normal;
    }

    void InvalidPieceError(string pieceName)
    {
        DisplayNewMessage(pieceName + invalidPieceError);
        state = ConsoleStates.normal;
    }

    void InvalidCommandError(string command)
    {
        DisplayNewMessage(invalidCommandError + command);
        state = ConsoleStates.normal;
    }

    void ProcessInput(string[] userInputs)
    {
        string firstString = userInputs[0];

        if (firstString.ToLower() == repair)
        {
            ProcessRepair(userInputs);
        } else if (firstString.ToLower() == scan)
        {
            ProcessScan(userInputs);
        }
        else
        {
            InvalidCommandError(userInputs[0]);
        }
    }

    void ProcessRepair(string[] userInputs)
    {
        string pieceName = userInputs[1];
        pieceName = pieceName.Substring(0, pieceName.Length -1);

        bool foundValidPiece = false;
        MechPiece piece = null;
        foreach(MechPiece mechPiece in mechPieces)
        {
            if(mechPiece.pieceName == pieceName)
            {
                foundValidPiece = true;
                piece = mechPiece;
            }
        }

        if (foundValidPiece)
        {
            BeginRepair(piece);
        }
        else
        {
            InvalidPieceError(pieceName);
        }
    }

    void ProcessScan(string[] userInputs)
    {
        string pieceName = userInputs[1];
        pieceName = pieceName.Substring(0, pieceName.Length - 1);

        bool foundValidPiece = false;
        MechPiece piece = null;
        foreach (MechPiece mechPiece in mechPieces)
        {
            if (mechPiece.pieceName == pieceName)
            {
                foundValidPiece = true;
                piece = mechPiece;
            }
        }

        if (foundValidPiece)
        {
            DisplayNewMessage("Found a piece: " + piece.pieceName);
            ScanPiece(piece);
        }
        else
        {
            InvalidPieceError(pieceName);
        }
    }

    void ScanPiece(MechPiece piece)
    {
        if (piece.isInfected)
        {
            currentPiece = piece;
            //BeginScan(); // Deperecated
            BeginMastermind();
            
        } else
        {
            UninfectedPiece(piece);
        }

    }

    void BeginScan()
    {
        state = ConsoleStates.scan;
        currentQuestion = userQuestions[Random.Range(0, userQuestions.Length)];

        DisplayNewMessage(scanOpeningMessage);
        DisplayNewMessage("");

        DisplayNewMessage(currentQuestion.question);
        DisplayNewMessage("");
        DisplayNewMessage("a) " + currentQuestion.answerA);
        DisplayNewMessage("b) " + currentQuestion.answerB);
        DisplayNewMessage("c) " + currentQuestion.answerC);
        DisplayNewMessage("d) " + currentQuestion.answerD);
    }

    void BeginMastermind()
    {
        state = ConsoleStates.mastermind;
        DisplayNewMessage(scanOpeningMessage);
        mastermindGame = MakeNewMastermindGame();
        DisplayNewMessage(beginCodeInjectionMessage);
        ShowCodeLength();
    }

    void BeginRepair(MechPiece pieceToRepair)
    {
        DisplayNewMessage(repairingPieceMessage + pieceToRepair.pieceName);
        foreach(MechPiece piece in mechPieces)
        {
            piece.StopHealing();
        }
        pieceToRepair.BeginHealing();
    }

    void MastermindUserInput(string userCommand)
    {
        if(userCommand == mastermindQuitCommand + ";")
        {
            QuitMasterMind();
        } else
        {

            string input = userCommand;
            bool isValid = true;

            input = input.ToUpper();

            //Don't care about spaces/white space
            input = input.Replace(" ", "");

            //Don't care if it has a semicolon or not
            if (input[input.Length - 1] == ';')
            {
                input = input.Substring(0, input.Length - 1);
            }

            //If using an array, check that they open and close the array properly
            if (useArray)
            {
                if(input[0] != arrayStarter && input[input.Length - 1] != arrayFinisher)
                {
                    isValid = false;
                    WrongSyntaxMastermind();
                } else
                {
                    //K good, we got an array. Now remove those two characters
                    input = input.Substring(1, input.Length - 2);
                }
            }


            //codeLength * 2 -1 is how many characters it should be accounting for commas
            if(input.Length != ((codeLength * 2) - 1))
            {
                isValid = false;
                WrongSyntaxMastermind();
            }

            //Split up the input into pieces using the comma
            string[] codeInputs = input.Split(',');
        
            //Check that the number of code inputs is equivalent to the length of the code
            if(codeInputs.Length != codeLength)
            {
                isValid = false;
                InvalidCodeLengthMastermind();
            }

            //Process the info
            if (isValid)
            {
                MastermindProcessCodeInputs(codeInputs);
            }
        }
    }

    void ScanUserInput(string userCommand)
    {
        string input = userCommand;
        //Don't care if it has a semicolon or not
        if(input[input.Length-1] == ';')
        {
            input = input.Substring(0, input.Length - 1);
        }

        string correctAnswer1 = "";
        string correctAnswer2 = "";

        switch (currentQuestion.correctAnswer)
        {
            case UserQuestion.CorrectAnswer.A:
                correctAnswer1 = "a";
                correctAnswer2 = currentQuestion.answerA;
                break;
            case UserQuestion.CorrectAnswer.B:
                correctAnswer1 = "b";
                correctAnswer2 = currentQuestion.answerB;
                break;
            case UserQuestion.CorrectAnswer.C:
                correctAnswer1 = "c";
                correctAnswer2 = currentQuestion.answerC;
                break;
            case UserQuestion.CorrectAnswer.D:
                correctAnswer1 = "d";
                correctAnswer2 = currentQuestion.answerD;
                break;
        }

        if(input.ToLower() == correctAnswer1.ToLower() || input.ToLower() == correctAnswer2.ToLower())
        {
            DisInfectPiece();
        } else
        {
            WrongAnswerDisinfect();
        }
    }

    void DisInfectPiece()
    {
        DisplayNewMessage(disInfectingPieceMessage + currentPiece.pieceName);
        currentPiece.DisInfect();

        currentPiece = null;
        state = ConsoleStates.normal;
    }

    void WrongAnswerDisinfect()
    {
        DisplayNewMessage(disInfectingWrongAnswerMessage);

        currentPiece = null;
        state = ConsoleStates.normal;
    }

    void UninfectedPiece(MechPiece piece)
    {
        DisplayNewMessage(uninfectedPieceMessage + piece.pieceName);

        currentPiece = null;
        state = ConsoleStates.normal;
    }

    MastermindGame MakeNewMastermindGame()
    {
        MastermindGame game = new MastermindGame();
        game.code = new MastermindGame.ColorOption[codeLength];
        game.numGuessesRemaining = numGuesses;

        for(int i=0; i<game.code.Length; i++)
        {
            game.code[i] = (MastermindGame.ColorOption)Random.Range((int)0, (int)MastermindGame.ColorOption.Count);
        }
        return game;
    }

    void WrongSyntaxMastermind()
    {
        DisplayNewMessage(mastermindWrongSyntaxMessage);
        
    }

    void InvalidCodeLengthMastermind()
    {
        DisplayNewMessage(mastermindInvalidCodeLengthMessage);
    }

    void ShowCodeLength()
    {
        string codeDisplay = "";
        if (useArray)
        {
            codeDisplay += arrayStarter + " ";
        }
        for(int i=0; i<codeLength; i++)
        {
            codeDisplay += "[ ]";

            if(i != codeLength - 1)
            {
                codeDisplay += ",";
            }

            codeDisplay += " ";
        }

        codeDisplay += arrayFinisher;
        DisplayNewMessage(codeDisplay);
    }

    void MastermindProcessCodeInputs(string[] codeInputs)
    {
        int numCorrectInputsInRightSpot = 0;
        int numCorrectInputsInWrongSpot = 0;
        bool[] matchedLetters = new bool[codeLength];
        bool[] usedLetters = new bool[codeLength];

        for(int i=0; i<usedLetters.Length; i++)
        {
            usedLetters[i] = false;
        }

        //Check how many code inputs are in the right spot
        for(int i=0; i<codeInputs.Length; i++)
        {
            if(codeInputs[i] == mastermindGame.code[i].ToString())
            {
                numCorrectInputsInRightSpot++;
                usedLetters[i] = true;
                matchedLetters[i] = true;
            }
        }

        //If the game is solved
        if (numCorrectInputsInRightSpot == codeLength)
        {
            SolvedMasterMind();
        }
        else
        {
            //Check how many are the correct color, but not in the right spot
            for (int i = 0; i < codeInputs.Length; i++)
            {
                if (!matchedLetters[i])
                {

                    bool foundMatch = false;
                    for(int j=0; j<codeInputs.Length; j++)
                    {
                        if(!foundMatch && !usedLetters[j] && codeInputs[i] == mastermindGame.code[j].ToString())
                        {
                            foundMatch = true;
                            usedLetters[j] = true;
                            numCorrectInputsInWrongSpot++;
                        }
                    }
                }
            }


            DisplayNewMessage("correct spot: " + numCorrectInputsInRightSpot);
            DisplayNewMessage("wrong spot: " + numCorrectInputsInWrongSpot);
        }
    }

    void SolvedMasterMind()
    {
        DisplayNewMessage("Yay you solved it!");
        DisInfectPiece();
    }

    void QuitMasterMind()
    {
        DisplayNewMessage(mastermindQuitMessage);

        currentPiece = null;
        state = ConsoleStates.normal;
    }
}
