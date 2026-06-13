namespace BackEndKrooq.DTO
{
    public class DashboardDTO
    {
        public string NomeUsuario { get; set; } = string.Empty;

        public int ProjetosAtivos { get; set; }

        public int ProjetosConcluidos { get; set; }

        public int TarefasPendentes { get; set; }

        public int TarefasConcluidas { get; set; }

        public int MetasPendentes { get; set; }

        public int MetasConcluidas { get; set; }

        public int ProgressoMedioProjetos { get; set; }
    }
}