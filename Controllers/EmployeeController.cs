using Microsoft.AspNetCore.Mvc;
using EmployeeServices;
using EmployeeModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Storage;

[ApiController]
[Route("api/[controller]")]

public class EmployeeController : ControllerBase
{
    private readonly EmployeeService _service;

    public EmployeeController(EmployeeService service)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("getValid")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllIsValid()
    {
        return Ok(await _service.GetAllIsValid());
    }

    [HttpGet("getByShainNo/{shainNo}")]
    public async Task<ActionResult<Employee>> GetByShainNo(string shainNo)
    {
        var empListByShainNo = await _service.GetByShainNo(shainNo);

        if (empListByShainNo is null)
        {
            return NotFound();
        }

        return Ok(empListByShainNo);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto body)
    {
        //400 BadRequestは入れるべき？→[ApiController]がついているので、DTOで付与した属性に応じてバリデーションエラーを自動で出してくれる
        var register = await _service.Register(body);

        if (register.Result is EmployeeResult.Conflict)
        {
            return Conflict("既に存在する社員番号です。");
        }

        return CreatedAtAction(
            nameof(GetByShainNo),
            new { shainNo = register.ShainNo },
            body);  //★画面からの登録時のレスポンスボディを確認
    }

    [HttpPut("update/{shainNo}")]
    public async Task<IActionResult> Update(string shainNo, [FromBody] UpdateEmployeeDto body)
    {
        var result = await _service.UpdateEmployee(shainNo, body);

        return result switch
        {
            EmployeeResult.Success => Ok(),
            EmployeeResult.NotFound => NotFound("社員が存在しません。"),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("logicalDelete/{shainNo}")]
    public async Task<IActionResult> LogicalDelete(string shainNo)
    {
        var result = await _service.LogicalDelete(shainNo);

        return result switch
        {
            EmployeeResult.Success => Ok(),
            EmployeeResult.NotFound => NotFound("社員が存在しません。"),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("physicalDelete/{shainNo}")]
    public async Task<IActionResult> PhysicalDelete(string shainNo)
    {
        var result = await _service.PhysicalDelete(shainNo);

        return result switch
        {
            EmployeeResult.Success => Ok(),
            EmployeeResult.NotFound => NotFound("社員が存在しません。"),
            _ => StatusCode(500)
        };
    }


//練習ーーーーーーーーーーーーーーーーーーーー


[HttpGet("search")]
public async Task<ActionResult<List<SearchResponceDto>?>> Search([FromQuery] string? name, [FromQuery] int? minAge, [FromQuery] int? maxAge, [FromQuery]  bool sortDesc)
    {
        var employees = await _service.Search(name, minAge, maxAge, sortDesc);

        if(employees.result == SearchResult.BadRequest)
        {
            return BadRequest();
        }

        return Ok(employees.employees);

    }








//練習ーーーーーーーーーーーーーーーーーーーー




}

