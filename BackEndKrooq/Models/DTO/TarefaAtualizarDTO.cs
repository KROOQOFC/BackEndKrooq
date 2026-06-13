namespace BackEndKrooq.DTO
{
    public class TarefaAtualizarDTO
    {
        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public int Progresso { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }
    }
}