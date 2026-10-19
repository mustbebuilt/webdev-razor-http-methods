using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using MyRazorApp.Services;

namespace MyRazorApp.Pages;

public class PostDemoModel : PageModel
{
    private readonly JsonDataService _jsonDataService;

    public PostDemoModel(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
    }

    [BindProperty]
    public FeedbackInput Input { get; set; } = new();

    // TempData flag to support Post-Redirect-Get (PRG) pattern demonstration
    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? SubmittedStudentName { get; set; }

    [TempData]
    public string? SubmittedCategory { get; set; }

    [TempData]
    public string? SubmittedFeedback { get; set; }

    public List<FeedbackItem> RecentFeedback { get; set; } = [];

    public void OnGet()
    {
        // OnGet loads persisted submissions from Data/feedback.json
        RecentFeedback = _jsonDataService.GetFeedback();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            RecentFeedback = _jsonDataService.GetFeedback();
            return Page(); // Return page with validation errors if invalid
        }

        // Persist submitted feedback to the JSON data file
        _jsonDataService.AddFeedback(Input.StudentName, Input.Email, Input.Category, Input.Comments);

        // Store feedback details in TempData across redirect (Post-Redirect-Get pattern)
        FlashMessage = "Form successfully submitted via HTTP POST & saved to Data/feedback.json!";
        SubmittedStudentName = Input.StudentName;
        SubmittedCategory = Input.Category;
        SubmittedFeedback = Input.Comments;

        // Post-Redirect-Get (PRG): Redirect back to GET page to prevent double-submit on browser refresh
        return RedirectToPage("/PostDemo");
    }
}

public class FeedbackInput
{
    [Required(ErrorMessage = "Please enter your student name.")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a feedback category.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your comments.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Comments must be between 10 and 500 characters.")]
    public string Comments { get; set; } = string.Empty;
}
