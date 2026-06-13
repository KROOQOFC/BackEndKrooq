using BackEndKrooq.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackEndKrooq.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(
            DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObterDashboard(
            int usuarioId)
        {
            var dashboard =
                await _dashboardService
                .ObterDashboard(usuarioId);

            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }
    }
}