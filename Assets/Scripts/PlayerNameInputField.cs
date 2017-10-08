using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.tutore.ColofulTCG
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        static string playerNamePrefkey = "PlayerName";

        void Start()
        {
            string defaultName = "";
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefkey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefkey);
                    _inputField.text = defaultName;
                }
            }
            PhotonNetwork.playerName = defaultName;
        }

        public void SetPlayerName(string value)
        {
            PhotonNetwork.playerName = value + " ";
            PlayerPrefs.SetString(playerNamePrefkey, value);
        }
    }

}