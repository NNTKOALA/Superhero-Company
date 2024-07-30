using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject player;
    public string username;

    [Header("Chat UI")]
    public GameObject chatPanel;
    public GameObject chatUI;
    public TMP_Text textPrefab;
    public TMP_InputField chatBox;
    public Color playerMessage, info;
    public ScrollRect chatScrollRect;

    [SerializeField] List<Message> messageList = new List<Message>();
    private bool isChatVisible = false;

    [Header("Character UI")]
    public TMP_Text characterText;
    public GameObject characterTextObject;
    public float messageDisplayDuration = 5f;

    [Header("Scroll Settings")]
    public float scrollSensitivity = 20f;
    public float decelerationRate = 0.135f;
    public float elasticity = 0.1f;
    public float fastScrollMultiplier = 3f;

    private const int MAX_MESSAGES = 50;
    private Coroutine clearCharacterTextCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        SetupScrollRect();
        InitializeChatUI();
        HideCharacterText();
    }

    void SetupScrollRect()
    {
        if (chatScrollRect != null)
        {
            chatScrollRect.scrollSensitivity = scrollSensitivity;
            chatScrollRect.decelerationRate = decelerationRate;
            chatScrollRect.elasticity = elasticity;
        }
    }

    void InitializeChatUI()
    {
        chatBox.gameObject.SetActive(true);
        chatUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (string.IsNullOrWhiteSpace(chatBox.text))
            {
                ToggleChatUI();
            }
            else
            {
                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                DisplayMessageAboveCharacter(chatBox.text);
                chatBox.text = "";
            }
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            HideChatUI();
        }

        if (isChatVisible)
        {
            HandleFastScrolling();
        }

        UpdateCharacterTextPosition();
    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void HandleFastScrolling()
    {
        if (chatScrollRect != null && Input.GetKey(KeyCode.LeftShift))
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0)
            {
                Vector2 newPosition = chatScrollRect.normalizedPosition;
                newPosition.y += scrollDelta * fastScrollMultiplier;
                chatScrollRect.normalizedPosition = newPosition;
            }
        }
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if (chatPanel == null || textPrefab == null)
        {
            Debug.LogError("Chat panel or text prefab is not assigned!");
            return;
        }

        Message newMessage = new Message();
        newMessage.text = text;
        TMP_Text newText = Instantiate(textPrefab, chatPanel.transform);
        newMessage.textObject = newText;
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);
        messageList.Add(newMessage);

        TrimExcessMessages();

        Canvas.ForceUpdateCanvases();
        ScrollToBottom();
    }

    void ScrollToBottom()
    {
        StartCoroutine(SmoothScrollToPosition(0f));
    }

    private IEnumerator SmoothScrollToPosition(float targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.3f;
        float startPosition = chatScrollRect.verticalNormalizedPosition;

        while (elapsedTime < duration)
        {
            chatScrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        chatScrollRect.verticalNormalizedPosition = targetPosition;
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        return messageType == Message.MessageType.playerMessage ? playerMessage : info;
    }

    private void ToggleChatUI()
    {
        isChatVisible = !isChatVisible;
        chatUI.SetActive(isChatVisible);

        if (isChatVisible)
        {
            chatBox.ActivateInputField();
        }
    }

    public void OnChatBoxClick()
    {
        if (!isChatVisible)
        {
            ShowChatUI();
        }
    }

    private void ShowChatUI()
    {
        isChatVisible = true;
        chatUI.SetActive(true);
        chatBox.ActivateInputField();
    }

    private void HideChatUI()
    {
        isChatVisible = false;
        chatUI.SetActive(false);
    }

    private void DisplayMessageAboveCharacter(string message)
    {
        if (characterText != null && characterTextObject != null)
        {
            characterText.text = message;
            characterTextObject.SetActive(true);

            if (clearCharacterTextCoroutine != null)
            {
                StopCoroutine(clearCharacterTextCoroutine);
            }

            clearCharacterTextCoroutine = StartCoroutine(ClearCharacterTextAfterDelay(messageDisplayDuration));
        }
    }

    private void UpdateCharacterTextPosition()
    {
        if (player != null && characterTextObject != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.transform.position);
            characterTextObject.transform.position = screenPos + new Vector3(0, 400, 0);
        }
    }

    private void HideCharacterText()
    {
        if (characterTextObject != null)
        {
            characterTextObject.SetActive(false);
        }
    }

    private IEnumerator ClearCharacterTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideCharacterText();
        clearCharacterTextCoroutine = null;
    }

    private void TrimExcessMessages()
    {
        if (messageList.Count > MAX_MESSAGES)
        {
            int excessCount = messageList.Count - MAX_MESSAGES;
            for (int i = 0; i < excessCount; i++)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.RemoveAt(0);
            }
        }
    }
}

public class Message
{
    public string text;
    public TMP_Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info
    }
}