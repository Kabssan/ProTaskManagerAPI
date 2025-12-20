namespace ProTaskManagerAPI.Models;
using System.ComponentModel.DataAnnotations;
public class TodoTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Der Titel ist ein Pflichtfeld.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Der Titel muss zwischen 3 und 100 Zeichen lang sein.")]
    public string Title { get; set; } = string.Empty;
    [MaxLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}