using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	[Route("api/values")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		// GET api/values
		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			if (id > 1)
				return Ok(id);
			return BadRequest("error msg");
		}

		[HttpPost("json")]
		public IActionResult Json([FromBody]TestData data)
		{
			if (data.Name == "error")
				return BadRequest("bad request");
			return Ok(data);
		}

		[HttpPost("form"), Consumes("application/x-www-form-urlencoded")]
		public IActionResult Form()
		{
			return Ok(Request.Form);
		}

		[HttpPost("upload"), Consumes("multipart/form-data")]
		public IActionResult Upload()
		{
			if (!Request.Form.Files.Any())
				return BadRequest("no file");
			return Ok(new
			{
				files = Request.Form.Files.Select(p => p.Length).ToList(),
				form = Request.Form
			});
		}
	}
}
