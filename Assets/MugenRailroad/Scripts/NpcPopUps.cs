using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NeoFPS.Samples.SinglePlayer
{
    public enum DialogType
    {
        Info,
        Confirmation
    }

    [System.Serializable]
    public class DialogData
    {
        [SerializeField, Multiline, Tooltip("The text to display in the dialog")]
        public string text = "Enter text here";

        [SerializeField, Tooltip("The type of dialog to show")]
        public DialogType dialogType = DialogType.Info;

        [SerializeField, Tooltip("The function to execute when the confirmation dialog is accepted")]
        public UnityEvent onConfirmed = new UnityEvent();
    }

    public class NpcPopUps : MonoBehaviour
    {
        [SerializeField, Tooltip("The sequence of dialogs to show")]
        private DialogData[] m_DialogSequence = new DialogData[0];

        private int m_CurrentDialogIndex = 0;

        public void Show()
        {
            if (m_DialogSequence.Length > 0)
            {
                m_CurrentDialogIndex = 0;
                ShowCurrentDialog();
            }
        }

        private void ShowCurrentDialog()
        {
            if (m_CurrentDialogIndex < m_DialogSequence.Length)
            {
                var dialog = m_DialogSequence[m_CurrentDialogIndex];

                if (dialog.dialogType == DialogType.Info)
                {
                    InfoPopup.ShowPopup(dialog.text, OnDialogClosed);
                }
                else
                {
                    ConfirmationPopup.ShowPopup(dialog.text, OnConfirmed, OnCancelled);
                }
            }
        }

        private void OnDialogClosed()
        {
            m_CurrentDialogIndex++;
            ShowCurrentDialog();
        }

        private void OnConfirmed()
        {
            var dialog = m_DialogSequence[m_CurrentDialogIndex];
            dialog.onConfirmed?.Invoke();
            m_CurrentDialogIndex++;
            ShowCurrentDialog();
        }

        private void OnCancelled()
        {
            m_CurrentDialogIndex = 0;
        }
    }
}
