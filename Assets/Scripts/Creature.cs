using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : Card
{
    public int ThreatLevel = 0;
    public bool isStarved = false;
    public bool isDead = false;
    public int EncounterCount = 0;

    public bool didHit = false;
    public bool hitAll = false;
    public bool isHealing = false;
    public bool isAttacking = false;
    public bool isDiscarding = false;

    private int cardToAttack = 0;
    public GameObject myEyes;
    public GameObject myMouth;

    private AudioSource myAS;
    public AudioClip talkSound;
    private float VolBuff;
    private float PitchBuff;

    public List<Texture2D> Eyes;
    public List<Texture2D> Mouth;

    private Texture2D _currentEye;
    private Texture2D _currentMouth;


    private string selectedText;
    private string currentText = "";
    public Text myText;

    private bool _isSpeaking = false;
    public float _textSpeed = 0.1f;


    public List<string> StartDialogue; //Lines to be said at the start of combat

    public List<string> FailDialogue; //Lines to be said if the player fails to block any damage

    public List<string> PartialDialogue; //Lines if player blocks some damage

    public List<string> FullDialogue; //Lines if players block all damage

    // Start is called before the first frame update
    void Start()
    {
        myEyes.GetComponent<Animator>().enabled = false;
        myEyes.GetComponent<Animator>().enabled = false;
        StartCoroutine("Blinking");
        myAS = GetComponent<AudioSource>();
        VolBuff = myAS.volume;
        PitchBuff = myAS.pitch;
    }

    private void OnEnable()
    {
        StartCoroutine("Blinking");
        myGm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnDisable()
    {
        StopCoroutine("Blinking");
    }

    private void OnDestroy()
    {
        StopCoroutine("Blinking");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            

        }
    }

    public void SayOpener()
    {
        sayLine(StartDialogue[Random.Range(0, StartDialogue.Count)]);
        _isSpeaking = true;
    }

    void Blink()
    {
        myEyes.GetComponent<Animator>().enabled = true;
        myEyes.GetComponent<Animator>().Play("Blink");          
    }

    void MoveMouth()
    {
        myEyes.GetComponent<Animator>().enabled = true;
        myMouth.GetComponent<Animator>().Play("Talk");
    }

    public void sayLine(string Line)
    {

        
            Debug.Log("Test");            
            selectedText = Line;
            _isSpeaking = true;
        StopCoroutine("Talking");
        StartCoroutine("Talking");

    }

    public void PlayTalkAudio(float Pitch)
    {        
        myAS.clip = talkSound;
        myAS.pitch = Pitch;
        myAS.UnPause();
        if (myAS.volume != 0)
        {
            myAS.volume = myGm.Voices.value;
            myAS.PlayOneShot(talkSound);
        }
        else
        {
            StopAudio();
            PlayTalkAudio(Pitch);
        }
    }

    public void StopAudio()
    {
        myAS.volume = 0f;
        myAS.Pause();
        myAS.time = 0f;
    }

    void updateTextBubble()
    {
        myText.text = currentText;
    }

    IEnumerator Talking()
    {
        currentText = "";
        foreach (char c in selectedText)
        {
            currentText = currentText + c;            
            updateTextBubble();            
            if (c == ",".ToCharArray()[0] || c == ".".ToCharArray()[0] || c == "!".ToCharArray()[0])
            {
                yield return new WaitForSeconds(_textSpeed * 3);
            }
            else if(c == "a".ToCharArray()[0] || c == "e".ToCharArray()[0] || c == "i".ToCharArray()[0] || c == "o".ToCharArray()[0] || c == "u".ToCharArray()[0] || c == "y".ToCharArray()[0])
            {
                MoveMouth();
                PlayTalkAudio(PitchBuff - 0.1f);
                yield return new WaitForSeconds(_textSpeed);
            }
            else
            {
                MoveMouth();
                PlayTalkAudio(PitchBuff);
                yield return new WaitForSeconds(_textSpeed);
            }
            
        }
        _isSpeaking = false;
        myGm.isTestTalk = false;
        StopCoroutine("Talking");                   
    }

    IEnumerator Blinking()
    {
        while(true)
        {
            Debug.Log("BlinkCheck");
            float blinktime = 3.4f;
            if (_isSpeaking)
            {
                blinktime = 2.1f;
            }
            Blink();
            yield return new WaitForSeconds(blinktime + Random.Range(-0.2f, 0.5f));
        }
        
    }

    public void Attack()
    {
        if(!isHealing && !isAttacking)
        {
            isAttacking = true;
            StartCoroutine("AttackCards");
        }
       
    }

    public IEnumerator HealCards()
    {
        isHealing = true;

            if(myGm.BattleField[cardToAttack]._healthPoints > 0)
            {
                if(myGm.mySquirrel._healthPoints < 5)
                {
                    myGm.mySquirrel._healthPoints = Mathf.Clamp(myGm.mySquirrel._healthPoints + myGm.BattleField[cardToAttack]._healthPoints, 0, 5);
                    myGm.BattleField[cardToAttack]._healthPoints = 0;
                }
                
                myGm.UpdateHealth();
                myGm.SFX2.PlayOneShot(myGm.Sounds[9]);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
            
        isHealing = false;
        isAttacking = true;
        StopCoroutine("HealCards");
    }

    public IEnumerator AttackCards()
    {
        myGm.canClick = false;
        for (int i = 0; i < myGm.BattleField.Count; i++)
        {
            cardToAttack = i;
            Windup();
            if (myGm.BattleField[cardToAttack]._survivalPoints == 0 && myGm.BattleField[cardToAttack]._healthPoints == 0 && cardToAttack != 2)
            {                
                yield return new WaitForEndOfFrame();
            }
            else
            {
                if (myGm.BattleField[cardToAttack]._healthPoints > 0)
                {
                    yield return StartCoroutine("HealCards");
                }

                                            
                if (ThreatLevel > 0)
                {
                    Invoke("HitCard", 0.2f);
                    Invoke("ReturnToSpace", 1f);
                    yield return new WaitForSeconds(1.2f);
                }
            }                       
        }
        yield return StartCoroutine("DiscardCards");
        isAttacking = false;
        myGm.ResetBattle();
        StopCoroutine("AttackCards");           
    }

    public IEnumerator DiscardCards()
    {
        int discardtotal = 0;
        foreach(Card c in myGm.BattleField)
        {
            discardtotal += c._discardNo;
        }

        for (int i = 0; i < discardtotal; i++)
        {
            myGm.DiscardRandom();
            yield return new WaitForSeconds(1f);
        }
        
        for (int i = 0; i < 1; i++)
        {            
            if (didHit)
            {
                sayLine(PartialDialogue[Random.Range(0, PartialDialogue.Count)]);
            }
            else if(hitAll)
            {
                sayLine(FailDialogue[Random.Range(0, FailDialogue.Count)]);
            }
            else
            {
                sayLine(FullDialogue[Random.Range(0, FullDialogue.Count)]);
            }
            
            yield return new WaitForSeconds(4.5f);
        }        
        myGm.CenterHand();
        myGm.canClick = true;
        StopCoroutine("DiscardCards");
    }

    public void OnHitSquirrel()
    {
        myGm.mySquirrel.sayLine(myGm.mySquirrel.FailDialogue[Random.Range(0, myGm.mySquirrel.FailDialogue.Count)]);
    }


    public void Windup()
    {
        LerpPos(myGm.CreatureFieldPoint.transform.position + (Vector3.forward + Vector3.up * 2));
    }

    public void HitCard()
    {
        LerpPos((myGm.BattleField[cardToAttack].transform.position + Vector3.up * 6) / 1.5f, 2);
    }

    public void ReturnToSpace()
    {
        if(!myGm.GameOver)
        {
            if (myGm.BattleField[cardToAttack]._survivalPoints >= ThreatLevel)
            {
                ThreatLevel = Mathf.Clamp(ThreatLevel - myGm.BattleField[cardToAttack]._survivalPoints, 0, 5);
                myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(8, 9)]);

            }
            else
            {

                int defenseTotal = 0;

                foreach(Card c in myGm.BattleField)
                {
                    defenseTotal += c._survivalPoints;
                }

                if (defenseTotal == 0)
                {
                    hitAll = true;
                }

                if (ThreatLevel < defenseTotal)
                {
                    myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(8,9)]);
                }                
                else if(cardToAttack == 0)
                {
                    if(myGm.BattleField[0]._survivalPoints >= ThreatLevel || myGm.BattleField[1]._survivalPoints > 0 || myGm.BattleField[2]._survivalPoints > 0)
                    {
                        myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(8, 9)]);
                        ThreatLevel = ThreatLevel - myGm.BattleField[cardToAttack]._survivalPoints;
                    }
                    else if(myGm.BattleField[cardToAttack]._survivalPoints < ThreatLevel)
                    {
                        Invoke("OnHitSquirrel", 1f);
                        myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(5, 6)]);
                        myGm.mySquirrel._healthPoints += myGm.BattleField[cardToAttack]._survivalPoints - ThreatLevel;
                        ThreatLevel = 0;
                        didHit = true;

                    }
                    
                }
                else if(cardToAttack == 1)
                {
                    if (myGm.BattleField[1]._survivalPoints >= ThreatLevel || myGm.BattleField[2]._survivalPoints > 0)
                    {
                        myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(8, 9)]);
                        ThreatLevel = ThreatLevel - myGm.BattleField[cardToAttack]._survivalPoints;

                    }
                    else if (myGm.BattleField[cardToAttack]._survivalPoints < ThreatLevel)
                    {
                        Invoke("OnHitSquirrel", 1f);
                        myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(5, 6)]);
                        myGm.mySquirrel._healthPoints += myGm.BattleField[cardToAttack]._survivalPoints - ThreatLevel;
                        ThreatLevel = 0;
                        didHit = true;

                    }
                }
                else if(cardToAttack == 2)
                {
                    if(myGm.BattleField[2]._survivalPoints >= ThreatLevel)
                    {
                        myGm.SFX2.PlayOneShot(myGm.Sounds[0]);
                    }
                    else
                    {
                        Invoke("OnHitSquirrel", 1f);
                        myGm.SFX2.PlayOneShot(myGm.Sounds[Random.Range(5, 6)]);
                        myGm.mySquirrel._healthPoints += myGm.BattleField[cardToAttack]._survivalPoints - ThreatLevel;
                        ThreatLevel = 0;
                        didHit = true;                       
                    }
                }
                  //BOOKMARK DOESN'T CALCULATE PROPERLY                
                
            }

            
        }
        LerpPos(myGm.CreatureFieldPoint.transform.position, 0.5f);
        myGm.UpdateHealth();        
    }

}
