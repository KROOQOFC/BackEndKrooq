//using Microsoft.AspNetCore.Mvc;

//namespace BackEndKrooq.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class DashboardController : ControllerBase
//    {
//        private readonly DashboardService _dashboardService;

//        public DashboardController(DashboardService dashboardService)
//        {
//            _dashboardService = dashboardService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> ObterDashboard()
//        {
//            var dados = await _dashboardService.ObterDashboardAsync();

//            return Ok(dados);
//        }
//    }
//}
