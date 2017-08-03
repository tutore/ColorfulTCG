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
    
    // 소스
    public List<Texture2D> statusImagePrefab;

    public enum CardType { Object, Movement };
    public CardType cardType;
    public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
    public CardStatus cardStatus = CardStatus.InDeck;
    public enum Team { My, AI };
    public Team team = Team.My;

    public GameObject MyHero;
    public GameObject AIHero;

    public Vector3 newPos;
    float distance_to_screen;
    bool selected = false;

    // Use this for initializationx`
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
                transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);
            }
        }
        //Update Visuals
        nameText.text = _name.ToString();
        manaText.text = mana.ToString();
    }

    public void DoMovement()
    {
        GameObject hero;
        Vector3 movementPosition;
        HeroBehaviourScript heroData;
        Rigidbody2D heroRigidBody;

        if (team == Team.My)
        {
            hero = MyHero;
        }
        else
        {
            hero = AIHero;
        }

        movementPosition = hero.transform.position;
        heroData = hero.GetComponent<HeroBehaviourScript>();
        heroRigidBody = hero.GetComponent<Rigidbody2D>();

        if (description.Contains("도약"))
        {
            Debug.Log("도약");
            heroRigidBody.AddForce(Vector2.up * 30000);
            if (team == Team.My)
            {
                heroRigidBody.AddForce(Vector2.right * 600);
                movementPosition.x++;
                heroData.newPos = movementPosition;
            }
            else
            {
                heroRigidBody.AddForce(Vector2.right * (-600));
                movementPosition.x--;
                heroData.newPos = movementPosition;
            }
        }
        if (description.Contains("돌진"))
        {
            Debug.Log("돌진");
            heroRigidBody.AddForce(Vector2.up * 15000);
            if (team == Team.My)
            {
                heroRigidBody.AddForce(Vector2.right * 5000);
                movementPosition.x += 2;
                heroData.newPos = movementPosition;
            }
            else
            {
                heroRigidBody.AddForce(Vector2.right * (-5000));
                movementPosition.x -= 2;
                heroData.newPos = movementPosition;
            }
        }
    }

    public GameObject CreateObject()
    {
        GameObject obj;
        Vector3 objectPosition;
        ObjectBehaviourScript objectData;
        Rigidbody2D objRigidBody;
        BoxCollider2D objCollider;
        
        // 오브젝트가 생성될 위치를 정해준다
        if (team == Team.My)
        {
            objectPosition = MyHero.transform.position;
            objectPosition.x++;
        }
        else
        {
            objectPosition = AIHero.transform.position;
            objectPosition.x--;
        }

        obj = (GameObject)Instantiate(objectPrefab, objectPosition, Quaternion.identity);
        objectData = obj.GetComponent<ObjectBehaviourScript>();
        objectData.health = health;
        objectData.guard = guard;
        objectData.damage = damage;
        objRigidBody = obj.GetComponent<Rigidbody2D>();
        objCollider = obj.GetComponent<BoxCollider2D>();

        if (team == Team.AI) // 적의 오브젝트면 오브젝트의 좌우를 뒤집는다
        {
            obj.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }

        // 특수효과
        if (description.Contains("직사"))
        {
            Debug.Log("직사");
            objRigidBody.AddForce(obj.transform.right * 100);   // Vector2.right는 그냥 오른쪽, transform.right는 그 오브젝트의 오른쪽(앞)        
            objRigidBody.AddForce(obj.transform.up * 40);
        }
        if (description.Contains("곡사"))
        {
            Debug.Log("곡사");
            objRigidBody.AddForce(obj.transform.right * 100);            
            objRigidBody.AddForce(obj.transform.up * 80);
        }
        if (description.Contains("관통"))
        {
            Debug.Log("관통");
            objRigidBody.isKinematic = true;    // 오브젝트가 물리 작용을 안 받도록 해준다
            objCollider.isTrigger = true;       // 오브젝트가 충돌되지 않고 트리거만 발동하도록 해준다
        }
        // 특수효과 끝
        return obj;
    }

    void OnMouseDown()
    {
        Debug.Log("On Mouse Down Event");

        transform.localScale = new Vector3(transform.localScale.x * 2, transform.localScale.y * 2, transform.localScale.z);

        if (cardStatus == CardStatus.InHand)
        {
            selected = true;
        }

    }
    
    void OnMouseUp()
    {
        Debug.Log("On Mouse Up Event");

        transform.localScale = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) // 카드의 위치에서 보드 방향으로 ray를 쏜다
        {
            if (hit.transform.CompareTag("Field")) BoardBehaviourScript.instance.PlaceCard(this);
        }

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
