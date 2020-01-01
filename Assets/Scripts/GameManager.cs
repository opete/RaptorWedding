using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public enum GameState
{
    Loading,
    Intro,
    Menu,
    Options,
    Game
}

public enum HighScoreTypes
{
    normal = 0,
    endless = 1,
    hard = 2,
    endlessHard = 3
}

public enum KeyTypes
{
    left,
    right,
    up,
    down
}

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Game information
    /// </summary>
    public bool invulnerable = true;
    public bool menuNavigation = false;
    private int menuItemSelected = 0;

    public bool hunLang = true;
    public bool hardMode = false;
    public bool endlessMode = false;

    public GameState gameState = GameState.Loading;
    public PlayerCharacter currentPlayer;

    private int currentLevel = 0;
    private int bonusLevel = 0;
    private int score = 0;
    private int streak = 0;
    private int maxLives = 5;
    private int currentLives;

    private int[] levelBPMs = new int[9] { 95, 105, 110, 115, 120, 125, 130, 135, 140 };

    private float levelTargets = 49;
    private float endLevel = 4;

    private float currentLevelMoveTime;
    private float currentLevelBPM;
    private float currentLevelInterval;

    private bool coolDownOk = true;
    private float keyPressCoolDown = 0.1f;
    /// <summary>
    /// References
    /// </summary>
    /// 
    PlayerControls controls;
    public Transform transIntro;
    public Transform transGame;
    public Transform transUICanvas;
    public Transform transParticleFX;

    private Transform transParent;
    private Transform transFX;
    private Transform transTargetStart;
    private Transform transTargetGoal;
    private Transform transObstacleStart;
    private Transform transObstacleGoal;
    private TextMeshProUGUI txtLevel;
    private TextMeshProUGUI txtScore;
    private TextMeshPro txtIntro;
    private Image uiFader;
    //public GameObject objMenu;
    public List<RectTransform> lstRectGoals = new List<RectTransform>();
    //public List<Animator> lstMenuAnims = new List<Animator>();
    private List<ParticleSystem> lstFx = new List<ParticleSystem>();
    private List<TextMeshPro> lstTxtHighScores = new List<TextMeshPro>();
    /// <summary>
    /// PreFabs
    /// </summary>
    public GameObject pfRaptor;
    public List<GameObject> lstExplosionPrefabs = new List<GameObject>();
    public List<GameObject> lstKeys = new List<GameObject>();
    public List<GameObject> lstObstacles = new List<GameObject>();
    /// <summary>
    /// 
    /// </summary>
    private List<KeyCode> lstKeyStrokes = new List<KeyCode>();
    public List<GameObject> lstActiveTargets = new List<GameObject>();
    public List<GameObject> lstActiveObstacles = new List<GameObject>();
    public List<string> lstStringHun = new List<string>();
    public List<string> lstStringEng = new List<string>();
    public List<string> lstString = new List<string>();
    /// <summary>
    /// Constants
    /// </summary>
    private Color cClear = new Color(1, 1, 1, 0);
    public float coRoutineInc = 0.01f;

    private float targetObstacleDifference = 0.15f;

    public AudioSource introMusic;
    public AudioSource gameMusic;
    public AudioSource winMusic;

    public bool escPressed = false;
    private float restartTime = 432.3f;

    IEnumerator CoIntro()
    {
        introMusic.Play();
        transIntro.Find("Fader").GetComponent<Animator>().SetBool("start", true);
        transIntro.Find("txtIntro").GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(21.75f);
        transIntro.Find("txtIntro").GetComponent<Animator>().SetBool("start", false);
        Transform transTitle = transIntro.Find("Title");
        transTitle.Find("PofaBe").GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(5f);
        transTitle.Find("Es").GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(0.5f);
        transTitle.Find("Fuss").GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(2f);
        transTitle.GetComponent<Animator>().SetBool("start", true);
        transIntro.Find("imgOpete").GetComponent<Animator>().SetBool("start", true);
        transIntro.Find("imgLolla").GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(1f);
        gameState = GameState.Menu;
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == "btnEndless" || obj.name == "btnHardcore")
            {
                obj.SetActive(true);
            }
        }
        yield return null;
    }

    IEnumerator CoCoolDown()
    {
        yield return new WaitForSeconds(keyPressCoolDown);
        coolDownOk = true;
    }

    IEnumerator CoSetPlayerAnimation(int i)
    {
        currentPlayer.GetComponent<Animator>().SetInteger("state", i);
        yield return new WaitForSeconds(0.5f);
        currentPlayer.GetComponent<Animator>().SetInteger("state", 0);
        yield return null;
    }

    IEnumerator CoMoveLevelText()
    {
        txtLevel.text = lstString[8] + ": " + (currentLevel + bonusLevel);
        float time = 1f;

        txtLevel.rectTransform.localPosition = new Vector3(200, 45, 0);
        Vector3 goalPos = new Vector3(0, 45, 0);
        Vector3 velocity = Vector3.zero;
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            txtLevel.rectTransform.localPosition = Vector3.SmoothDamp(txtLevel.rectTransform.localPosition, goalPos, ref velocity, 0.3f);
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(coRoutineInc);
        }

        goalPos = new Vector3(-300, 45, 0);
        startTime = Time.time;
        elapsedTime = 0;

        while (elapsedTime < time)
        {
            txtLevel.rectTransform.localPosition = Vector3.SmoothDamp(txtLevel.rectTransform.localPosition, goalPos, ref velocity, 0.3f);
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(coRoutineInc);
        }
        yield return null;
    }

    public IEnumerator CoCreateExplosion(int no)
    {
        GameObject objExplosion = GameObject.Instantiate(lstExplosionPrefabs[no]);
        objExplosion.transform.SetParent(transFX);
        objExplosion.transform.localScale = Vector3.one; //new Vector3(0.75f, 0.5f, 1);
        objExplosion.transform.localPosition = new Vector3(-100, 25, 0);

        objExplosion.GetComponentInChildren<TextMeshProUGUI>().text = lstString[no];

        switch (no)
        {
            case 0:
                score += 3;
                streak++;
                break;
            case 1:
                score += 6;
                streak++;
                break;
            case 2:
                score += 8;
                streak++;
                break;
            default:
                streak = 0;
                if (!invulnerable)
                {
                    currentLives--;
                }
                if (currentLives == 0)
                {
                    StartEndGame(true);
                }
                break;
        }
        txtScore.text = lstString[7] + ": " + score;
        if (streak > 0)
        {
            if (streak % 10 == 0 && currentLives < maxLives)
            {
                currentLives++;
            }
        }

        foreach (Raptor raptor in FindObjectsOfType<Raptor>())
        {
            raptor.goalX = -0.5f - currentLives * 0.1f;
        }

        ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
        emitOverride.startLifetime = 0.5f;

        switch (no)
        {
            case 0:
                lstFx[0].Play();
                lstFx[0].Emit(emitOverride, 5);
                break;
            case 1:
                lstFx[1].Play();
                lstFx[1].Emit(emitOverride, 5);
                break;
            case 2:
                lstFx[0].Play();
                lstFx[1].Play();
                lstFx[0].Emit(emitOverride, 5);
                lstFx[1].Emit(emitOverride, 5);
                break;
            case 3:
                lstFx[2].Play();
                lstFx[2].Emit(emitOverride, 5);
                break;
            case 4:
                lstFx[2].Play();
                lstFx[2].Emit(emitOverride, 5);
                break;
            case 5:
                lstFx[3].Play();
                lstFx[3].Emit(emitOverride, 5);
                break;
        }

        yield return new WaitForSeconds(0.01f);

        EnableFX(false);
        yield return null;
    }

    IEnumerator CoGenerateWithDelay(float delay, float targetDistance, float obstacleDistance)
    {
        yield return new WaitForSeconds(delay);
        int randNo = Random.Range(0, lstKeys.Count);
        GenerateTarget(randNo, targetDistance);
        StartCoroutine(CoGenerateObstacle(randNo, obstacleDistance));
        yield return null;
    }

    IEnumerator CoGenerateObstacle(int i, float distance)
    {
        yield return new WaitForSeconds(targetObstacleDifference);

        GameObject objObstacle = GameObject.Instantiate(lstObstacles[i]);
        objObstacle.transform.SetParent(transGame);
        objObstacle.transform.position = transObstacleStart.position;

        Move move = objObstacle.GetComponent<Move>();
        move.moveTime = currentLevelMoveTime;
        move.speed = distance / move.moveTime;

        lstActiveObstacles.Add(objObstacle);
        yield return new WaitForSeconds(currentLevelMoveTime * 2f);
        lstActiveObstacles.Remove(objObstacle);
        Destroy(objObstacle);
        yield return null;
    }

    void GenerateTarget(int i, float distance)
    {
        GameObject objTarget = GameObject.Instantiate(lstKeys[i]);
        objTarget.transform.SetParent(transParent);
        objTarget.transform.localScale = Vector3.one * 0.25f;
        objTarget.transform.position = transTargetStart.position;

        KeyStroke keyStroke = objTarget.GetComponent<KeyStroke>();
        keyStroke.moveTime = currentLevelMoveTime;
        keyStroke.oldPos = transTargetStart.position;
        keyStroke.goalPos = transTargetGoal.position;
        keyStroke.speed = distance / keyStroke.moveTime;

        lstActiveTargets.Add(objTarget);
    }

    GameObject GetClosestTargetToGoal()
    {
        GameObject minDistObj = null;

        foreach (GameObject objActiveTarget in lstActiveTargets)
        {
            if (objActiveTarget != null)
            {
                KeyStroke keyStroke = objActiveTarget.GetComponent<KeyStroke>();
                if (keyStroke.isEnabled && keyStroke.currentlyAtTarget > -1)
                {
                    minDistObj = objActiveTarget;
                    break;
                }
            }
        }

        return minDistObj;
    }

    private void CreateRaptor()
    {
        GameObject obj = GameObject.Instantiate(pfRaptor);
        obj.transform.parent = transGame;
    }

    IEnumerator CoWin()
    {
        gameMusic.GetComponent<MusicManager>().StartFade(0, true);
        winMusic.Play();
        Car car = FindObjectOfType<Car>();
        car.goal = car.end;
        yield return new WaitForSeconds(2f);
        car.animator.SetBool("open", true);
        yield return new WaitForSeconds(0.5f);
        currentPlayer.goalX = car.goal.x;
        currentPlayer.GetComponent<Animator>().SetInteger("state", 5);
        yield return new WaitForSeconds(2f);
        car.animator.SetBool("open", false);
        currentPlayer.gameObject.SetActive(false);
        car.goal = car.start;
        yield return new WaitForSeconds(2f);
        car.Initialize();
        StartEndGame(false);
        yield return null;
    }

     private void SetActivePlayer(Player player)
    {
        foreach (PlayerCharacter playerCharacter in Resources.FindObjectsOfTypeAll<PlayerCharacter>())
        {
            if (playerCharacter.player != player)
            {
                playerCharacter.gameObject.SetActive(false);
            }
            else
            {
                playerCharacter.gameObject.SetActive(true);
                currentPlayer = playerCharacter;
            }
        }
    }

    private void EnableScrollers(bool on)
    {
        foreach (Scroller scroller in FindObjectsOfType<Scroller>())
        {
            scroller.isEnabled = on;
        }
    }

    private void EnableFX(bool on)
    {
        for (int i = 0; i < lstFx.Count; i++)
        {
            if (on)
            {

            }
            else
            {
                lstFx[i].Stop();
            }
        }
    }

    public void StartIntro()
    {
        gameState = GameState.Intro;
    }

    IEnumerator CoFade(Image image, float time, bool game, Player player)
    {
        float startTime = Time.time;
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            image.color = Color.Lerp(cClear, Color.white, (elapsedTime / time));
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(coRoutineInc);
        }

        image.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (!game)
        {
            EndGame();
            transGame.gameObject.SetActive(game);
        }
        else
        {
            transGame.gameObject.SetActive(game);
            StartCoroutine(StartGame(player));
        }

        startTime = Time.time;
        elapsedTime = 0;

        while (elapsedTime < time)
        {
            image.color = Color.Lerp(Color.white, cClear, (elapsedTime / time));
            elapsedTime = Time.time - startTime;
            yield return new WaitForSeconds(coRoutineInc);
        }

        image.color = cClear;

        yield return null;
    }

    public void ChangeLanguage(bool hun)
    {
        foreach (Animator animator in FindObjectsOfType<Animator>())
        {
            if (animator.name == "PofaBe" || animator.name == "Fuss")
            {
                animator.SetBool("hun", hun);
            }
        }

        if (hun) {
            lstString = lstStringHun;
        } else
        {
            lstString = lstStringEng;
        }
        txtIntro.text = lstString[9];

        hunLang = hun;
    }

    void ChangeToMenuGame(bool game, Player player)
    {
        StartCoroutine(CoFade(uiFader, 0.5f, game, player));
    }

    public void EndGame()
    {
        foreach (Explode explode in FindObjectsOfType<Explode>())
        {
            GameObject.Destroy(explode.gameObject);
        }

        foreach (ParticleSystem p in lstFx)
        {
            p.Stop();
        }

        if (hardMode)
        {
            if (endlessMode)
            {
                lstTxtHighScores[3].gameObject.SetActive(true);
                if (score > int.Parse(lstTxtHighScores[3].text))
                {
                    lstTxtHighScores[3].text = score.ToString();
                }
            }
            else
            {
                lstTxtHighScores[2].gameObject.SetActive(true);
                if (score > int.Parse(lstTxtHighScores[2].text))
                {
                    lstTxtHighScores[2].text = score.ToString();
                }
            }
        }
        else
        {
            if (endlessMode)
            {
                lstTxtHighScores[1].gameObject.SetActive(true);
                if (score > int.Parse(lstTxtHighScores[1].text))
                {
                    lstTxtHighScores[1].text = score.ToString();
                }
            }
            else
            {
                lstTxtHighScores[0].gameObject.SetActive(true);
                if (score > int.Parse(lstTxtHighScores[0].text))
                {
                    lstTxtHighScores[0].text = score.ToString();
                }
            }
        }
        gameState = GameState.Menu;
    }

    private void StartEndGame(bool death)
    {
        StopAllCoroutines();
        introMusic.GetComponent<MusicManager>().StartFade(1, false);
        gameMusic.GetComponent<MusicManager>().StartFade(0, true);
        EnableFX(false);
        EnableScrollers(false);

        foreach (PlayerCharacter playerCharacter in FindObjectsOfType<PlayerCharacter>())
        {
            playerCharacter.isEnabled = false;
        }

        if (death)
        {
            foreach (Raptor raptor in FindObjectsOfType<Raptor>())
            {
                raptor.animator.SetInteger("state", 1);
            }
        }

        
        ChangeToMenuGame(false, new Player());
    }

    private void CleanPreviousGame()
    {
        foreach (GameObject obj in lstActiveTargets)
        {
            GameObject.Destroy(obj);
        }
        lstActiveTargets.Clear();
        foreach (GameObject obj in lstActiveObstacles)
        {
            GameObject.Destroy(obj);
        }
        lstActiveObstacles.Clear();
        foreach (Raptor raptor in FindObjectsOfType<Raptor>())
        {
            GameObject.Destroy(raptor.gameObject);
        }
    }

    private void InitializeNewGame(Player player)
    {
        currentLives = maxLives;
        currentLevel = 0;
        bonusLevel = 0;
        score = 0;
        txtScore.text = lstString[7] + ": " + score;

        transGame.gameObject.SetActive(true);
        SetActivePlayer(player);
        CreateRaptor();
        EnableScrollers(true);
    }

    private float GenerateLevel(int level)
    {
        currentLevelBPM = levelBPMs[level - 1];
        currentLevelInterval = (60 / currentLevelBPM) * 2f;
        currentLevelMoveTime = currentLevelInterval * 3f;
        float initialDelay = currentLevelInterval * 4f - currentLevelMoveTime;

        float levelLength = initialDelay;

        float targetDistance = Vector3.Distance(transTargetStart.position, transTargetGoal.position);
        float obstacleDistance = Vector3.Distance(transObstacleStart.position,transObstacleGoal.position);

        for (int i = 0; i < levelTargets; i++)
        {
            StartCoroutine(CoGenerateWithDelay(levelLength, targetDistance, obstacleDistance));
            levelLength += currentLevelInterval;
        }

        levelLength = currentLevelInterval * (levelTargets + 3);

        //Debug.Log("level " + currentLevel + " generated at " + Time.time + " with length " + (levelLength));
        return levelLength;
    }

    IEnumerator StartGame(Player player)
    {
        CleanPreviousGame();
        InitializeNewGame(player);
        introMusic.GetComponent<MusicManager>().StartFade(0, false);
        gameMusic.Stop();
        gameMusic.time = 0;
        gameMusic.Play();
        gameState = GameState.Game;

        float levelEndTime = 0;
        while (true) {
            yield return new WaitForSeconds(levelEndTime);
            if (currentLevel < levelBPMs.Length)
            {
                currentLevel++;
            } else
            {
                bonusLevel++;
                gameMusic.time = restartTime;
            }
            StartCoroutine(CoMoveLevelText());
            if (!endlessMode && endLevel == currentLevel)
            {
                StartCoroutine(CoWin());
                yield return null;
            }
            else
            {
                levelEndTime = GenerateLevel(currentLevel);
            }
        }
    }

    public void StartStartGame(Player player)
    {
        ChangeToMenuGame(true, player);
    }

    KeyCode KeyPress()
    {
        if (coolDownOk)
        {
            foreach (KeyCode key in lstKeyStrokes)
            {
                if (Input.GetKeyDown(key))
                {
                    coolDownOk = false;
                    StartCoroutine(CoCoolDown());
                    return (key);
                }
            }
        }
        return KeyCode.Backspace;
    }

    public void ProcessKeyStroke(KeyCode currentKey)
    {
        switch (gameState)
        {
            case GameState.Loading:
                break;
            case GameState.Intro:
                switch (currentKey)
                {
                    case KeyCode.Escape:

                        break;
                }
                        break;
            case GameState.Menu:
                if (menuNavigation)
                {
                    switch (currentKey)
                    {
                        case KeyCode.LeftArrow:
                            if (menuItemSelected > 0)
                            {
                                menuItemSelected--;
                                foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                                {
                                    myButton.isSelected = menuItemSelected == myButton.buttonNo;
                                    if (myButton.isSelected)
                                    {
                                        myButton.OnMouseEnter();
                                    }
                                    else
                                    {
                                        myButton.OnMouseExit();
                                    }
                                }
                            }
                            break;
                        case KeyCode.RightArrow:
                            if (menuItemSelected < 3)
                            {
                                menuItemSelected++;
                                foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                                {
                                    myButton.isSelected = menuItemSelected == myButton.buttonNo;
                                    if (myButton.isSelected)
                                    {
                                        myButton.OnMouseEnter();
                                    } else
                                    {
                                        myButton.OnMouseExit();
                                    }
                                }
                            }
                            break;
                        case KeyCode.UpArrow:
                            break;
                        case KeyCode.DownArrow:
                            menuNavigation = false;
                            foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                            {
                                myButton.isSelected = false;
                                myButton.OnMouseExit();
                            }
                            break;
                        case KeyCode.Backspace:
                            menuNavigation = false;
                            break;
                        case KeyCode.Return:
                            foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                            {
                                if (myButton.isSelected)
                                {
                                    myButton.OnMouseDown();
                                }
                            }
                            if (menuItemSelected == 0)
                            {
                                foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                                {
                                    myButton.isSelected = menuItemSelected == myButton.buttonNo;
                                    if (myButton.isSelected)
                                    {
                                        myButton.OnMouseEnter();
                                    }
                                    else
                                    {
                                        myButton.OnMouseExit();
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (currentKey)
                    {
                        case KeyCode.LeftArrow:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                playerSelect.isSelected = playerSelect.player == Player.lolla;
                            }
                            break;
                        case KeyCode.RightArrow:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                playerSelect.isSelected = playerSelect.player == Player.opete;
                            }
                            break;
                        case KeyCode.UpArrow:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                playerSelect.isSelected = false;
                                menuNavigation = true;
                                foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                                {
                                    myButton.isSelected = menuItemSelected == myButton.buttonNo;
                                    if (myButton.isSelected)
                                    {
                                        myButton.OnMouseEnter();
                                    }
                                    else
                                    {
                                        myButton.OnMouseExit();
                                    }
                                }
                            }
                            break;
                        case KeyCode.DownArrow:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                playerSelect.isSelected = false;
                            }
                            break;
                        case KeyCode.Backspace:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                playerSelect.isSelected = false;
                                menuNavigation = true;
                                foreach (MyButton myButton in FindObjectsOfType<MyButton>())
                                {
                                    myButton.isSelected = menuItemSelected == myButton.buttonNo;
                                    if (myButton.isSelected)
                                    {
                                        myButton.OnMouseEnter();
                                    }
                                    else
                                    {
                                        myButton.OnMouseExit();
                                    }
                                }
                            }
                            break;
                        case KeyCode.Return:
                            foreach (PlayerSelect playerSelect in FindObjectsOfType<PlayerSelect>())
                            {
                                if (playerSelect.isSelected)
                                {
                                    StartStartGame(playerSelect.player);
                                }
                            }
                            break;
                    }
                }

                break;
            case GameState.Game:

                // get closest target, and check if it is within range
                GameObject minDistObj = GetClosestTargetToGoal();
                if (minDistObj != null)
                {
                    KeyStroke minDistKey = minDistObj.GetComponent<KeyStroke>();

                    if (minDistKey.currentlyAtTarget > -1 && minDistKey.isEnabled)
                    {
                        int keyNo = lstKeyStrokes.IndexOf(currentKey);
                        if (keyNo > -1)
                        {
                            switch (keyNo)
                            {
                                case 0:
                                    StartCoroutine(currentPlayer.CoScaleSine(0.5f, -1));
                                    break;
                                case 1:
                                    StartCoroutine(currentPlayer.CoScaleSine(0.5f, 1));
                                    break;
                                default:
                                    StartCoroutine(CoSetPlayerAnimation(keyNo + 1));
                                    break;
                            }
                        }


                        if (currentKey == minDistKey.keyCode)
                        {
                            minDistObj.GetComponent<KeyStroke>().Press();
                        }
                        else
                        {
                            // Miss
                            minDistKey.Disable();
                            StartCoroutine(CoCreateExplosion(5));
                        }
                    }
                }
                break;
        }
    }


    void Update()
    {
        switch (gameState) {
            case GameState.Loading:
                StartIntro();
                break;
            case GameState.Intro:
                break;
            case GameState.Menu:
                break;
            case GameState.Game:
                break;
        }
    }

    void Start()
    {
        txtIntro = transIntro.Find("txtIntro").GetComponent<TextMeshPro>();
        for (int i = 0; i < 4; i++)
        {
            lstTxtHighScores.Add(transIntro.Find("txtHighScore" + i).GetComponent<TextMeshPro>());
            lstTxtHighScores[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < 4; i++)
        {
            lstFx.Add(transParticleFX.Find("p" + i).GetComponent<ParticleSystem>());
            lstFx[i].Stop();
        }

        uiFader =  transUICanvas.Find("Fader").GetComponent<Image>();

        Transform transCanvas = transGame.Find("Canvas");
        transFX = transCanvas.Find("FX");
        Transform transOverlay = transCanvas.Find("Overlay");
        transTargetStart = transOverlay.Find("Start");
        transTargetGoal = transOverlay.Find("Goal");
        for (int i = 0; i < 7; i++)
        {
            lstRectGoals.Add(transOverlay.Find("rect" + i).GetComponent<RectTransform>());
            lstRectGoals[i].GetComponent<Image>().enabled = false;
        }

        transParent = transCanvas.Find("Keys");
        Transform transUI = transCanvas.Find("UI");
        txtScore = transUI.Find("txtScore").GetComponent<TextMeshProUGUI>();
        txtLevel = transUI.Find("txtLevel").GetComponent<TextMeshProUGUI>();
        transObstacleStart = transGame.Find("ObstacleStart");
        transObstacleGoal = transGame.Find("ObstacleGoal");
        transGame.gameObject.SetActive(false);

        escPressed = false;

        controls = new PlayerControls();
        controls.Gameplay.Up.performed += ctx => ProcessKeyStroke(KeyCode.UpArrow);
        controls.Gameplay.Down.performed += ctx => ProcessKeyStroke(KeyCode.DownArrow);
        controls.Gameplay.Left.performed += ctx => ProcessKeyStroke(KeyCode.LeftArrow);
        controls.Gameplay.Right.performed += ctx => ProcessKeyStroke(KeyCode.RightArrow);
        controls.Gameplay.Cancel.performed += ctx => ProcessKeyStroke(KeyCode.Backspace);
        controls.Gameplay.Select.performed += ctx => ProcessKeyStroke(KeyCode.Return);

        controls.Gameplay.Enable();

        foreach (GameObject obj in lstKeys)
        {
            lstKeyStrokes.Add(obj.GetComponent<KeyStroke>().keyCode);
        }

        ChangeLanguage(hunLang);

        if (gameState == GameState.Game)
        {
            StartStartGame(Player.opete);
        } else
        {
            transGame.gameObject.SetActive(false);
            StartCoroutine(CoIntro());
        }
    }
}
