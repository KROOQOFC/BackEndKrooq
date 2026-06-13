namespace BackEndKrooq.Models.DTO
{
    public class DashboardDTO
    {
        public string NomeUsuario { get; set; } = string.Empty;

        public int TotalProjetos { get; set; }

        public int ProjetosAndamento { get; set; }

        public int ProjetosConcluidos { get; set; }

        public int TotalTarefas { get; set; }

        public int TarefasConcluidas { get; set; }

        public int ProgressoGeral { get; set; }

        public List<MetaDTO> Metas { get; set; } = [];
    }
}
