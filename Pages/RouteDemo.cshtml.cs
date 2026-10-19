using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Services;

namespace MyRazorApp.Pages;

public class RouteDemoModel : PageModel
{
    private readonly JsonDataService _jsonDataService;

    public RouteDemoModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
    }

    // Bound from the URL route path defined in @page "{id:int?}"
    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    // Bound from the URL query string e.g. ?tab=salary&view=detailed
    [BindProperty(SupportsGet = true)]
    public string? Tab { get; set; } = "overview";

    [BindProperty(SupportsGet = true)]
    public string? ViewMode { get; set; } = "standard";

    public StaffMember? SelectedMember { get; set; }
    public List<StaffMember> SampleStaff { get; set; } = [];

    public void OnGet(int? id)
    {
        SampleStaff = _jsonDataService.GetStaff();

        if (id.HasValue)
        {
            SelectedMember = SampleStaff.FirstOrDefault(s => s.Id == id.Value);
        }
        else if (SampleStaff.Count > 0)
        {
            SelectedMember = SampleStaff.First();
        }
    }
}
