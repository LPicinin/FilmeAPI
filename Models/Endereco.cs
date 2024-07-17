using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmesApi.Models
{
    public class Endereco
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Logradouro { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "O número deve ser maior que zero")]
        public int Numero { get; set; }
    }
}
