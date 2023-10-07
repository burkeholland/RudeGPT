using RudeGPT.Shared;
using System.Text.Json;
using Microsoft.JSInterop;

public class StateContainer
{
    private ChatSession chatSession = new ChatSession();

    private List<ChatSession> chatHistory = new List<ChatSession>();

    private bool sideBarIsOpen = false;

    private IJSRuntime _jsRuntime;

    public StateContainer(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ChatSession ChatSession
    {
        get => chatSession;
        set
        {
            chatSession = value;
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

    public void AddToChatHistory(ChatSession chatSession)
    {
        ChatHistory.Add(chatSession);
        NotifyStateChanged();
    }

    public void SaveHistoryToLocalStorage()
    {
        var json = JsonSerializer.Serialize(ChatHistory);
        _jsRuntime.InvokeVoidAsync("localStorage.setItem", "chatHistory", json);
    }

    public async Task LoadHistoryFromLocalStorage()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "chatHistory");
        if (!string.IsNullOrEmpty(json))
        {
            var chatHistory = JsonSerializer.Deserialize<List<ChatSession>>(json);
            ChatHistory = chatHistory ?? new List<ChatSession>();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}