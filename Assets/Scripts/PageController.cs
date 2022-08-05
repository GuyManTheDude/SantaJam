using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{

    // 5 cards. 3 on Right, 2 on left
    public List<Card> CardsOnPage;
    //positions cards should move to
    public Transform CardPositions;

    //up to 3 cards, center of screen.
    public List<Card> CardsForBattle;
    //positions cards should move to
    public Transform PlayPositions;

    public Transform PlayerPosition;
    public Transform PlayerBattlePosition;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
