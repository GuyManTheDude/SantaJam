using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public AudioSource SFX1;
    public AudioSource SFX2;
    public AudioSource Music;

    public bool isTestTalk = false;
    public Slider SFX;
    public Slider Tunes;
    public Slider Voices;

    public GameObject HowTo;
    public GameObject EndText;
    public MeshRenderer Page1;
    public MeshRenderer Page2;

    public List<Material> firstPages;
    public List<Material> secondPages;

    float volMult = 1f;
    float sfxMult = 1f;
    
    private int indexBuffer = 0;
    public int takeCount = 0;
    public bool canClick = true;
    public List<AudioClip> Sounds;
    public List<AudioClip> Songs;

    public GameObject ContinueButton;
    public GameObject StartButton;
    public Transform playerPoint;
    public Animator Book;
    public bool isBattling = false;
    public bool gameStarted = false;

    public GameObject emptySlot;
    public GameObject SnippedCard;
    public List<GameObject> HealthIcons;
    public Creature mySquirrel;
    public List<Card> _deck;
    public Transform DeckPosition;
    public Transform FieldSpawnPos;

    public List<Card> _hand;
    public int handSize = 5;
    public List<Transform> HandPoints;
    public Text DeckCount;

    public GameObject BattleCanvas;
    public List<Card> BattleField;
    public List<Transform> BattleFieldPoints;
    public Transform CreatureFieldPoint;
    public Creature BattleCreature;

    public List<Card> Field;
    public List<Transform> FieldPoints;
    public int FieldRevealed = 0;

    public GameObject FightButton;
    public GameObject NextStageButton;

    public List<GameObject> SummerDaySpawnList;
    public List<GameObject> SummerNightSpawnList;

    public List<GameObject> FallDaySpawnList;
    public List<GameObject> FallNightSpawnList;

    public List<GameObject> WinterDaySpawnList;
    public List<GameObject> WinterNightSpawnList;

    public Text Month;
    public List<Stage> Stages;
    public Stage currentStage;

    public List<Card> EventDeck;

    public Card _selectedCard1 = null;
    public Card _selectedCard2 = null;

    public Toggle endlessToggle;
    public int currentYear = 1;
    public bool isEndless = false;
    public bool GameOver = false;
    private SceneManager scene;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        updateDeckCount();
        MonthCheck();
    }

    

    public void GameEnd()
    {
        GameOver = true;
        Invoke("CloseBook", 1.4f);
        Invoke("ResetScene", 2.8f);
        mySquirrel.sayLine("......");
        BattleCreature.sayLine("......");
        SFX1.Stop();
        SFX2.Stop();
        Music.Stop();
        ClearField();
        RemoveCard(mySquirrel);
        BattleCanvas.SetActive(false);
        ContinueButton.SetActive(false);
        EndText.SetActive(false);
        for (int i = 0; i <10; i++)
        {
            DiscardRandom();
        }
        
    }

    public void EndlessToggle()
    {
        isEndless = endlessToggle.isOn;
    }

    public void HowToOn()
    {
        HowTo.SetActive(true);
    }

    public void HowToOff()
    {
        HowTo.SetActive(false);
    }

    private void CloseBook()
    {
        Book.Play("Close");
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < HealthIcons.Count; i++)
        {
            HealthIcons[i].SetActive(false);

        }
        for (int i = 0; i < mySquirrel._healthPoints; i++)
        {
            HealthIcons[i].SetActive(true);
        }
        if(mySquirrel._healthPoints <= 0)
        {
            GameOver = true;
            canClick = false;
            Invoke("GameEnd", 1f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartStage();
        //}
    }

    void updateDeckCount()
    {
        DeckCount.text = _deck.Count.ToString();
    }

    public void SelectCard(Card toSelect)
    {
        if(_selectedCard1 == null)
        {
            _selectedCard1 = toSelect;
            _selectedCard1.transform.position += new Vector3(0, 2, 0);
            _selectedCard2 = null;

            SFX1.PlayOneShot(Sounds[0]);
            
        }
        else if(_selectedCard2 == null && _selectedCard1 != null)
        {
            _selectedCard2 = toSelect;

            if (_selectedCard1 != null && _selectedCard2 != null)
            {
                
                if(_selectedCard1.transform.position.y >= 7)
                {
                    _selectedCard1.transform.position -= new Vector3(0, 2, 0);
                }
                
             
                SwitchCards(_selectedCard1, _selectedCard2);
                _selectedCard1 = null;
                _selectedCard2 = null;
            }
        }
        
        
    }

    public void SetVolumes()
    {
        SFX1.volume = SFX.value;
        SFX2.volume = SFX.value;
        Music.volume = Tunes.value;
    }


    public void SwitchCards(Card c1, Card c2)
    {
        if(c1 != c2)
        {
            
            if (!c1.isMoving && !c2.isMoving)
            {
                if (_hand.Contains(c1) && Field.Contains(c2))
                {
                    SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);
                    c1.LerpPos(c2.transform.position);
                    c2.LerpPos(c1.transform.position);
                    _hand.Insert(_hand.IndexOf(c1), c2);
                    Field.Insert(Field.IndexOf(c2), c1);
                    _hand.Remove(c1);
                    Field.Remove(c2);

                }
                else if (Field.Contains(c1) && _hand.Contains(c2))
                {
                    SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);
                    c1.LerpPos(c2.transform.position);
                    c2.LerpPos(c1.transform.position);
                    _hand.Insert(_hand.IndexOf(c2), c1);
                    Field.Insert(Field.IndexOf(c1), c2);
                    _hand.Remove(c2);
                    Field.Remove(c1);
                }
                else if (_hand.Contains(c1) && _hand.Contains(c2))
                {
                    SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);
                    c1.LerpPos(c2.transform.position);
                    c2.LerpPos(c1.transform.position);
                    Swap<Card>(_hand, _hand.IndexOf(c1), _hand.IndexOf(c2));
                }
                else if (_hand.Contains(c1) && BattleField.Contains(c2))
                {
                    SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);

                    _hand.Insert(_hand.IndexOf(c1), c2);
                    BattleField.Insert(BattleField.IndexOf(c2), c1);
                    _hand.Remove(c1);
                    BattleField.Remove(c2);
                    c1.LerpPos(c2.transform.position);
                    c2.LerpPos(c1.transform.position);
                    indexBuffer = _hand.IndexOf(c2);
                    Invoke("FixHandPos", 0.1f);
                }
                else if (_hand.Contains(c2) && BattleField.Contains(c1))
                {
                    SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);

                    _hand.Insert(_hand.IndexOf(c2), c1);
                    BattleField.Insert(BattleField.IndexOf(c1), c2);
                    _hand.Remove(c2);
                    BattleField.Remove(c1);
                    c1.LerpPos(c2.transform.position);
                    c2.LerpPos(c1.transform.position);
                    indexBuffer = _hand.IndexOf(c1);
                    Invoke("FixHandPos", 0.1f);
                }
                
            }
        }
        else if (Field.Contains(c2) && Field.Contains(c1) && c1 == c2)
        {
            if (takeCount < 2)
            {
                TakeCard(c1);
                takeCount++;
            }
    }

    }

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public void StartGame()
    {
        MonthCheck();
        Book.Play("Open");
        Invoke("StartStage", 1f);
        StartButton.SetActive(false);
    }

    

    public void StartStage()
    {
        FieldRevealed = 0;
        mySquirrel.LerpPos(playerPoint.position);
        DeckPosition.gameObject.SetActive(true);
        SetField();
        DealHand();
    }

    public void StopMusic()
    {
        Music.Pause();
        Music.volume = 0;
    }

    public void StartMusic(int trackCount)
    {
        StopMusic();
        Music.volume = Tunes.value;
        Music.clip = Songs[trackCount];
        Music.Play();
        
    }


    public void MonthCheck()
    {
        if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 0)
            Month.text = "June";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 1)
            Month.text = "July";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 2)
            Month.text = "August";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 3)
            Month.text = "September";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 4)
            Month.text = "October";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 5)
            Month.text = "November";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 6)
            Month.text = "December";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 7)
            Month.text = "January";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 8)
            Month.text = "February";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 9)
            Month.text = "March";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 10)
            Month.text = "April";
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) == 11)
            Month.text = "May";

        if(isEndless)
        {
            Month.text += ", Year " + currentYear;
        }
        SongCheck();
    }


    public void SongCheck()
    {
        if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) < 3)
        {
            Page1.material = firstPages[0];
            Page2.material = secondPages[0];
            StopMusic();
            StartMusic(1);
        }
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) < 6)
        {
            Page1.material = firstPages[1];
            Page2.material = secondPages[1];
            StopMusic();
            StartMusic(2);
        }
        else if (Stages.Contains(currentStage) && Stages.IndexOf(currentStage) < 9)
        {
            Page1.material = firstPages[2];
            Page2.material = secondPages[2];
            StopMusic();
            StartMusic(3);
        }
        else
        {
            Page1.material = firstPages[0];
            Page2.material = secondPages[0];
            StopMusic();
            StartMusic(1);
        }
    }

    void ShuffleDeck(List<Card> Deck)
    {
        if(Deck.Count > 0)
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                Card temp = Deck[i];
                int randomIndex = Random.Range(i, Deck.Count);
                Deck[i] = Deck[randomIndex];
                Deck[randomIndex] = temp;
            }
        }
        else
        {
            Debug.Log("No cards :(");
        }
    }

    void DealHand()
    {
        
        if(_hand.Count == 0)
        {
            ShuffleDeck(_deck);
            StartCoroutine("DrawHand");
        }        
    }

    public void ResetBattle()
    {
        Invoke("SongCheck", 0.8f);
        SFX2.PlayOneShot(Sounds[10]);

        StopCoroutine("AttackCards");
        for (int i = 0; i < 3; i++)
        {
            GameObject toDelete = BattleField[0].gameObject;
            BattleField.Remove(BattleField[0]);
            Destroy(toDelete);
        }
        //BattleField.Clear();
        for(int i = 0; i < 3; i++)
        {
            GameObject newSlot = Instantiate(emptySlot, BattleFieldPoints[i]);
            BattleField.Add(newSlot.GetComponent<Card>());
        }
        Field.Remove(BattleCreature);
        Destroy(BattleCreature.gameObject);
        FightButton.SetActive(true);
        BattleCanvas.SetActive(false);
    }

    public void FightCreature()
    {
        FightButton.SetActive(false);       
        ClearEmpty();
        BattleCreature.Attack();
    }
    
    public void ClearEmpty()
    {
        List<Card> toSnip = new List<Card>();
        foreach (Card c in _hand)
        {
            if (c.Name == "Slot")
            {
                toSnip.Add(c);
            }
        }       

        foreach(Card c in toSnip)
        {
            RemoveCard(toSnip[toSnip.IndexOf(c)]);
        }

    }

    public void NextStage()
    {
        ContinueButton.SetActive(false);
        StartCoroutine("AddHandToDeck");        
    }

    public IEnumerator AddHandToDeck()
    {
        canClick = false;
        foreach (Card c in _hand)
        {
            c.LerpPos(DeckPosition.position);
            c.FlipCard();
            yield return new WaitForSeconds(0.4f);
            _deck.Add(c);
            updateDeckCount();
            c.gameObject.SetActive(false);
            playFlipSFX();
        }
        _hand.Clear();

        if (Stages.IndexOf(currentStage) < 8 && !isEndless)
        {

            currentStage = Stages[Stages.IndexOf(currentStage) + 1];
            ClearField();
            Book.Play("Close");
            Invoke("MonthCheck", 1.5f);
            Invoke("StartStage", 3.25f);
            canClick = true;
            StopCoroutine("AddHandToDeck");
        }
        else if(isEndless)
        {
            if (Stages.IndexOf(currentStage) < 11)
            {
                currentStage = Stages[Stages.IndexOf(currentStage) + 1];
            }
            else
            {
                currentStage = Stages[0];
                currentYear++;
            }
            ClearField();
            Book.Play("Close");
            Invoke("MonthCheck", 1.5f);
            Invoke("StartStage", 3.25f);
            canClick = true;
            StopCoroutine("AddHandToDeck");
        }
        else
        {
            ClearField();
            Invoke("endScreen", 3f);
            Book.Play("Close");            
            StopCoroutine("AddHandToDeck");            
        }

       
    }

    public void endScreen()
    {
        Page1.material = firstPages[3];
        Page2.material = secondPages[3];
        StopMusic();
        StartMusic(1);
        EndText.SetActive(true);
    }

    public void FixHandPos()
    {
        if (_hand.Count > 0)
        {
            if (indexBuffer <= _hand.Count)
            {
                _hand[indexBuffer].transform.position = (HandPoints[0].transform.position + new Vector3(Mathf.Abs(3 * indexBuffer * (HandPoints[0].transform.position.x / _hand.Count)), indexBuffer * 0.00001f, 0));
            }
        }
        indexBuffer = 0;
    }

    public void playFlipSFX()
    {
        SFX2.pitch = Random.Range(0.8f, 1f);
        SFX2.PlayOneShot(Sounds[3]);
    }

    public void playPickSFX()
    {
        SFX1.PlayOneShot(Sounds[Random.Range(1, 2)]);
    }
    void ClearField()
    {
        if (Field.Count > 0)
        {
            foreach (Card c in Field)
            {
                Destroy(c.gameObject);
            }
        }

        Field.Clear();
    }

    void TakeCard(Card toAdd)
    {
        if(Field.Contains(toAdd))
        {
            Invoke("playPickSFX", 0.3f);

            _hand.Add(toAdd);
            Field.Remove(toAdd);
            StartCoroutine("RecenterHand");

           // Invoke("FixHandPos", 0.1f);
        }
    }

    void SetField()
    {
        ShuffleDeck(currentStage.DayCards);
        ShuffleDeck(currentStage.NightCards);
        if(Field.Count > 0)
        {
            foreach (Card c in Field)
            {
                Destroy(c.gameObject);
            }
        }
        takeCount = 0;
        Field.Clear();



        if(Stages.Contains(currentStage) && Stages.IndexOf(currentStage) > 6)
        {
            Field.Add(Instantiate(currentStage.DayCards[Random.Range(0, currentStage.DayCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.DayCards[Random.Range(0, currentStage.DayCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.NightCards[Random.Range(0, currentStage.NightCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.NightCards[Random.Range(0, currentStage.NightCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.NightCards[Random.Range(0, currentStage.NightCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
        }
        else
        {
            Field.Add(Instantiate(currentStage.DayCards[Random.Range(0, currentStage.DayCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.DayCards[Random.Range(0, currentStage.DayCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.DayCards[Random.Range(0, currentStage.DayCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.NightCards[Random.Range(0, currentStage.NightCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
            Field.Add(Instantiate(currentStage.NightCards[Random.Range(0, currentStage.NightCards.Count)], FieldSpawnPos.transform.position, FieldSpawnPos.transform.rotation));
        }

        StartCoroutine("DrawField");
        
    }

    public IEnumerator RecenterHand()
    {
        if(_hand.Count > 0)
        {
            canClick = false;
            for (int i = 0; i < _hand.Count; i++)
            {
                _hand[i].LerpPos(HandPoints[0].transform.position + new Vector3(Mathf.Abs(3 * i * (HandPoints[0].transform.position.x / _hand.Count)), i * 0.025f, 0));
                yield return new WaitForSeconds(0.1f);
            }
            //canClick = true;
            
        }
        if(!BattleCreature) 
            canClick = true;
        StopCoroutine("RecenterHand");
    }

    public void CleanUp()
    {
        List<Card> toClean = new List<Card>();

        foreach (Card i in GameObject.FindObjectsOfType<Card>())
        {
            if(_hand.Contains(i) || _deck.Contains(i) || Field.Contains(i) || BattleCreature == i || mySquirrel == i)
            {

            }
            else
            {
                Destroy(i.gameObject);
            }
        }
        
    }

    public IEnumerator DrawHand()
    {
        canClick = false;
        handSize = 5;
        for (int i = 0; i < handSize; i++)
        {
            if (_deck.Count >= handSize)
            {
                if (!_deck[i].isActiveAndEnabled)
                    _deck[i].gameObject.SetActive(true);
                _hand.Add(Instantiate(_deck[i], DeckPosition.transform.position, DeckPosition.transform.rotation));
            }
            else
            {
                handSize = _deck.Count;
                if (!_deck[i].isActiveAndEnabled)
                    _deck[i].gameObject.SetActive(true);
                _hand.Add(Instantiate(_deck[i], DeckPosition.transform.position, DeckPosition.transform.rotation));
            }
        }
            

        for (int i = 0; i < _hand.Count; i++)
        {
            if (_deck.Count > 0)
            {
                playFlipSFX();
                _hand[i].FlipCard();
                _hand[i].LerpPos(HandPoints[0].transform.position + new Vector3(Mathf.Abs(3 * i * (HandPoints[0].transform.position.x / handSize)), i * 0.025f, 0));                
                _deck.Remove(_deck[0]);
                updateDeckCount();
                yield return new WaitForSeconds(0.25f);
            }
            
       }

        canClick = true;
        CleanUp();
        StopCoroutine("DrawHand");
    }

    public IEnumerator DrawField()
    {
        for (int i = 0; i < Field.Count; i++)
        {
            Field[i].LerpPos(FieldPoints[i].transform.position);
            yield return new WaitForSeconds(0.25f);
        }
        StopCoroutine("DrawField");
    }

    public void DiscardRandom()
    {
        if(_hand.Count > 0)
        {
            int rando = Random.Range(0, _hand.Count);
            Card theCard = _hand[rando];
            _hand.RemoveAt(rando);
            GameObject SnipCard = Instantiate(SnippedCard, theCard.transform.position, theCard.transform.rotation);
            Destroy(SnipCard, 1f);
            Destroy(theCard.gameObject);
            SFX1.PlayOneShot(Sounds[6]);
        }            
            StartCoroutine("RecenterHand");
    }


    public void CenterHand()
    {
        StartCoroutine("RecenterHand");
    }

    public void RemoveCard(Card toRemove)
    {
        if(_hand.Contains(toRemove))
        {
            _hand.RemoveAt(_hand.IndexOf(toRemove));
        }        
        GameObject SnipCard = Instantiate(SnippedCard, toRemove.transform.position, toRemove.transform.rotation);
        Destroy(SnipCard, 1f);
        Destroy(toRemove.gameObject);
        StartCoroutine("RecenterHand");
        SFX1.PlayOneShot(Sounds[6]);
    }
}
