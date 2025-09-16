using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgendaPro.Models;


[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class EventosController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public EventosController(ApplicationDbContext db) { _db = db; }

    [HttpGet]
    public IActionResult Get() => Ok(_db.Set<Evento>().ToList()); // supondo model Evento

    [HttpPost]
    public IActionResult Post(Evento e)
    {
        _db.Add(e);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = e.Id }, e);
    }
}
