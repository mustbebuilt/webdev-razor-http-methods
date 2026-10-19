using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Services;

namespace MyRazorApp.Pages;

public class GetDemoModel : PageModel
{
    private readonly JsonDataService _jsonDataService;

    public GetDemoModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
    }

    // Bind properties from the URL query string using SupportsGet = true
    [BindProperty(SupportsGet = true)]
    public string? Subject { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Level { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    public List<Course> FilteredCourses { get; set; } = [];
    public List<string> AvailableCategories { get; set; } = [];
    public List<string> AvailableLevels { get; set; } = [];

    public void OnGet()
    {
        var allCourses = _jsonDataService.GetCourses();

        AvailableCategories = allCourses.Select(c => c.Category).Distinct().Order().ToList();
        AvailableLevels = allCourses.Select(c => c.Level).Distinct().Order().ToList();

        var query = allCourses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(Subject))
        {
            query = query.Where(c => c.Category.Equals(Subject, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(Level))
        {
            query = query.Where(c => c.Level.Equals(Level, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            query = query.Where(c => c.Title.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ||
                                     c.Code.Contains(Keyword, StringComparison.OrdinalIgnoreCase));
        }

        FilteredCourses = query.ToList();
    }
}
