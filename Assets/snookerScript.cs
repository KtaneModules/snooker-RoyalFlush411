using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class snookerScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public Ball[] balls;
    public KMSelectable cueBall;
    public TextMesh[] breakTargets;
    private int activeReds = 0;
    public Color[] uiColours;

    private int potentialShots = 0;
    private int actualShots = 0;
    public int[] shotsPerBreak = new int[4];
    public int[] breaks = new int[4];
    private int currentColour = 2;
    private bool clearance;
    private int targetColour = 2;
    private int playerIndex = 0;
    private int playerIndex2 = 0;
    private List<int> breakBreakdown = new List<int>();
    private List<int> break1Breakdown = new List<int>();
    private List<string> break1String = new List<string>();
    private List<int> break2Breakdown = new List<int>();
    private List<string> break2String = new List<string>();
    private List<int> break3Breakdown = new List<int>();
    private List<string> break3String = new List<string>();
    private List<int> break4Breakdown = new List<int>();
    private List<string> break4String = new List<string>();
    public string[] loggingColours;

    private int currentBreak = 0;
    private readonly int[] breaksPlayed = new int[4];
    private int breakShotsTaken = 0;
    private bool red;
    private bool targetRed = true;

    public AudioSource scoresSFX;
    public AudioClip[] scores;
    public AudioSource strikeSFX;
    public AudioClip[] strikeOptions;
    public AudioClip[] playerNames;
    public AudioClip applause;
    public AudioClip frame;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    int stage = 0;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        foreach (Ball ball in balls)
        {
            ball.selectable = ball.GetComponent<KMSelectable>();
            ball.selectable.OnInteract += delegate () { OnBallPress(ball); return false; };
        }
        cueBall.OnInteract += delegate () { OnCueBallPress(); return false; };
    }

    void Start()
    {
        SetUpBalls();
        CalculateShotsPerBreak();
        CalculateBreaks();
        TerribleLoggingWhichLiterallyDoubledTheLengthOfTheCode();
    }

    void SetUpBalls()
    {
        activeReds = UnityEngine.Random.Range(8, 11);
        if (activeReds == 9)
        {
            int index = UnityEngine.Random.Range(0, 6);
            balls[index].active = false;
            balls[index].gameObject.SetActive(false);
        }
        else if (activeReds == 8)
        {
            int index = UnityEngine.Random.Range(0, 6);
            balls[index].active = false;
            balls[index].gameObject.SetActive(false);
            int index2 = UnityEngine.Random.Range(0, 6);
            while (index2 == index)
            {
                index2 = UnityEngine.Random.Range(0, 6);
            }
            balls[index2].active = false;
            balls[index2].gameObject.SetActive(false);
        }
        playerIndex = UnityEngine.Random.Range(0, 19);
        playerIndex2 = UnityEngine.Random.Range(0, 19);
        while (playerIndex2 == playerIndex)
        {
            playerIndex2 = UnityEngine.Random.Range(0, 19);
        }
        Debug.LogFormat("[Snooker #{0}] You have {1} red balls available.", moduleId, activeReds);
    }

    void CalculateShotsPerBreak()
    {
        potentialShots = (activeReds * 2) + 6;
        int breakIndex = potentialShots / 2;
        int shots = UnityEngine.Random.Range(2, breakIndex);
        shotsPerBreak[0] = shots;
        if (shots % 2 == 1 && (potentialShots - shots) > 6)
        {
            shots++;
        }
        potentialShots -= shots;

        breakIndex = potentialShots / 2;
        shots = UnityEngine.Random.Range(5, breakIndex);
        shotsPerBreak[1] = shots;
        if (shots % 2 == 1 && (potentialShots - shots) > 6)
        {
            shots++;
        }
        potentialShots -= shots;

        breakIndex = potentialShots - 1;
        shots = UnityEngine.Random.Range(3, breakIndex);
        shotsPerBreak[2] = shots;
        if (shots % 2 == 1 && (potentialShots - shots) > 6)
        {
            shots++;
        }
        potentialShots -= shots;

        shotsPerBreak[3] = potentialShots;
    }

    void CalculateBreaks()
    {
        actualShots = shotsPerBreak.Sum();
        for (int i = 0; i <= 3; i++)
        {
            breakShotsTaken = 0;
            if (actualShots - shotsPerBreak[i] >= 6)
            {
                actualShots -= shotsPerBreak[i];
                while (shotsPerBreak[i] - breakShotsTaken > 1)
                {
                    int colour = UnityEngine.Random.Range(2, 8);
                    breakBreakdown.Add(colour);
                    breaks[i] += colour + 1;
                    breakShotsTaken += 2;
                }
                if (breakShotsTaken != shotsPerBreak[i])
                {
                    breaks[i]++;
                }
            }
            else
            {
                red = true;
                while (shotsPerBreak[i] - breakShotsTaken > 1 && actualShots > 6)
                {
                    if (red)
                    {
                        breaks[i]++;
                        red = false;
                        breakShotsTaken++;
                        actualShots--;
                    }
                    else
                    {
                        int colour = UnityEngine.Random.Range(2, 8);
                        breakBreakdown.Add(colour);
                        breaks[i] += colour;
                        red = true;
                        breakShotsTaken++;
                        actualShots--;
                    }
                }
                while (shotsPerBreak[i] - breakShotsTaken > 0)
                {
                    breaks[i] += currentColour;
                    breakBreakdown.Add(currentColour);
                    breakShotsTaken++;
                    currentColour++;
                    actualShots--;
                }
            }
        }
        for (int i = 0; i <= 3; i++)
        {
            breakTargets[i].text = breaks[i].ToString();
            breakTargets[i].color = uiColours[2];
        }
        breakTargets[stage].color = uiColours[0];
    }

    void TerribleLoggingWhichLiterallyDoubledTheLengthOfTheCode()
    {
        break1Breakdown.Add(1);
        int breakStage = 0;
        int redActive = activeReds - 1;
        while (break1Breakdown.Sum() != breaks[0])
        {
            break1Breakdown.Add(breakBreakdown[breakStage]);
            breakStage++;
            if (break1Breakdown.Sum() == breaks[0])
            {
                break;
            }
            else if (redActive > 0)
            {
                break1Breakdown.Add(1);
                redActive--;
            }
        }

        if (redActive > 0)
        {
            break2Breakdown.Add(1);
            redActive--;
        }
        while (break2Breakdown.Sum() != breaks[1])
        {
            break2Breakdown.Add(breakBreakdown[breakStage]);
            breakStage++;
            if (break2Breakdown.Sum() == breaks[1])
            {
                break;
            }
            else if (redActive > 0)
            {
                break2Breakdown.Add(1);
                redActive--;
            }
        }

        if (redActive > 0)
        {
            break3Breakdown.Add(1);
            redActive--;
        }
        while (break3Breakdown.Sum() != breaks[2])
        {
            break3Breakdown.Add(breakBreakdown[breakStage]);
            breakStage++;
            if (break3Breakdown.Sum() == breaks[2])
            {
                break;
            }
            else if (redActive > 0)
            {
                break3Breakdown.Add(1);
                redActive--;
            }
        }

        if (redActive > 0)
        {
            break4Breakdown.Add(1);
            redActive--;
        }
        while (break4Breakdown.Sum() != breaks[3])
        {
            break4Breakdown.Add(breakBreakdown[breakStage]);
            breakStage++;
            if (break4Breakdown.Sum() == breaks[3])
            {
                break;
            }
            else if (redActive > 0)
            {
                break4Breakdown.Add(1);
                redActive--;
            }
        }
        for (int i = 0; i <= break1Breakdown.Count() - 1; i++)
        {
            if (break1Breakdown[i] == 1)
            {
                break1String.Add("red");
            }
            else if (break1Breakdown[i] == 2)
            {
                break1String.Add("yellow");
            }
            else if (break1Breakdown[i] == 3)
            {
                break1String.Add("green");
            }
            else if (break1Breakdown[i] == 4)
            {
                break1String.Add("brown");
            }
            else if (break1Breakdown[i] == 5)
            {
                break1String.Add("blue");
            }
            else if (break1Breakdown[i] == 6)
            {
                break1String.Add("pink");
            }
            else if (break1Breakdown[i] == 7)
            {
                break1String.Add("black");
            }
        }
        for (int i = 0; i <= break2Breakdown.Count() - 1; i++)
        {
            if (break2Breakdown[i] == 1)
            {
                break2String.Add("red");
            }
            else if (break2Breakdown[i] == 2)
            {
                break2String.Add("yellow");
            }
            else if (break2Breakdown[i] == 3)
            {
                break2String.Add("green");
            }
            else if (break2Breakdown[i] == 4)
            {
                break2String.Add("brown");
            }
            else if (break2Breakdown[i] == 5)
            {
                break2String.Add("blue");
            }
            else if (break2Breakdown[i] == 6)
            {
                break2String.Add("pink");
            }
            else if (break2Breakdown[i] == 7)
            {
                break2String.Add("black");
            }
        }
        for (int i = 0; i <= break3Breakdown.Count() - 1; i++)
        {
            if (break3Breakdown[i] == 1)
            {
                break3String.Add("red");
            }
            else if (break3Breakdown[i] == 2)
            {
                break3String.Add("yellow");
            }
            else if (break3Breakdown[i] == 3)
            {
                break3String.Add("green");
            }
            else if (break3Breakdown[i] == 4)
            {
                break3String.Add("brown");
            }
            else if (break3Breakdown[i] == 5)
            {
                break3String.Add("blue");
            }
            else if (break3Breakdown[i] == 6)
            {
                break3String.Add("pink");
            }
            else if (break3Breakdown[i] == 7)
            {
                break3String.Add("black");
            }
        }
        for (int i = 0; i <= break4Breakdown.Count() - 1; i++)
        {
            if (break4Breakdown[i] == 1)
            {
                break4String.Add("red");
            }
            else if (break4Breakdown[i] == 2)
            {
                break4String.Add("yellow");
            }
            else if (break4Breakdown[i] == 3)
            {
                break4String.Add("green");
            }
            else if (break4Breakdown[i] == 4)
            {
                break4String.Add("brown");
            }
            else if (break4Breakdown[i] == 5)
            {
                break4String.Add("blue");
            }
            else if (break4Breakdown[i] == 6)
            {
                break4String.Add("pink");
            }
            else if (break4Breakdown[i] == 7)
            {
                break4String.Add("black");
            }
        }
        Debug.LogFormat("[Snooker #{0}] Your four breaks are {1}, {2}, {3} & {4}.", moduleId, breakTargets[0].text, breakTargets[1].text, breakTargets[2].text, breakTargets[3].text);
        Debug.LogFormat("[Snooker #{0}] There are multiple ways to achieve your four breaks; suggestions are given below.", moduleId);
        Debug.LogFormat("[Snooker #{0}] 1st break: {1}.", moduleId, string.Join(", ", break1String.Select((x) => x).ToArray()));
        Debug.LogFormat("[Snooker #{0}] 2nd break: {1}.", moduleId, string.Join(", ", break2String.Select((x) => x).ToArray()));
        Debug.LogFormat("[Snooker #{0}] 3rd break: {1}.", moduleId, string.Join(", ", break3String.Select((x) => x).ToArray()));
        Debug.LogFormat("[Snooker #{0}] 4th break: {1}.", moduleId, string.Join(", ", break4String.Select((x) => x).ToArray()));
    }

    public void OnBallPress(Ball ball)
    {
        ball.selectable.AddInteractionPunch(0.5f);
        currentBreak += ball.points;
        if (ball.colour == "red" && targetRed && !clearance)
        {
            ball.active = false;
            ball.gameObject.SetActive(false);
            Debug.LogFormat("[Snooker #{0}] You potted a red ball. Current break: {1}.", moduleId, currentBreak);
            targetRed = false;
            activeReds--;
            StartCoroutine(PlayAudio(currentBreak));
        }
        else if (clearance)
        {
            if (ball.points == targetColour)
            {
                targetColour++;
                ball.active = false;
                ball.gameObject.SetActive(false);
                Debug.LogFormat("[Snooker #{0}] You potted a {1} ball. Current break: {2}.", moduleId, ball.colour, currentBreak);
                StartCoroutine(PlayAudio(currentBreak));
            }
            else
            {
                StartCoroutine(PotReplace(ball));
                Debug.LogFormat("[Snooker #{0}] Strike! You potted a {1} ball. I was expecting a {2} ball. Table reset.", moduleId, ball.colour, loggingColours[targetColour]);
                Strike();
                Audio.PlaySoundAtTransform("foul", transform);
            }
        }
        else if ((targetRed && ball.colour != "red") || (!targetRed && ball.colour == "red"))
        {
            if (targetRed)
            {
                Debug.LogFormat("[Snooker #{0}] Strike! You potted a {1} ball. I was expecting a red ball. Table reset.", moduleId, ball.colour);
            }
            else
            {
                Debug.LogFormat("[Snooker #{0}] Strike! You potted a {1} ball. I was expecting a coloured ball. Table reset.", moduleId, ball.colour);
            }
            Audio.PlaySoundAtTransform("foul", transform);
            Strike();
        }
        else
        {
            Debug.LogFormat("[Snooker #{0}] You potted a {1} ball. Current break: {2}.", moduleId, ball.colour, currentBreak);
            StartCoroutine(PotReplace(ball));
            StartCoroutine(PlayAudio(currentBreak));
        }
    }

    public void OnCueBallPress()
    {
        cueBall.AddInteractionPunch();
        breaksPlayed[stage] = currentBreak;
        if (breaksPlayed[stage] == breaks[stage])
        {
            breakTargets[stage].color = uiColours[1];
            stage++;
            if (stage == 4)
            {
                SolveChecker();
            }
            else
            {
                Debug.LogFormat("[Snooker #{0}] YOUR BREAK WAS {1}. That is correct.", moduleId, currentBreak, breakTargets[stage]);
                StartCoroutine(EndTurnAudio());
                breakTargets[stage].color = uiColours[0];
                currentBreak = 0;
                if (activeReds == 0)
                    clearance = true;
                targetRed = !clearance;
            }
        }
        else
        {
            Debug.LogFormat("[Snooker #{0}] Strike! YOUR BREAK WAS {1}. I was expecting {2}. Table reset.", moduleId, currentBreak, breakTargets[stage].text);
            Audio.PlaySoundAtTransform("foul", transform);
            Strike();
        }

    }

    IEnumerator PotReplace(Ball iterator)
    {
        if (activeReds == 0)
            clearance = true;
        targetRed = !clearance;
        iterator.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        iterator.gameObject.SetActive(true);
    }

    IEnumerator PlayAudio(int reportedBreak)
    {
        int strikeIndex = UnityEngine.Random.Range(0, 5);
        strikeSFX.clip = strikeOptions[strikeIndex];
        strikeSFX.Play();
        yield return new WaitForSeconds(0.5f);
        scoresSFX.clip = scores[reportedBreak - 1];
        scoresSFX.Play();
    }

    IEnumerator EndTurnAudio()
    {
        int currentBreak2 = currentBreak;
        strikeSFX.clip = applause;
        strikeSFX.Play();
        scoresSFX.clip = playerNames[stage % 2 == 0 ? playerIndex : playerIndex2];
        scoresSFX.Play();
        yield return new WaitForSeconds(1.3f);
        scoresSFX.clip = scores[currentBreak2 - 1];
        scoresSFX.Play();
        yield return new WaitForSeconds(1.0f);
        if (moduleSolved)
        {
            scoresSFX.clip = frame;
            scoresSFX.Play();
        }
    }

    void SolveChecker()
    {
        if (!balls[15].active && !balls[14].active && !balls[13].active && !balls[12].active && !balls[11].active && !balls[10].active)
        {
            moduleSolved = true;
            StartCoroutine(EndTurnAudio());
            Debug.LogFormat("[Snooker #{0}] YOUR BREAK WAS {1}. That is correct. Module disarmed.", moduleId, currentBreak);
            cueBall.gameObject.SetActive(false);
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            Audio.PlaySoundAtTransform("foul", transform);
            Debug.LogFormat("[Snooker #{0}] Strike! The table has not been cleared. Table reset.", moduleId);
            Strike();
        }
    }

    void Strike()
    {
        GetComponent<KMBombModule>().HandleStrike();
        stage = 0;
        foreach (Ball ball in balls)
        {
            ball.gameObject.SetActive(true);
        }
        activeReds = 0;
        currentColour = 2;
        potentialShots = 0;
        actualShots = 0;
        for (int i = 0; i <= 3; i++)
        {
            shotsPerBreak[i] = 0;
            breaks[i] = 0;
            breaksPlayed[i] = 0;
        }
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].active = true;
        }
        clearance = false;
        targetColour = 2;
        currentBreak = 0;
        breakShotsTaken = 0;
        red = false;
        targetRed = true;
        breakBreakdown.Clear();
        break1Breakdown.Clear();
        break1String.Clear();
        break2Breakdown.Clear();
        break2String.Clear();
        break3Breakdown.Clear();
        break3String.Clear();
        break4Breakdown.Clear();
        break4String.Clear();
        Start();
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} red blue red brown cue | !{0} RBRNRPW [N=brown, K=black]";
#pragma warning restore 414

    private static readonly string[] _colorNames = new[] { "white", "red", "yellow", "green", "brown", "blue", "pink", "black" };
    private static readonly string[] _colorNamesShort = new[] { "WCwc", "Rr", "Yy", "Gg", "Nn", "Bb", "PIpi", "Kk" };

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var list = new List<string>();

        Match m;
        if ((m = Regex.Match(command, string.Format(@"^\s*([{0}\s]+)\s*$", string.Join("", _colorNamesShort)), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            foreach (var ch in m.Groups[1].Value)
                if (!char.IsWhiteSpace(ch))
                    list.Add(_colorNames[Array.FindIndex(_colorNamesShort, cn => cn.Contains(ch))]);
        }
        else if ((m = Regex.Match(command, string.Format(@"^\s*({0}|cue| |,|;)+\s*$", string.Join("|", _colorNames)), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
            list.AddRange(m.Groups[1].Value.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
        else
            yield break;

        foreach (var color in list)
        {
            if (color.Equals("white", StringComparison.InvariantCultureIgnoreCase) || color.Equals("cue", StringComparison.InvariantCultureIgnoreCase))
            {
                yield return null;
                yield return new[] { cueBall };
                yield return new WaitForSeconds(2f);
                continue;
            }
            var ball = balls.FirstOrDefault(b => b.active && b.colour.Equals(color, StringComparison.InvariantCultureIgnoreCase));
            if (ball == null || ball.selectable == null)
            {
                Debug.LogFormat("<Snooker #{0}> Had the case where a {1} ball wasn’t available. Colors are: {2}", moduleId, color, string.Join(", ", balls.Select(b => string.Format("“{0}/{1}/{2}”", b.colour, b.active, b.selectable == null ? "null" : "sel")).ToArray()));
                yield return string.Format("sendtochat There’s no “{0}” ball available.", color);
                yield break;
            }
            if (!ball.gameObject.activeSelf)
                yield return new WaitForSeconds(1f);
            if (!ball.gameObject.activeSelf)
            {
                yield return string.Format("sendtochat There’s no {0} ball available.", color);
                yield break;
            }
            yield return null;
            yield return new[] { ball.selectable };
            yield return new WaitForSeconds(1f);
        }
    }
}
