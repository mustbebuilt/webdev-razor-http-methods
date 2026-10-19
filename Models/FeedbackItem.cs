namespace MyRazorApp.Models;

public record FeedbackItem(
    int Id,
    string StudentName,
    string Email,
    string Category,
    string Comments,
    DateTime SubmittedAt
);
