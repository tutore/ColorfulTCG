using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviourScript : MonoBehaviour {

    // 카드 정보
    public string _name = "";
    public override string ToString()
    {
        return _name;
    }
    public TextMesh nameText;
    public string description = "description";
    public TextMesh DescriptionText;
    public int mana;
    public TextMesh manaText;
    public Texture2D manaImage;
    public enum ObjectStatus { Health, Guard, Damage };
    public int health, guard, damage;
    public List<TextMesh> statusText;
    public List<Texture2D> statusImage;

    public GameObject objectPrefab;
    ObjectBehaviourScript objectData;

    // 소스
    public List<Texture2D> statusImagePrefab;

    public enum CardStatus { InDeck, InHand, OnTable, Destroyed};
    public CardStatus cardStatus = CardStatus.InDeck;
    public enum Team { My, AI };
    public Team team = Team.My;

    public GameObject MyHero;
    public GameObject AIHero;

    public Vector3 newPos;
    float distance_to_screen;
    bool selected = false;

    // Use this for initialization
    void Start ()
    {
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;

        manaText.text = mana.ToString();
        if (health > 0) statusText[(int)ObjectStatus.Health].text = health.ToString();
        if (guard > 0) statusText[(int)ObjectStatus.Guard].text = guard.ToString();
        if (damage > 0) statusText[(int)ObjectStatus.Damage].text = damage.ToString();
        DescriptionText.text = description.ToString();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!selected)
        {
            // move position to newPos when mouse up
            //transform.position = newPos;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
            if (cardStatus == CardStatus.InDeck)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);
            }
        }
        //Update Visuals
        nameText.text = _name.ToString();
        manaText.text = mana.ToString();
    }

    public void PlaceCard()
    {
        if (BoardBehaviourScript.instance.turn == BoardBehaviourScript.Turn.MyTurn && cardStatus == CardStatus.InHand && team == Team.My)
        {
            //selected = false;
            BoardBehaviourScript.instance.PlaceCard(this);
            CreateObject();
        }
    }

    GameObject CreateObject()
    {
        Vector3 objectPosition = MyHero.transform.position;
        objectPosition.x++;
        GameObject obj = (GameObject)Instantiate(objectPrefab, objectPosition, Quaternion.identity);

        objectData = obj.GetComponent<ObjectBehaviourScript>();
        objectData.health = health;
        objectData.guard = guard;
        objectData.damage = damage;
        Rigidbody2D objRigidBody = obj.GetComponent<Rigidbody2D>();
        if ( description.Contains("직사") )
        {
            Debug.Log("직사");
            objRigidBody.AddForce(Vector2.right * 101);
        }
        return obj;
    }

    void OnMouseDown()
    {
        Debug.Log("On Mouse Down Event");
        if (cardStatus == CardStatus.InHand && team == Team.My)
        {
            selected = true;
        }

    }
    
    void OnMouseUp()
    {
        Debug.Log("On Mouse Up Event");
        selected = false;
        
    }
    /*
    void OnMouseOver()
    {

        Debug.Log("On Mouse Over Event");
    }

    void OnMouseEnter()
    {
        Debug.Log("On Mouse Enter Event");
        //newPos += new Vector3(0,0.5f,0);
    }

    void OnMouseExit()
    {
        Debug.Log("On Mouse Exit Event");
        //newPos -= new Vector3(0,0.5f, 0);
    }
    */
    void OnMouseDrag()
    {
        //Debug.Log("On Mouse Drag Event");
        if ( selected == true )
        {
            GetComponent<Rigidbody>().MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
        }
    }

    public void SetCardStatus(CardStatus status)
    {
        cardStatus = status;
    }

    public bool GetSelected()
    {
        return selected;
    }
    
}
