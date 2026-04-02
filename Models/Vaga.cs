namespace JobMiner.Models
{
 public enum StatusVaga
 {
 Nova,
 Pendente,
 Enviado,
 Excluida
 }

 public class Vaga
 {
 public int Id { get; set; }
 public string Site { get; set; } = string.Empty;
 public string Titulo { get; set; } = string.Empty;
 public string Descricao { get; set; } = string.Empty;
 public string Codigo { get; set; } = string.Empty;
 public string Url { get; set; } = string.Empty;
 public string Responsavel { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string Telefone { get; set; } = string.Empty;
 public StatusVaga Status { get; set; }
 public string Data { get; set; } = string.Empty;
 public int Match { get; set; }
 }
}