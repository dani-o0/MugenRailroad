using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NeoFPS.Samples.SinglePlayer
{
    public class NpcConfirmationPopUp : MonoBehaviour
    {
        [SerializeField, Multiline, Tooltip("The info to display in the popup when interated with.")]
        private string m_Info = "Enter text here";

        public void Show()
        {
            ConfirmationPopup.ShowPopup(m_Info, null, null);
        }
    }
}
