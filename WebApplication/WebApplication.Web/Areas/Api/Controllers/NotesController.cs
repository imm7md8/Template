using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Domain.Models;
using WebApplication.Infrastructure.Repos.Interfaces;

namespace WebApplication.App.Web.Areas.Api.Controllers
{
	[Area("Api")]
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class NotesController : ControllerBase
	{
		private readonly IRepository<Note> _repo;

		public NotesController(IRepository<Note> repo)
		{
			_repo = repo;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var notes = await _repo.GetAllAsync();
			return Ok(notes);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Note note)
		{
			await _repo.AddAsync(note);
			return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var note = await _repo.GetByIdAsync(id);
			return note is null ? NotFound() : Ok(note);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await _repo.DeleteAsync(id);
			return NoContent();
		}
	}
}
