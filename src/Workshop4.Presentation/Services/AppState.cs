using Workshop4.Application.Pipelines.Nodes;
using Workshop4.Presentation.Models;

namespace Workshop4.Presentation.Services;

public sealed class AppState
{
    private static readonly Lazy<AppState> LazyInstance = new(
        () => new AppState(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    private Stack<GroupNode> _navigationStack = [];
    private Stack<GroupNode>? _backupNavigationStack;

    private AppState() { }

    public static AppState Instance => LazyInstance.Value;

    public bool IsExecuting => ExecutionState is ExecutionState.Run or ExecutionState.Debug;

    public ExecutionState ExecutionState { get; set; }

    public string InputJson { get; set; } = MockFactory.MockInput;

    public string OutputText { get; set; } = string.Empty;

    public GroupNode RootGroup { get; private set; } = new();

    public GroupNode CurrentGroup => _navigationStack.TryPeek(out GroupNode? node) ? node : RootGroup;

    public void Initialize(GroupNode rootGroup)
    {
        _navigationStack.Clear();
        _navigationStack.Push(rootGroup);

        RootGroup = rootGroup;
    }

    public void NavigateTo(GroupNode node)
    {
        _navigationStack.Push(node);
    }

    public void NavigateBack()
    {
        if (CurrentGroup != RootGroup)
            _navigationStack.TryPop(out _);
    }

    public void BackupNavigationStack()
    {
        _backupNavigationStack = new Stack<GroupNode>(_navigationStack);
        _navigationStack = new Stack<GroupNode>([RootGroup]);
    }

    public void RestoreNavigationStack()
    {
        if (_backupNavigationStack is not null)
            _navigationStack = new Stack<GroupNode>(_backupNavigationStack);
    }
}
