namespace my_api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Opcional: Você pode adicionar validações usando DataAnnotations
    // [Required]
    // [StringLength(100)]
    // public string Name { get; set; } = string.Empty;
}