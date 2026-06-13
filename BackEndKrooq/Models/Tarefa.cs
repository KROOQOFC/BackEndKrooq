namespace BackEndKrooq.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int Progresso { get; set; }

        public string Status { get; set; } = "Pendente";

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int ProjetoId { get; set; }

        public Projeto Projeto { get; set; } = null!;
    }
}