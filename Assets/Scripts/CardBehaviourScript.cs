using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviourScript : MonoBehaviour {

    public string _name = "";
    public override string ToString()
    {
        return _name;
    }
    public Texture2D image;
    public string description = "description";
    public int mana;
    public enum CardStatus { InDeck, InHand, OnTable, Destroyed};
    public CardStatus cardStatus = CardStatus.InDeck;
    public enum Team { My, AI };
    public Team team = Team.My;

    public TextMesh nameText;
    public TextMesh manaText;
    public TextMesh DescriptionText;

    public Vector3 newPos;
    float distance_to_screen;
    bool selected = false;

    // Use this for initialization
    void Start ()
    {
        distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;
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
        }
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
