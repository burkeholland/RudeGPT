using RudeGPT.Shared;
using System.Text.Json;
using Microsoft.JSInterop;

public class StateContainer
{
    private ChatSession activeChat = new ChatSession();

    private List<ChatSession> chatHistory = new List<ChatSession>();

    private bool sideBarIsOpen = false;

    private IJSRuntime _jsRuntime;

    public StateContainer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ChatSession ActiveChat
    {
        get => activeChat;
        set
        {
            activeChat = value;
            NotifyStateChanged();
        }
    }

    public List<ChatSession> ChatHistory
    {
        get => chatHistory;
        set
        {
            chatHistory = value;
            NotifyStateChanged();
        }
    }

    public bool SideBarIsOpen
    {
        get => sideBarIsOpen;
        set
        {
            sideBarIsOpen = value;
            NotifyStateChanged();
        }
    }

    public void SetActiveChatSession(ChatSession chatSession)
    {
        ActiveChat = chatSession;
        SaveApplicationState();
    }

    public void StartNewChatSession()
    {
        if (ActiveChat.Messages.Count > 0)
        {
            ActiveChat = new ChatSession();
            SaveApplicationState();
        }
    }

    public void AddToChatHistory(ChatSession chatSession)
    {
        ChatHistory.Add(chatSession);
        SaveApplicationState();
        NotifyStateChanged();
    }

    public void RemoveFromChatHistory(ChatSession chatSession)
    {
        ChatHistory.Remove(chatSession);
        SaveApplicationState();
        NotifyStateChanged();
    }

    public void SaveApplicationState()
    {
        var json = JsonSerializer.Serialize(ChatHistory);
        _jsRuntime.InvokeVoidAsync("localStorage.setItem", "chatHistory", json);

        json = JsonSerializer.Serialize(ActiveChat);
        _jsRuntime.InvokeVoidAsync("localStorage.setItem", "activeChat", json);
    }

    public async Task LoadApplicationState()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "chatHistory");
        if (!string.IsNullOrEmpty(json))
        {
            var chatHistory = JsonSerializer.Deserialize<List<ChatSession>>(json);
            ChatHistory = chatHistory ?? new List<ChatSession>();
        }

        json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "activeChat");
        if (!string.IsNullOrEmpty(json))
        {
            var activeChat = JsonSerializer.Deserialize<ChatSession>(json);
            ActiveChat = activeChat ?? new ChatSession();
        }

        NotifyStateChanged();
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}