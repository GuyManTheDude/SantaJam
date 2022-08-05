using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameManager myGm;

    public Vector3 setPosition;
    public bool Revealed = true;
    public bool isFlipping = false;
    public bool isMoving = false;
    public bool beingDragged = false;

    public float lerpSpeed = 1.0f;
    public string Name;
    public int _survivalPoints = 0;
    public int _healthPoints = 0;
    public int _discardNo = 0;
    public List<string> OnPlayText;
    public bool isPicked = false;

    private void Start()
    {
        myGm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }

    private void OnEnable()
    {
        myGm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (setPosition != Vector3.zero)
        {
            transform.position = setPosition;
        }
        setPosition = transform.position; // Debug for now
       
    }

    public void Update()
    {
        //if (Input.touchCount == 1)
        //{
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //        beingDragged = true;
        //    if (Input.GetTouch(0).phase == TouchPhase.Ended)
        //    {
        //        beingDragged = false;
        //    }
        //}
    }

    public void OnMouseDown()
    {
        if(myGm.canClick)
        {
            if (Revealed)
            {
                if (!this.GetComponent<Creature>())
                {
                    myGm.SelectCard(this);
                }

            }
            else
            {
                if (!isFlipping)
                {
                    myGm.playFlipSFX();
                    FlipCard();
                    myGm.FieldRevealed++;
                    if (myGm.FieldRevealed >= 3)
                    {
                        myGm.NextStageButton.SetActive(true);
                    }
                    if (this.GetComponent<Creature>())
                    {
                        this.GetComponent<Creature>().Invoke("SayOpener", 1f);
                        myGm.mySquirrel.sayLine("...");
                        myGm.isBattling = true;
                        myGm.canClick = false;
                        Invoke("BattleStart", 0.5f);

                    }
                    else
                    {
                        if (OnPlayText.Count > 0)
                        {
                            myGm.mySquirrel.sayLine(OnPlayText[Random.Range(0, OnPlayText.Count)]);
                        }
                    }
                }

            }
        }
        
    }

    public void BattleStart()
    {
        myGm.canClick = true;
        for (int i = 0; i < myGm.Field.Count; i++)
        {
            myGm.Field[i].transform.position = myGm.FieldPoints[i].transform.position;
        }
        LerpPos(myGm.CreatureFieldPoint.transform.position);
        myGm.BattleCreature = this.GetComponent<Creature>();
        myGm.BattleCanvas.SetActive(true);
        myGm.StartMusic(0);
        
    }

    public void LerpPos(Vector3 newPos)
    {
        lerpSpeed = 1.0f;
        setPosition = newPos;
        StartCoroutine("LerpPosition");
    }

    public void LerpPos(Vector3 newPos, float newSpeed)
    {
        lerpSpeed = newSpeed;
        setPosition = newPos;
        StartCoroutine("LerpPosition");
    }

    public void FlipCard()
    {
        Revealed = !Revealed;
        StartCoroutine("FlipTime");
    }

    IEnumerator FlipTime()
    {
        Debug.Log("Flipped");
        float elapsedTime = 0;
        isFlipping = true;
        if (!Revealed)
        {

            while (elapsedTime < 0.5f)
            {
                
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, 180f, 180f)), elapsedTime);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.rotation.eulerAngles.Set(transform.rotation.x, 180f, 180f);
        }
        if(Revealed)
        {
            while (elapsedTime < 0.5f)
            {

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, 180f, 0.0f)), 180 * elapsedTime * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.rotation.eulerAngles.Set(transform.rotation.x, 180f, 0.0f);
        }
        isFlipping = false;
        StopCoroutine("FlipTime");
    }

    IEnumerator LerpPosition()
    {
        Debug.Log("Moving");
        float elapsedTime = 0;        
        if (!isMoving)
        {
            isMoving = true;
            while (elapsedTime < 0.5f && Vector3.Distance(transform.position, setPosition) > 0.1f)
            {

                transform.position = Vector3.Lerp(transform.position, setPosition, 180 * elapsedTime * lerpSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.position = setPosition;
        }
        //if (isMoving)
        //{
        //    while (elapsedTime < 0.5f)
        //    {

        //        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, 180f, 0.0f)), elapsedTime / 1.5f);
        //        elapsedTime += Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }
        //    transform.rotation.eulerAngles.Set(transform.rotation.x, 180f, 0.0f);
        //}
        isMoving = false;
        StopCoroutine("LerpPosition");
    }


}
