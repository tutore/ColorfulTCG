using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBehaviourScript : MonoBehaviour {

    public static BoardBehaviourScript instance;
    public Transform MyDeckPos;
    public Transform MyHandPos;
    public Transform MyTablePos;
    public Transform AIDeckPos;
    public Transform AIHandPos;
    public Transform AITablePos;

    public List<GameObject> MyDeckCards = new List<GameObject>();
    public List<GameObject> MyHandCards = new List<GameObject>();
    //public List<GameObject> MyTableCards = new List<GameObject>();
    public List<GameObject> AIDeckCards = new List<GameObject>();
    public List<GameObject> AIHandCards = new List<GameObject>();
    //public List<GameObject> AITableCards = new List<GameObject>();

    public HeroBehaviourScript MyHero;
    public HeroBehaviourScript AIHero;
    
    float f_time;
    public static int time;
    int maxMyMana = 1;
    int maxAIMana = 1;
    int MyMana = 1;
    int AIMana = 1;

    public TextMesh TimeText;
    public TextMesh MyManaText;
    public TextMesh AIManaText;

    public bool timeContinued = true;
    public bool gameStarted = false;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        foreach (GameObject CardObject in GameObject.FindGameObjectsWithTag("Card"))
        {
            CardObject.GetComponent<Rigidbody>().isKinematic = true;
            CardBehaviourScript c = CardObject.GetComponent<CardBehaviourScript>();

            // 서로의 덱의 카드를 찾아 리스트에 넣는다
            if (c.team == CardBehaviourScript.Team.My)
            {
                MyDeckCards.Add(CardObject);
                c.GetComponent<CardBehaviourScript>().newPos = MyDeckPos.position;
            }
            else
            {
                AIDeckCards.Add(CardObject);
                c.GetComponent<CardBehaviourScript>().newPos = AIDeckPos.position;
            }
        }
        
        //Start Game
        StartGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        UpdateGame();

        // 맨 처음 3장씩 뽑는다
        for (int i = 0; i < 3; i++)
        {
            DrawCardFromDeck(CardBehaviourScript.Team.My);
            DrawCardFromDeck(CardBehaviourScript.Team.AI);
        }
    }

    // Update is called once per frame
    void Update () {
        UpdateTime();
    }

    void UpdateTime()
    {        
        if ( timeContinued == true )
        {
            f_time += Time.deltaTime;
            time = Mathf.FloorToInt(f_time);
            TimeText.text = time.ToString();
        }
        if (time % 10 == 0 && time != 0)    // 10초마다 호출
        {
            NewTurn();
        }
    }

    void UpdateGame()
    {
        MyManaText.text = MyMana.ToString() + "/" + maxMyMana.ToString();
        AIManaText.text = AIMana.ToString() + "/" + maxAIMana.ToString();

        /*
        if (MyHero.health <= 0)
            EndGame(AIHero);
        if (AIHero.health <= 0)
            EndGame(MyHero);
            */

        //UpdateBoard();
    }
    /*
    public void EndGame(HeroBehaviourScript winner)
    {
        if (winner == MyHero)
        {
            Debug.Log("MyHero");
            Time.timeScale = 0;
            winnertext.text = "You Won";
            //Destroy(this);
        }

        if (winner == AIHero)
        {
            Time.timeScale = 0;
            Debug.Log("AIHero");
            winnertext.text = "You Losse";
            //Destroy(this);
        }
    }
    */

    void NewTurn ()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object"))
        {
            ObjectBehaviourScript objData = obj.GetComponent<ObjectBehaviourScript>();
            objData.SetColl(true); // 오브젝트의 충돌 판정을 다시 가능하게 한다
            if (objData != null)
            {
                if (objData.health <= 0 && objData.guard <= 0)
                {
                    Destroy(obj);
                }
            }
        }
        
        // 양 팀의 최대마나를 1 늘리고 마나를 1 회복시킨다
        if (maxMyMana < 10) maxMyMana++;
        if (MyMana + 1 <= maxMyMana) MyMana += 1;
        if (maxAIMana < 10) maxAIMana++;
        if (AIMana + 1 <= maxAIMana) AIMana += 1;

        // 양 팀이 카드를 한 장씩 뽑는다
        DrawCardFromDeck(CardBehaviourScript.Team.My);
        DrawCardFromDeck(CardBehaviourScript.Team.AI);
        // 영웅을 다시 이동 가능하게 한다
        MyHero.canMove = true;
        AIHero.canMove = true;

        HandPositionUpdate();
        //TablePositionUpdate();

        UpdateGame();

    }

    /*
void OnGUI ()
{
    if (gameStarted)
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 6 - 25, 100, 50), "End Turn"))
        {
            EndTurn();
        }
    }
}
    */

    public void DrawCardFromDeck(CardBehaviourScript.Team team)
    {
        // 카드를 뽑는다
        if (team == CardBehaviourScript.Team.My && MyDeckCards.Count != 0 && MyHandCards.Count <= 5)
        {
            int random = Random.Range(0, MyDeckCards.Count);
            GameObject tempCard = MyDeckCards[random];

            // 카드를 패로 옮긴다
            //tempCard.transform.position = MyHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);
            tempCard.transform.rotation = Quaternion.identity;
            //tempCard.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);

            // 카드를 뽑았으면 덱 리스트에서 제거하고 패 리스트에 추가한다
            MyDeckCards.Remove(tempCard);
            MyHandCards.Add(tempCard);
        }

        if (team == CardBehaviourScript.Team.AI && AIDeckCards.Count != 0 && AIHandCards.Count < 7)
        {
            int random = Random.Range(0, AIDeckCards.Count);
            GameObject tempCard = AIDeckCards[random];

            tempCard.transform.position = AIHandPos.position;
            tempCard.GetComponent<CardBehaviourScript>().SetCardStatus(CardBehaviourScript.CardStatus.InHand);
            tempCard.transform.rotation = Quaternion.identity;
            //tempCard.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);

            AIDeckCards.Remove(tempCard);
            AIHandCards.Add(tempCard);
        }
        HandPositionUpdate();
    }

    public void HandPositionUpdate()
    {
        // 손 패의 정보를 업데이트한다
        float space = 0f;
        float space2 = 0f;
        float gap = 1.8f;

        foreach (GameObject card in MyHandCards)
        {
            int numberOfCards = MyHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position + new Vector3(-(numberOfCards - 1) * 0.9f + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AIHandCards)
        {
            int numberOfCards = AIHandCards.Count;
            card.GetComponent<CardBehaviourScript>().newPos = AIHandPos.position + new Vector3(-(numberOfCards - 1) * 0.9f + space2, 0, 0);
            space2 += gap;
        }
    }

    /*
    public void TablePositionUpdate()
    {
        // 테이블의 정보를 업데이트한다
        float space = 0f;
        float space2 = 0f;
        float gap = 1.8f;

        foreach (GameObject card in MyTableCards)
        {
            int numberOfCards = MyTableCards.Count;
            //card.transform.position = myTablePos.position + new Vector3(-numberOfCards + space - 2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = MyTablePos.position + new Vector3(-(numberOfCards - 1) * 0.9f + space, 0, 0);
            space += gap;
        }

        foreach (GameObject card in AITableCards)
        {
            int numberOfCards = AITableCards.Count;
            //card.transform.position = AITablePos.position + new Vector3(-numberOfCards + space2,0,0);
            card.GetComponent<CardBehaviourScript>().newPos = AITablePos.position + new Vector3(-(numberOfCards - 1) * 0.9f + space2, 0, 0);
            space2 += gap;
        }
    }
    */


    public void PlaceCard(CardBehaviourScript card)
    {
        // 카드를 보드에 놓는다
        if (card.cardStatus == CardBehaviourScript.CardStatus.InHand && card.team == CardBehaviourScript.Team.My && MyMana - card.mana >= 0)
        {
            //card.GetComponent<CardBehaviourScript>().newPos = MyTablePos.position; // 카드의 새 위치를 테이블로 지정

            // 카드를 냈으면 패 리스트에서 제거하고 테이블 리스트에 추가한다            
            MyHandCards.Remove(card.gameObject);
            //MyTableCards.Add(card.gameObject);
            //card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);

            // 오브젝트를 만든다
            if (card.cardType == CardBehaviourScript.CardType.Object)
            {
                card.CreateObject();
            }
            else // 무브먼트를 사용한다
            {
                card.DoMovement();
            }
            card.SetCardStatus(CardBehaviourScript.CardStatus.Destroyed);
            Destroy(card.gameObject);

            MyMana -= card.mana;
        }

        if(card.cardStatus == CardBehaviourScript.CardStatus.InHand && card.team == CardBehaviourScript.Team.AI && AIMana - card.mana >= 0)
        {
            //card.GetComponent<CardBehaviourScript>().newPos = MyTablePos.position; // 카드의 새 위치를 테이블로 지정

            AIHandCards.Remove(card.gameObject);
            //AITableCards.Add(card.gameObject);
            //card.SetCardStatus(CardBehaviourScript.CardStatus.OnTable);

            // 오브젝트를 만든다
            if (card.cardType == CardBehaviourScript.CardType.Object)
            {
                card.CreateObject();
            }
            else // 무브먼트를 사용한다
            {
                card.DoMovement();
            }
            card.SetCardStatus(CardBehaviourScript.CardStatus.Destroyed);
            Destroy(card.gameObject);

            AIMana -= card.mana;
        }
        
        //TablePositionUpdate();
        HandPositionUpdate();
        UpdateGame();
    }

    /*
    void OnTriggerStay(Collider Obj)
    {
        // 드래그한 카드가 보드의 트리거를 켜면 카드를 놓는다.
        CardBehaviourScript card = Obj.GetComponent<CardBehaviourScript>();
        if (card.GetSelected() == false)
        { 
            card.PlaceCard();
        }
    }
    */
}
