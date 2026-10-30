using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WidgetDemoApp;

public partial class MainPage : ContentPage
{
    // Collections used by the UI
    public ObservableCollection<TaskItem> VisibleTasks { get; set; } = new();
    public ObservableCollection<string> Skills { get; set; } = new();
    public ObservableCollection<NoteItem> Notes { get; set; } = new();

    // Stores all tasks, including tasks hidden by search
    private readonly List<TaskItem> AllTasks = new();

    // Counters
    private int likeCount = 0;
    private int presentCount = 0;
    private int absentCount = 0;

    public MainPage()
    {
        InitializeComponent();

        BindingContext = this;

        // Header
        GreetingLabel.Text = "Hello, Akash!";
        DateLabel.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

        // Sample tasks
        AllTasks.Add(new TaskItem("Complete .NET MAUI assignment"));
        AllTasks.Add(new TaskItem("Practice Data Structures", true));
        AllTasks.Add(new TaskItem("Prepare for technical interview"));
        AllTasks.Add(new TaskItem("Revise Database Management"));

        ApplyTaskSearch("");

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

        UpdateTaskStatistics();
        UpdateAttendance();
    }

    // --------------------------------
    // PROFILE SECTION
    // --------------------------------

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
        string comment = CommentEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(comment))
        {
            CommentLabel.Text = "Please enter a comment.";
            return;
        }

        CommentLabel.Text = $"Your comment: {comment}";
        CommentEntry.Text = "";
    }

    private void OnSwitchToggled(object? sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            StatusLabel.Text = "Notifications are enabled.";
            StatusLabel.TextColor = Colors.Green;
        }
        else
        {
            StatusLabel.Text = "Notifications are disabled.";
            StatusLabel.TextColor = Colors.Red;
        }
    }

    // --------------------------------
    // TASK MANAGEMENT
    // --------------------------------

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        string title = TaskEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlertAsync(
                "Invalid Task",
                "Please enter a task name.",
                "OK");

            return;
        }

        AllTasks.Add(new TaskItem(title));

        TaskEntry.Text = "";

        ApplyTaskSearch(TaskSearchBar.Text ?? "");
        UpdateTaskStatistics();
    }

    private void OnTaskCheckedChanged(
        object? sender,
        CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox &&
            checkBox.BindingContext is TaskItem task)
        {
            task.IsCompleted = e.Value;

            UpdateTaskStatistics();
        }
    }

    private void OnDeleteTaskClicked(object? sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is TaskItem task)
        {
            AllTasks.Remove(task);

            ApplyTaskSearch(TaskSearchBar.Text ?? "");
            UpdateTaskStatistics();
        }
    }

    private void OnTaskSearchTextChanged(
        object? sender,
        TextChangedEventArgs e)
    {
        ApplyTaskSearch(e.NewTextValue ?? "");
    }

    private void ApplyTaskSearch(string searchText)
    {
        VisibleTasks.Clear();

        var filteredTasks = AllTasks.Where(task =>
            task.Title.Contains(
                searchText,
                StringComparison.OrdinalIgnoreCase));

        foreach (var task in filteredTasks)
        {
            VisibleTasks.Add(task);
        }
    }

    private void UpdateTaskStatistics()
    {
        int total = AllTasks.Count;

        int completed = AllTasks.Count(task =>
            task.IsCompleted);

        int pending = total - completed;

        TotalTasksLabel.Text = total.ToString();
        CompletedTasksLabel.Text = completed.ToString();
        PendingTasksLabel.Text = pending.ToString();
    }

    // --------------------------------
    // ATTENDANCE SECTION
    // --------------------------------

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

        double percentage = total == 0
            ? 0
            : (double)presentCount / total * 100;

        AttendanceLabel.Text =
            $"Present: {presentCount} | Absent: {absentCount}";

        AttendancePercentageLabel.Text =
            $"Attendance: {percentage:F1}%";

        AttendanceProgress.Progress =
            percentage / 100.0;
    }

    // --------------------------------
    // NOTES SECTION
    // --------------------------------

    private async void OnAddNoteClicked(object? sender, EventArgs e)
    {
        string noteText = NoteEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(noteText))
        {
            await DisplayAlertAsync(
                "Invalid Note",
                "Please enter some text.",
                "OK");

            return;
        }

        Notes.Add(new NoteItem(noteText));

        NoteEntry.Text = "";
    }

    private void OnDeleteNoteClicked(object? sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is NoteItem note)
        {
            Notes.Remove(note);
        }
    }
}

// --------------------------------
// TASK MODEL
// --------------------------------

public class TaskItem : INotifyPropertyChanged
{
    private bool isCompleted;

    public string Title { get; set; }

    public bool IsCompleted
    {
        get => isCompleted;

        set
        {
            if (isCompleted != value)
            {
                isCompleted = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public string StatusText =>
        IsCompleted ? "Completed" : "Pending";

    public TaskItem(string title, bool completed = false)
    {
        Title = title;
        IsCompleted = completed;
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

// --------------------------------
// NOTE MODEL
// --------------------------------

public class NoteItem
{
    public string Text { get; set; }

    public NoteItem(string text)
    {
        Text = text;
    }
}