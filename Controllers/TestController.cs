using Microsoft.AspNetCore.Mvc;
using RepairSystem.API.Data;
using System.Threading.Tasks;

namespace RepairSystem.API.Controllers
{
    /// <summary>
    /// 測試控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IDapperContext _dapperContext;
        private readonly IRepairRepository _repository;

        /// <summary>
        /// 構造函數
        /// </summary>
        public TestController(IDapperContext dapperContext, IRepairRepository repository)
        {
            _dapperContext = dapperContext;
            _repository = repository;
        }

        /// <summary>
        /// 測試連接
        /// </summary>
        /// <returns>連接測試結果</returns>
        [HttpGet("connection")]
        public IActionResult TestConnection()
        {
            try
            {
                using var connection = _dapperContext.CreateConnection();
                connection.Open();
                return Ok(new { success = true, message = "成功連接到數據庫" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = $"無法連接到數據庫: {ex.Message}" });
            }
        }

        /// <summary>
        /// 測試服務
        /// </summary>
        /// <returns>服務測試結果</returns>
        [HttpGet("service")]
        public async Task<IActionResult> TestService()
        {
            try
            {
                var services = new
                {
                    dapperContext = _dapperContext != null,
                    repository = _repository != null
                };
                
                return Ok(new { success = true, message = "服務注入成功", services });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { success = false, message = $"服務測試失敗: {ex.Message}" });
            }
        }
    }
} 