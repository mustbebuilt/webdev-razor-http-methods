using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Services;

namespace MyRazorApp.Pages;

public class MultiRouteDemoModel : PageModel
{
    private readonly JsonDataService _jsonDataService;

    public MultiRouteDemoModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
    }

    // Bound from route path segment 1: @page "{department?}/{id:int?}"
    [BindProperty(SupportsGet = true)]
    public string? Department { get; set; }

    // Bound from route path segment 2: @page "{department?}/{id:int?}"
    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    // Bound from query string ?tab=...
    [BindProperty(SupportsGet = true)]
    public string? Tab { get; set; } = "overview";

    public List<DepartmentGroup> AvailableDepartments { get; set; } = [];
    public StaffMember? SelectedMember { get; set; }
    public List<StaffMember> DepartmentStaff { get; set; } = [];

    public void OnGet(string? department, int? id)
    {
        var allStaff = _jsonDataService.GetStaff();

        AvailableDepartments = allStaff
            .Select(s => s.Department.ToLower())
            .Distinct()
            .Select(dept => new DepartmentGroup(
                dept,
                $"{char.ToUpper(dept[0]) + dept[1..]} ({allStaff.Count(s => s.Department.Equals(dept, StringComparison.OrdinalIgnoreCase))} staff)"
            ))
            .ToList();

        if (string.IsNullOrWhiteSpace(Department) && AvailableDepartments.Count > 0)
        {
            Department = AvailableDepartments.First().Slug;
        }

        DepartmentStaff = allStaff
            .Where(s => s.Department.Equals(Department, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (id.HasValue)
        {
            SelectedMember = DepartmentStaff.FirstOrDefault(s => s.Id == id.Value)
                           ?? allStaff.FirstOrDefault(s => s.Id == id.Value);
        }
        else if (DepartmentStaff.Count > 0)
        {
            SelectedMember = DepartmentStaff.First();
            Id = SelectedMember.Id;
        }
    }
}

public record DepartmentGroup(string Slug, string DisplayName);
