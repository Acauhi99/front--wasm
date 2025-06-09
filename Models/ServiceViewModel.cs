namespace front__wasm.Models
{
  // Modelo para cards e listagens (catálogo e destaques)
  public class ServiceCardViewModel
  {
    public string Id { get; set; } = string.Empty; // Identificador (gold, raids, etc)
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string DetailsUrl => $"/{Id}";
    public string GlowColor { get; set; } = string.Empty;
  }

  // Modelo para página de detalhes do serviço
  public class ServiceDetailsViewModel
  {
    public string Id { get; set; } = string.Empty;
    public Service Service { get; set; } = new();
    public string GradientClasses { get; set; } = string.Empty;
    public string IconColor { get; set; } = string.Empty;
    public string HoverBorderColor { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
  }
}