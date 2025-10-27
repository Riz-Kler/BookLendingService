using Microsoft.AspNetCore.Mvc;
using BookLending.Api.Models;
using BookLending.Api.Repositories;

namespace BookLending.Api.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repo;
    public BooksController(IBookRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Book book)
    {
        await _repo.AddAsync(book);
        return CreatedAtAction(nameof(GetAll), new { id = book.Id }, book);
    }

    [HttpPost("{id:guid}/checkout")]
    public async Task<IActionResult> Checkout(Guid id)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book is null) return NotFound();
        if (!book.IsAvailable) return BadRequest("Book is already checked out.");
        book.IsAvailable = false;
        await _repo.UpdateAsync(book);
        return NoContent();
    }

    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book is null) return NotFound();
        book.IsAvailable = true;
        await _repo.UpdateAsync(book);
        return NoContent();
    }
}
