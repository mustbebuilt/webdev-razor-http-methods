namespace MyRazorApp.Models;

public record StaffMember(
    int Id,
    string Name,
    string Role,
    string Department,
    string Email,
    string Salary,
    string EmploymentType,
    string HiredDate
);
