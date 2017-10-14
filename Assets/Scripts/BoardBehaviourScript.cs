using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.tutore.ColofulTCG
{
    public class BoardBehaviourScript : MonoBehaviour
    {
        public static BoardBehaviourScript instance;

        public Transform BlueDeckPos;
        public Transform BlueHandPos;
        //public Transform BlueManaPos;
        public Transform RedDeckPos;
        public Transform RedHandPos;
        //public Transform RedManaPos;
        
        public GameObject HeroPrefab;
        public GameObject[] CardPrefab;
        //public GameObject ManaPrefab;

        public TextMesh TimeText;
        public TextMesh BlueManaText;
        public TextMesh RedManaText;
        public TextMesh MyManaText;

        private List<GameObject> MyDeckCards = new List<GameObject>();
        private List<GameObject> MyHandCards = new List<GameObject>();

        private GameObject MyHero;
        //private GameObject MyMana;
        
        private Transform MyDeckPos;
        private Transform MyHandPos;
        //private Transform MyManaPos;

        public int turn = 1;
        public PunTeams.Team WhoTurn;

        public int maxMana = 0;
        public int Mana = 0;
        
        float f_time;
        static int time;

        PhotonView photonView;
        
        void Awake()
        {
            PhotonNetwork.OnEventCall += this.OnEvent; // raiseevent

            instance = this;
            // 팀을 설정한다
            InitiateTeam();
        }
        
        void Start()
        {
            // 영웅을 생성한다
            CreateHero();
            // 카드를 생성하여 덱을 만든다
            CreateDeck();
            // 마나를 갱신한다
            PhotonNetwork.RaiseEvent(0, new object[] { PhotonNetwork.player.GetTeam(), Mana, maxMana }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache });
            // 두 플레이어 중 마스터만 실행하여 양 측의 플레이어가 한 번만 턴을 갱신하도록 한다
            if (PhotonNetwork.isMasterClient)
                PhotonNetwork.RaiseEvent(1, new object[] { turn }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache });

            StartGame();
        }
        
        // instant gui -> you have to make button image
        void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 6 - 25, 100, 50), "End Turn"))
            {
                EndTurn();
            }
        }

        private void OnEvent(byte eventcode, object content, int senderid)
        {
            object[] data = (object[])content;
            if ( eventcode == 0 )
            {
                Debug.Log("OnRaiseEvent: setmanatext");
                PunTeams.Team team = (PunTeams.Team)data[0];
                if ( team == PunTeams.Team.blue )
                    this.BlueManaText.text = ((int)data[1]).ToString() + "/" + ((int)data[2]).ToString();
                else if (team == PunTeams.Team.red)
                    this.RedManaText.text = ((int)data[1]).ToString() + "/" + ((int)data[2]).ToString();
                else
                    Debug.Log("fail setmanatext");
            }
            else if ( eventcode == 1 )
            {
                Debug.Log("OnRaiseEvent: changeturn");
                turn = (int)data[0];
                if (turn % 2 == 1) WhoTurn = PunTeams.Team.blue;
                else WhoTurn = PunTeams.Team.red;

                // New Turn
                if (WhoTurn != PhotonNetwork.player.GetTeam()) return;
                Debug.Log(PhotonNetwork.player.GetTeam().ToString() + "'s turn start");

                // 최대마나를 1 늘리고 마나를 2 회복시킨다
                if (maxMana < 10) maxMana++;
                if (Mana + 2 <= maxMana) Mana += 2;
                else if (Mana + 1 <= maxMana) Mana += 1;
                PhotonNetwork.RaiseEvent(0, new object[] { PhotonNetwork.player.GetTeam(), Mana, maxMana }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache });

                // 카드를 한 장 뽑는다
                DrawCardFromDeck();
            }
        }

        /*
        public void EndGame()
        {
            if (winner == BlueHero)
            {
                Debug.Log("MyHero Win");
                Time.timeScale = 0;
                //Destroy(this);
            }

            if (winner == RedHero)
            {
                Time.timeScale = 0;
                Debug.Log("AIHero Win");
                //Destroy(this);
            }
            GameManager.Instance.LeaveRoom();
        }
        */
        void InitiateTeam()
        {
            // 룸에 인원이 두 명 뿐이므로 마스터 클라이언트와 로컬 클라이언트로 팀을 구분한다 
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
                MyDeckPos = BlueDeckPos;
                MyHandPos = BlueHandPos;
                //MyManaPos = BlueManaPos;
                MyManaText = BlueManaText;
            }
            else if (PhotonNetwork.isNonMasterClientInRoom)
            {
                PhotonNetwork.player.SetTeam(PunTeams.Team.red);
                MyDeckPos = RedDeckPos;
                MyHandPos = RedHandPos;
                //MyManaPos = RedManaPos;
                MyManaText = RedManaText;
            }
            else
            {
                Debug.Log("player can't matching");
            }

        }

        void CreateHero()
        {
            // 현재 플레이어의 팀을 확인해서 해당 진영에 영웅을 생성한다
            if (PhotonNetwork.player.GetTeam() == PunTeams.Team.blue)
            {
                MyHero = PhotonNetwork.Instantiate("Heros/" + this.HeroPrefab.name, new Vector3(-4f, 0f, -11f), Quaternion.identity, 0); //Resourses/Heros 폴더에 있는 프리팹을 가지고 온다
                MyHero.GetComponent<PhotonView>().RPC("SetHeroTeam", PhotonTargets.All, PunTeams.Team.blue);
            }
            else if (PhotonNetwork.player.GetTeam() == PunTeams.Team.red)
            {
                MyHero = PhotonNetwork.Instantiate("Heros/" + this.HeroPrefab.name, new Vector3(4f, 0f, -11f), Quaternion.identity, 0);
                MyHero.GetComponent<PhotonView>().RPC("SetHeroTeam", PhotonTargets.All, PunTeams.Team.red);
            }
            else
            {
                Debug.Log("can't matching hero's team");
            }
        }

        void CreateDeck()
        {
            // 추후 덱을 여러 개 만들 경우 CardPrefab[DeckNum][CardNum]으로 2차원 배열 형식으로 만들면 좋을 것 같다
            for (int i = 0; i < CardPrefab.Length; i++)
            {
                GameObject card;
                card = PhotonNetwork.Instantiate("Cards/" + this.CardPrefab[i].name, MyDeckPos.position, Quaternion.identity, 0);
                
                PhotonView pv = card.GetComponent<PhotonView>();
                MyDeckCards.Add(card);
                pv.RPC("SetNewPosition", PhotonTargets.All, MyDeckPos.position);
                pv.RPC("SetCardTeam", PhotonTargets.All, PhotonNetwork.player.GetTeam());
                pv.RPC("SetCardHero", PhotonTargets.All, MyHero.GetPhotonView().viewID);
            }
            // 현재 덱을 만들 때 랜덤으로 만드는 게 아니라 덱을 만들고 거기서 랜덤으로 뽑게 하고 있으며 추후 수정이 필요하다

        }

        public void StartGame()
        {
            // 맨 처음 3장씩 뽑는다
            for (int i = 0; i < 3; i++)
            {
                DrawCardFromDeck();
            }
            // 20초마다 NewTurn 함수를 호출하여 카드를 뽑고 마나를 채운다
            // InvokeRepeating("NewTurn", 20f, 20f);
        }

        void Update()
        {
            /*
            f_time += Time.deltaTime;
            time = Mathf.FloorToInt(f_time);
            TimeText.text = time.ToString();

            //this.photonView.RPC("SetManaText", PhotonTargets.All, Mana, maxMana);
            
            if (BlueHero.health <= 0 || RedHero.health <= 0)
                EndGame(BlueHero);
             */
        }

        // 현재 차례인 사람만 호출 가능
        void EndTurn()
        {
            if (WhoTurn != PhotonNetwork.player.GetTeam()) return;
            Debug.Log(PhotonNetwork.player.GetTeam().ToString() + "'s turn end");

            foreach (GameObject CardObject in GameObject.FindGameObjectsWithTag("Object"))
            {
                ObjectBehaviourScript obj = CardObject.GetComponent<ObjectBehaviourScript>();
                PhotonView objPv = CardObject.GetComponent<PhotonView>();
                if (obj.health <= 0 && obj.guard <= 0) obj.DestroyObject();
                    //objPv.RPC("DestroyObject", PhotonTargets.All);
            }

            MyHero.GetComponent<HeroBehaviourScript>().basicMove = true;
            turn++;
            PhotonNetwork.RaiseEvent(1, new object[] { turn }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache });
        }

        public void DrawCardFromDeck()
        {
            // 카드를 무작위로 뽑는다
            if (MyDeckCards.Count != 0 && MyHandCards.Count < 5)
            {
                int random = Random.Range(0, MyDeckCards.Count);
                GameObject tempCard = MyDeckCards[random];

                // 카드를 패로 갱신한다
                //tempCard.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position;
                tempCard.GetComponent<PhotonView>().RPC("SetNewPosition", PhotonTargets.All, MyHandPos.position);
                tempCard.GetComponent<CardBehaviourScript>().cardStatus = CardBehaviourScript.CardStatus.InHand;
                tempCard.transform.rotation = Quaternion.identity;
                //tempCard.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, 180.0f, 0.0f), Time.deltaTime * 3);

                // 카드를 뽑았으면 덱 리스트에서 제거하고 패 리스트에 추가한다
                MyDeckCards.Remove(tempCard);
                MyHandCards.Add(tempCard);
                HandPositionUpdate();
            }            
        }

        public void HandPositionUpdate()
        {
            // 패의 위치를 조정한다
            float space = 0f;
            float gap = -1.6f;

            foreach (GameObject card in MyHandCards)
            {
                //card.GetComponent<CardBehaviourScript>().newPos = MyHandPos.position + new Vector3((MyHandCards.Count - 1) * 0.9f + space, 0, 0);
                card.GetComponent<PhotonView>().RPC("SetNewPosition", PhotonTargets.All, MyHandPos.position + new Vector3((MyHandCards.Count - 1) * 0.9f + space, 0, 0));

                space += gap;
            }
        }

        public void PlaceCard(CardBehaviourScript card)
        {
            // 카드를 보드에 놓는다
            if (WhoTurn != PhotonNetwork.player.GetTeam()) return;
            if (card.team == PhotonNetwork.player.GetTeam() && Mana - card.mana >= 0)
            {
                // 카드를 냈으면 패 리스트에서 제거한다            
                MyHandCards.Remove(card.gameObject);

                // 오브젝트를 만든다
                if (card.cardType == CardBehaviourScript.CardType.Object)
                {
                    card.CreateObject();
                }
                else // 무브먼트를 사용한다
                {
                    card.DoMovement();
                }
                card.cardStatus = CardBehaviourScript.CardStatus.Destroyed;
                //Destroy(card.gameObject);
                //card.GetComponent<PhotonView>().RPC("DestroyCard", PhotonTargets.All);
                PhotonNetwork.Destroy(card.gameObject);

                Mana -= card.mana;
            }
            PhotonNetwork.RaiseEvent(0, new object[] { PhotonNetwork.player.GetTeam(), Mana, maxMana }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache });
            HandPositionUpdate();
        }
    }
}