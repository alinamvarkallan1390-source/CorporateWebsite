using System.Collections.Generic;

namespace CorporateWebsite.Application.Interfaces;

public class SubmitFormDto
{
    public int FormId { get; set; }
    public Dictionary<string, string> FieldValues { get; set; } = new Dictionary<string, string>();
    public string? RecaptchaToken { get; set; }
}