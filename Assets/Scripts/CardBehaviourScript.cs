using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.tutore.ColofulTCG
{
    public class CardBehaviourScript : MonoBehaviour
    {

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

        public int health, guard, damage;
        public enum ObjectStatus { Health, Guard, Damage }; // TextMesh List의 index
        public List<TextMesh> statusText;

        public enum CardType { Object, Movement };
        public CardType cardType;
        // 카드 정보 끝

        public GameObject objectPrefab;

        public PhotonView cardPhotonView;

        public enum CardStatus { InDeck, InHand, OnTable, Destroyed };
        public CardStatus cardStatus = CardStatus.InDeck;
        public PunTeams.Team team;

        public GameObject MyHero;

        // 카드가 선택 중임을 나타낸다
        bool selected = false;

        public Vector3 handPos;
        public Vector3 newPos;
        float distance_to_screen;

        // Use this for initializationx`
        void Start()
        {
            this.GetComponent<Rigidbody>().isKinematic = true;

            distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z - 1;

            InitiateCardText();
            
            cardPhotonView = GetComponent<PhotonView>();
        }

        void InitiateCardText()
        {
            manaText.text = mana.ToString();
            if (health > 0) statusText[(int)ObjectStatus.Health].text = health.ToString();
            if (guard > 0) statusText[(int)ObjectStatus.Guard].text = guard.ToString();
            if (damage > 0) statusText[(int)ObjectStatus.Damage].text = damage.ToString();
            DescriptionText.text = description.ToString();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // selected일 때는 드래그하는 곳으로 카드가 이동되어야 한다
            if (!selected)
            {
                SetCardTransform();
            }
            //Update Visuals
            nameText.text = _name.ToString();
            manaText.text = mana.ToString();

        }
        
        [PunRPC]
        public void SetCardTeam(PunTeams.Team getTeam)
        {
            this.team = getTeam;
        }

        [PunRPC]
        public void SetCardHero(int id)
        {
            this.MyHero = PhotonView.Find(id).gameObject;
        }

        [PunRPC]
        public void SetNewPosition(Vector3 pos)
        {
            this.newPos = pos;
        }

        void SetCardTransform()
        {
            // 카드 좌표를 수정한다
            // Debug.Log(ToString() + "'s SetCardTransform() : " + transform.position + " -> " + pos);
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 3);
            // 덱에 있다면 카드를 뒷면으로 한다
            if (cardStatus == CardStatus.InDeck)
            {
                transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);
            }
            // 내 카드이면서 패에 있다면 앞으로 되돌린다
            else if (cardPhotonView.isMine == true && cardStatus == CardStatus.InHand)
            {
                transform.rotation = Quaternion.identity;
            }
        }


        // 무브먼트 카드를 사용한다
        public void DoMovement()
        {
            Vector3 movementPosition;
            HeroBehaviourScript heroData;
            Rigidbody2D heroRigidBody;
            
            heroData = MyHero.GetComponent<HeroBehaviourScript>();
            heroRigidBody = MyHero.GetComponent<Rigidbody2D>();

            if (description.Contains("도약"))
            {
                Debug.Log("도약");
                heroRigidBody.AddForce(Vector2.up * 15000);
                heroRigidBody.AddForce(heroData.direction * 6000);
            }
            if (description.Contains("돌진"))
            {
                Debug.Log("돌진");
                heroRigidBody.AddForce(Vector2.up * 10000);
                heroRigidBody.AddForce(heroData.direction * 10000);
            }
        }

        // 오브젝트 카드를 사용한다
        public GameObject CreateObject()
        {
            GameObject obj;
            Vector3 objectPosition;
            ObjectBehaviourScript objectData;
            Rigidbody2D objRigidBody;
            BoxCollider2D objCollider;

            // 오브젝트가 생성될 위치를 정해준다
            objectPosition = MyHero.transform.position;
            if (team == PunTeams.Team.blue)
                objectPosition.x += 2;
            else if (team == PunTeams.Team.red)
                objectPosition.x -= 2;
            // 오브젝트를 만들고 그 능력치를 카드의 능력치와 같게 해준다
            obj = PhotonNetwork.Instantiate("Objects/" + this.objectPrefab.name, objectPosition, Quaternion.identity, 0);            
            objectData = obj.GetComponent<ObjectBehaviourScript>();
            objectData.health = health;
            objectData.guard = guard;
            objectData.damage = damage;
            objRigidBody = obj.GetComponent<Rigidbody2D>();
            objCollider = obj.GetComponent<BoxCollider2D>();

            if (team == PunTeams.Team.red) // 레드팀의 오브젝트면 오브젝트의 좌우를 뒤집는다
            {
                obj.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }

            // 특수효과
            if (description.Contains("직사"))
            {
                Debug.Log("직사");
                objRigidBody.AddForce(obj.transform.right * 200);   // Vector2.right는 그냥 오른쪽, transform.right는 그 오브젝트의 오른쪽(앞)        
                objRigidBody.AddForce(obj.transform.up * 80);
            }
            if (description.Contains("곡사"))
            {
                Debug.Log("곡사");
                objRigidBody.AddForce(obj.transform.right * 200);
                objRigidBody.AddForce(obj.transform.up * 120);
            }
            if (description.Contains("관통"))
            {
                Debug.Log("관통");
                objRigidBody.isKinematic = true;    // 오브젝트가 물리 작용을 안 받도록 해준다
                objCollider.isTrigger = true;       // 오브젝트가 충돌되지 않고 트리거만 발동하도록 해준다
            }
            // 특수능력 끝
            return obj;
        }

        void OnMouseDown()
        {
            Debug.Log("On Mouse Down Event");

            // 누르고 있는 동안 카드를 확대시킨다
            if( cardStatus != CardStatus.InDeck )
            {
                transform.localScale = new Vector3(transform.localScale.x * 2, transform.localScale.y * 2, transform.localScale.z);

                if (cardStatus == CardStatus.InHand)
                {
                    selected = true;
                }
            }

        }

        void OnMouseUp()
        {
            Debug.Log("On Mouse Up Event");

            if (cardStatus != CardStatus.InDeck)
                transform.localScale = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z);
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit)) // 카드의 위치에서 보드 방향으로 ray를 쏜다
            {
                if (hit.transform.CompareTag("Field")) BoardBehaviourScript.instance.PlaceCard(this);
            }

            selected = false;

        }
        void OnMouseDrag()
        {
            //Debug.Log("On Mouse Drag Event");
            if (cardPhotonView.isMine == true && selected == true)
            {
                GetComponent<Rigidbody>().MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen)));
            }
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
    }
}