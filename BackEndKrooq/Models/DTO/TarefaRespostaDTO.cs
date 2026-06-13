namespace BackEndKrooq.DTO
{
    public class TarefaRespostaDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int Progresso { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int ProjetoId { get; set; }
    }
}