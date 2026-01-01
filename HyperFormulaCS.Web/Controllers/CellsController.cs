using Microsoft.AspNetCore.Mvc;
using HyperFormulaCS.Calculation;
using HyperFormulaCS.Models;
using System.Collections.Generic;

namespace HyperFormulaCS.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CellsController : ControllerBase
    {
        private readonly Engine _engine;

        public CellsController(Engine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        public IActionResult GetCells()
        {
            var cells = _engine.GetAllCells();
            return Ok(cells);
        }

        // DTO
        public class SetCellRequest
        {
            public string Address { get; set; }
            public string Formula { get; set; }
        }

        [HttpPost]
        public IActionResult SetCell([FromBody] SetCellRequest req)
        {
            if (string.IsNullOrEmpty(req.Address)) return BadRequest("Address required");

            _engine.SetCell(req.Address, req.Formula ?? "");
            var val = _engine.GetCellValue(req.Address);

            // Return the updated cell value (and ideally all impacted cells)
            // For now, just return the value of this cell.
            return Ok(new
            {
                address = req.Address,
                value = val.ToString(),
                type = val.GetType().Name
            });
        }
    }
}
