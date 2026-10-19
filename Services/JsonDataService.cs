using System.Text.Json;
using MyRazorApp.Models;

namespace MyRazorApp.Services;

public class JsonDataService
{
    private readonly string _staffFilePath;
    private readonly string _coursesFilePath;
    private readonly string _feedbackFilePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
    private readonly object _feedbackLock = new();

    public JsonDataService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        _staffFilePath = Path.Combine(dataDir, "staff.json");
        _coursesFilePath = Path.Combine(dataDir, "courses.json");
        _feedbackFilePath = Path.Combine(dataDir, "feedback.json");
    }

    public List<StaffMember> GetStaff()
    {
        if (!File.Exists(_staffFilePath)) return [];
        var json = File.ReadAllText(_staffFilePath);
        return JsonSerializer.Deserialize<List<StaffMember>>(json, _jsonOptions) ?? [];
    }

    public List<Course> GetCourses()
    {
        if (!File.Exists(_coursesFilePath)) return [];
        var json = File.ReadAllText(_coursesFilePath);
        return JsonSerializer.Deserialize<List<Course>>(json, _jsonOptions) ?? [];
    }

    public List<FeedbackItem> GetFeedback()
    {
        lock (_feedbackLock)
        {
            if (!File.Exists(_feedbackFilePath)) return [];
            var json = File.ReadAllText(_feedbackFilePath);
            return JsonSerializer.Deserialize<List<FeedbackItem>>(json, _jsonOptions) ?? [];
        }
    }

    public void AddFeedback(string studentName, string email, string category, string comments)
    {
        lock (_feedbackLock)
        {
            var list = GetFeedback();
            int nextId = list.Count > 0 ? list.Max(f => f.Id) + 1 : 1;
            var item = new FeedbackItem(nextId, studentName, email, category, comments, DateTime.Now);
            list.Insert(0, item);
            var json = JsonSerializer.Serialize(list, _jsonOptions);
            File.WriteAllText(_feedbackFilePath, json);
        }
    }
}
