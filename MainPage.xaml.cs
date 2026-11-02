
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WidgetDemoApp;

public partial class MainPage : ContentPage
{
    // -------------------------------
    // DATA COLLECTIONS
    // -------------------------------

    public ObservableCollection<TaskItem> VisibleTasks { get; } = new();

    public ObservableCollection<string> Skills { get; } = new();

    public ObservableCollection<NoteItem> Notes { get; } = new();

    private readonly List<TaskItem> allTasks = new();

    // -------------------------------
    // COUNTERS
    // -------------------------------

    private int likeCount;
    private int presentCount;
    private int absentCount;

    // -------------------------------
    // CONSTRUCTOR
    // -------------------------------

    public MainPage()
    {
        InitializeComponent();

        BindingContext = this;

        InitializeDashboard();
    }

    private void InitializeDashboard()
    {
        // Header
        GreetingLabel.Text = "Hello, Akash!";
        DateLabel.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

        // Sample tasks
        allTasks.Add(new TaskItem("Complete .NET MAUI assignment"));
        allTasks.Add(new TaskItem("Practice Data Structures", true));
        allTasks.Add(new TaskItem("Prepare for technical interview"));
        allTasks.Add(new TaskItem("Revise Database Management"));

        // Sample skills
        Skills.Add("C#");
        Skills.Add(".NET MAUI");
        Skills.Add("Java");
        Skills.Add("SQL");
        Skills.Add("React");
        Skills.Add("Git");
        Skills.Add("DSA");

        // Sample notes
        Notes.Add(new NoteItem("Revise OOP concepts"));
        Notes.Add(new NoteItem("Practice SQL queries"));

        // Initial UI updates
        ApplyTaskSearch(string.Empty);
        UpdateTaskStatistics();
        UpdateAttendance();
    }

    // -------------------------------
    // PROFILE SECTION
    // -------------------------------

    private void OnLikeClicked(object? sender, EventArgs e)
    {
        likeCount++;

        LikeButton.Text = $"♥ Like ({likeCount})";
    }

    private async void OnMessageClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync(
            "Message",
            "Welcome to Student Hub!",
            "OK");
    }

    private async void OnNotificationsClicked(object? sender, EventArgs e)
    {
        string message = NotificationSwitch.IsToggled
            ? "You're all caught up. New study reminders will appear here."
            : "Notifications are turned off. You can enable them from your profile.";

        await DisplayAlertAsync("Notifications", message, "OK");
    }

    private async void OnViewProfileClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync(
            "Akash Patil",
            "Computer Engineering Student\n" +
            "Interested in software development\n" +
            "Skills: C#, .NET MAUI, Java, SQL",
            "Close");
    }

    // -------------------------------
    // COMMENT SECTION
    // -------------------------------

    private void OnCommentClicked(object? sender, EventArgs e)
    {
        PostComment();
    }

    private void OnCommentCompleted(object? sender, EventArgs e)
    {
        PostComment();
    }

    private void PostComment()
    {
        string comment = CommentEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(comment))
        {
            CommentLabel.Text = "Please enter a comment.";
            return;
        }

        CommentLabel.Text = $"Your comment: {comment}";

        CommentEntry.Text = string.Empty;
    }

    // -------------------------------
    // NOTIFICATION SETTINGS
    // -------------------------------

    private void OnSwitchToggled(object? sender, ToggledEventArgs e)
    {
        StatusLabel.Text = e.Value
            ? "Notifications are enabled."
            : "Notifications are disabled.";

        StatusLabel.TextColor = e.Value
            ? Colors.Green
            : Colors.Red;
    }

    // -------------------------------
    // TASK MANAGEMENT
    // -------------------------------

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        string title = TaskEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync(
                "Invalid Task",
                "Please enter a task name.",
                "OK");

            return;
        }

        // Prevent duplicate tasks
        bool alreadyExists = allTasks.Any(task =>
            task.Title.Equals(
                title,
                StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            await DisplayAlertAsync(
                "Duplicate Task",
                "This task already exists.",
                "OK");

            return;
        }

        allTasks.Add(new TaskItem(title));

        TaskEntry.Text = string.Empty;

        RefreshTasks();
    }

    private void OnTaskCheckedChanged(
        object? sender,
        CheckedChangedEventArgs e)
    {
        if (sender is not CheckBox checkBox ||
            checkBox.BindingContext is not TaskItem task)
        {
            return;
        }

        task.IsCompleted = e.Value;

        UpdateTaskStatistics();
    }

    private void OnDeleteTaskClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not TaskItem task)
        {
            return;
        }

        allTasks.Remove(task);

        RefreshTasks();
    }

    private void OnTaskSearchTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        ApplyTaskSearch(e.NewTextValue ?? string.Empty);
    }

    // Filter tasks according to search text
    private void ApplyTaskSearch(string searchText)
    {
        VisibleTasks.Clear();

        var filteredTasks = allTasks.Where(task =>
            task.Title.Contains(
                searchText.Trim(),
                StringComparison.OrdinalIgnoreCase));

        foreach (var task in filteredTasks)
        {
            VisibleTasks.Add(task);
        }
    }

    // Refresh list and statistics
    private void RefreshTasks()
    {
        ApplyTaskSearch(TaskSearchBar.Text ?? string.Empty);

        UpdateTaskStatistics();
    }

    private void UpdateTaskStatistics()
    {
        int total = allTasks.Count;

        int completed = allTasks.Count(task => task.IsCompleted);

        int pending = total - completed;

        TotalTasksLabel.Text = total.ToString();

        CompletedTasksLabel.Text = completed.ToString();

        PendingTasksLabel.Text = pending.ToString();
    }

    // -------------------------------
    // ATTENDANCE MANAGEMENT
    // -------------------------------

    private async void OnPresentClicked(object? sender, EventArgs e)
    {
        presentCount++;

        UpdateAttendance();

        await DisplayAlertAsync(
            "Attendance",
            "Marked as Present!",
            "OK");
    }

    private async void OnAbsentClicked(object? sender, EventArgs e)
    {
        absentCount++;

        UpdateAttendance();

        await DisplayAlertAsync(
            "Attendance",
            "Marked as Absent!",
            "OK");
    }

    private void UpdateAttendance()
    {
        int total = presentCount + absentCount;

        double percentage = total > 0
            ? (double)presentCount / total * 100
            : 0;

        AttendanceLabel.Text =
            $"Present: {presentCount} | Absent: {absentCount}";

        AttendancePercentageLabel.Text =
            $"Attendance: {percentage:F1}%";

        AttendanceProgress.Progress = percentage / 100.0;
    }

    // -------------------------------
    // NOTES MANAGEMENT
    // -------------------------------

    private async void OnAddNoteClicked(object? sender, EventArgs e)
    {
        string noteText = NoteEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(noteText))
        {
            await DisplayAlertAsync(
                "Invalid Note",
                "Please enter some text.",
                "OK");

            return;
        }

        Notes.Add(new NoteItem(noteText));

        NoteEntry.Text = string.Empty;
    }

    private void OnDeleteNoteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not NoteItem note)
        {
            return;
        }

        Notes.Remove(note);
    }
}

// =================================
// TASK MODEL
// =================================

public class TaskItem : INotifyPropertyChanged
{
    private bool isCompleted;

    public string Title { get; }

    public bool IsCompleted
    {
        get => isCompleted;

        set
        {
            if (isCompleted == value)
                return;

            isCompleted = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
        }
    }

    public string StatusText =>
        IsCompleted ? "Completed" : "Pending";

    public TaskItem(string title, bool completed = false)
    {
        Title = title;
        isCompleted = completed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

// =================================
// NOTE MODEL
// =================================

public class NoteItem
{
    public string Text { get; }

    public NoteItem(string text)
    {
        Text = text;
    }
}
