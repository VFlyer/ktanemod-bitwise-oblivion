using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BitwiseOblivion;
using UnityEngine;
using RNG = UnityEngine.Random;

public class qkBitwiseOblivion : KtaneModule
{
    [SerializeField] private TextMesh QuestionMarksText;
    [SerializeField] private TextMesh StageIndicator;
    [SerializeField] internal TextMesh StringIndicator;
    [SerializeField] private BoolGridSet BoolRow1;
    [SerializeField] private BoolGridSet BoolRow2;
    [SerializeField] private BoolGridSet BoolCol1;
    [SerializeField] private BoolGridSet BoolCol2;
    [SerializeField] private GameObject BoolTL;
    [SerializeField] private GameObject BoolTR;
    [SerializeField] private GameObject BoolBL;
    [SerializeField] private GameObject BoolBR;
    [SerializeField] private KMSelectable GoBackButton;
    [SerializeField] private GameObject StageParent;
    [SerializeField] private GameObject InputParent;
    [SerializeField] private KMSelectable SubmitButton;
    [SerializeField] private KMSelectable ClearButton;
    [SerializeField] internal TextMesh InputText;
    [SerializeField] private TextMesh IndexText;

    internal Dictionary<char, KMSelectable> InputButtons = new Dictionary<char, KMSelectable>();
    private int QuestionMarks = 1;
    internal KMAudio Audio;
    internal bool TwitchTriggers;
    private LinkedList<StageInfo> Stages = new LinkedList<StageInfo>();
    private LinkedListNode<StageInfo> CurrentStage;
    private bool solved;
    private int Stage;
    
    private const bool EnableTwitchTriggers = true;
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    private Coroutine QuestionMarksRoutine;

    IEnumerator HandleQuestionMarks()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            QuestionMarks++;
            if (QuestionMarks == 4)
                QuestionMarks = 1;
            QuestionMarksText.text = new string('?', QuestionMarks);
        }
    }

    private int Input
    {
        get
        {
            return string.IsNullOrEmpty(InputText.text) ? -1 : int.Parse(InputText.text);
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        if(EnableTwitchTriggers)
            Patcher.Patch();
        Patcher.Modules.Clear();
    }
    
    protected override void Start()
    {
        base.Start();
        Patcher.Modules.Add(this);
        Audio = GetComponent<KMAudio>();
        GoBackButton.gameObject.SetActive(false);
        InputParent.SetActive(false);
        QuestionMarksRoutine = StartCoroutine(HandleQuestionMarks());
        BombModule.OnActivate += () =>
        {
            GetIgnoredModules("Bitwise Oblivion", new[] { "Bitwise Oblivion" });
            if (BombInfo.GetSolvableModuleNames().Except(BombInfo.GetSolvedModuleNames()).All(IgnoredModules.Contains))
            {
                Log("No other non-ignored module found. Solving module...");
                solved = true;
                BombModule.HandlePass();
                StopCoroutine(QuestionMarksRoutine);
                QuestionMarksText.text = " :(";
                return;
            }
            TwitchTriggers = EnableTwitchTriggers && TwitchPlaysActive && Forwards.TwitchModules.Cast<object>().Any(m =>
                qkBitwiseOblivionService.BossModules.Contains(
                    Forwards.TwitchModuleComponentID(Forwards.TwitchModuleInfo(Forwards.TwitchModuleSolver(m)))));
            Log("New stage method: {0}", TwitchTriggers ? "Twitch queue" : "module solve");
            OnNewStage += (ModuleName, ReadyToSolve) =>
            { 
                if (solved) 
                    return;
                if (!TwitchTriggers && !ReadyToSolve) 
                    RecordStage(ModuleName, GetRandomString());
                if (ReadyToSolve) 
                { 
                    CurrentStage = Stages.First;
                    StageParent.SetActive(false);
                    if (CurrentStage == null)
                    {
                        Log("Ready to be solved but there weren't any stages recorded. Solving module...");
                        solved = true;
                        StageIndicator.gameObject.SetActive(false);
                        QuestionMarksText.gameObject.SetActive(true);
                        QuestionMarksText.text = " :(";
                        BombModule.HandlePass();
                        return;
                    }
                    InputParent.SetActive(true); 
                    StringIndicator.gameObject.SetActive(true);
                    IndexText.text = (CurrentStage.Value.Index + 1).ToString(); 
                    Stage = 1; 
                    StageIndicator.text = "1"; 
                    Log("Ready to solve.");
                    Log("Stage #1 solution: {0}", CurrentStage.Value.Solution);
                }
            };
        };
        SubmitButton.OnInteract += () =>
        {
            Submit(Input);
            return false;
        };
        GoBackButton.OnInteract += () =>
        {
            //GoBackButton.AddInteractionPunch(.5f);
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, GoBackButton.transform);
            StageParent.SetActive(false);
            InputParent.SetActive(true);
            GoBackButton.gameObject.SetActive(false);
            return false;
        };
        ClearButton.OnInteract += () =>
        {
            InputText.text = "";
            return false;
        };
    }

    private void OnDestroy()
    {
        Patcher.Modules.Clear();
    }

    private bool Submit(int input)
    {
        Log("Submitted input: {0}", input == -1 ? "*empty*" : input.ToString());
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SubmitButton.transform);
        if (solved)
            return false;
        var stage = CurrentStage.Value;
        if (input == stage.Solution)
        {
            Log("Correct solution");
            CurrentStage = CurrentStage.Next;
            if (CurrentStage == null)
            {
                solved = true;
                InputText.text = "GG!";
                StageIndicator.text = "";
                IndexText.text = "Module solved :D";
                Log("Module solved!");
                BombModule.HandlePass();
                return false;
            }
            InputText.text = "";
            IndexText.text = (CurrentStage.Value.Index + 1).ToString();
            StageIndicator.text = (++Stage).ToString();
            Log("Stage #{0} solution: {1}", Stage, CurrentStage.Value.Solution);
            return false;
        }
        Log("Incorrect solution");
        BombModule.HandleStrike();
        InputText.text = "";
        ShowStage(stage);
        GoBackButton.gameObject.SetActive(true);
        return true;
    }

    private string GetRandomString()
    {
        var str = new string(Enumerable.Repeat(Characters, 16)
            .Select(s => s[RNG.Range(0, s.Length)]).ToArray());
        StringIndicator.text = str;
        StringIndicator.gameObject.SetActive(true);
        return str;
    }

    private void ShowStage(StageInfo stage)
    {
        InputParent.SetActive(false);
        StageParent.SetActive(true);
        if(CurrentStage != null)
            StringIndicator.text = stage.RecoverString;
        BoolRow1.Reset(stage.Row1, stage.RowText1);
        BoolRow2.Reset(!stage.Row1, stage.RowText1);
        BoolCol1.Reset(stage.Col1, stage.ColText1);
        BoolCol2.Reset(!stage.Col1, stage.ColText1);
        BoolTL.SetActive(stage.Operator(BoolRow1.Value, BoolCol1.Value) == 1);
        BoolTR.SetActive(stage.Operator(BoolRow1.Value, BoolCol2.Value) == 1);
        BoolBL.SetActive(stage.Operator(BoolRow2.Value, BoolCol1.Value) == 1);
        BoolBR.SetActive(stage.Operator(BoolRow2.Value, BoolCol2.Value) == 1);
    }

    internal void RecordStage(string str1, string str2)
    {
        if (solved)
            return;
        StopCoroutine(QuestionMarksRoutine);
        QuestionMarksText.gameObject.SetActive(false);
        StageIndicator.gameObject.SetActive(true);
        StageParent.SetActive(true);
        StageIndicator.text = (++Stage).ToString();
        Log("------Stage #{0}------", Stage);
        var stage = new StageInfo(str1, str2, LogOBJ);
        Log("------------");
        Stages.AddLast(stage);
        ShowStage(stage);
    }

    private void LogOBJ(string msg, object arg)
    {
        Log(msg, arg);
    }

    private void Log(string msg, params object[] args)
    {
        Debug.LogFormat("[Bitwise Oblivion #{0}] {1}", ModuleID, string.Format(msg, args));
    }
    
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = "Use '!{0} submit n1 n2 n3' to submit a number for stage 1, 2 and 3, '!{0} back' to go back to the input stage after a strike";
    #pragma warning restore 414
    
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        if (command == "back" && GoBackButton.gameObject.activeInHierarchy)
        {
            yield return null;
            GoBackButton.OnInteract();
            yield break;
        }
        var match = Regex.Match(command, @"submit (([0-9]+ ?)+)");
        if (match.Success && InputParent.activeInHierarchy)
        {
            yield return null;
            foreach (var num in match.Groups[1].Value.Split(' '))
            {
                foreach (var character in num.Trim())
                {
                    yield return null;
                    InputButtons[character].OnInteract();
                }
                yield return null;
                if(Submit(Input))
                    yield break;
            }
        }
    }
}
