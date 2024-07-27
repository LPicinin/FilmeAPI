using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaController : ControllerBase
    {
        private DatabaseContext _context;
        private IMapper _mapper;

        public CinemaController(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Adiciona([FromBody] CreateCinemaDto dto)
        {
            var entity = _mapper.Map<Cinema>(dto);
            _context.Cinemas.Add(entity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaPorId), new { id = entity.Id }, entity);
        }

        [HttpGet]
        public IEnumerable<ReadCinemaDto> Recupera([FromQuery] int? enderecoId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (enderecoId == null)
                return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.Skip(skip).Take(take).ToList());
            else
                return _mapper.Map<List<ReadCinemaDto>>(
                    _context.Cinemas.FromSqlRaw($"SELECT \"Id\", \"Nome\", \"EnderecoId\" " +
                    $"  FROM \"Cinemas\" WHERE \"EnderecoId\" = {enderecoId}").Skip(skip).Take(take).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult RecuperaPorId(int id)
        {
            var entity = _context.Cinemas
                .FirstOrDefault(entity => entity.Id == id);
            if (entity == null)
                return NotFound();
            var dto = _mapper.Map<ReadCinemaDto>(entity);
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public IActionResult Atualiza(int id, [FromBody] UpdateCinemaDto dto)
        {
            var entity = _context.Cinemas.FirstOrDefault(entity => entity.Id == id);
            if (entity == null)
                return NotFound();

            _mapper.Map(dto, entity);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult AtualizaParcial(int id, [FromBody] JsonPatchDocument<UpdateCinemaDto> path)
        {
            var entity = _context.Cinemas.FirstOrDefault(entity => entity.Id == id);
            if (entity == null)
                return NotFound();

            var entityParaAtualizar = _mapper.Map<UpdateCinemaDto>(entity);
            path.ApplyTo(entityParaAtualizar, ModelState);

            if (!TryValidateModel(entityParaAtualizar))
                return ValidationProblem(ModelState);

            _mapper.Map(entityParaAtualizar, entity);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Deleta(int id)
        {
            var entity = _context.Cinemas.FirstOrDefault(entity => entity.Id == id);
            if (entity == null)
                return NotFound();

            _context.Remove(entity);

            _context.SaveChanges();
            return NoContent();
        }
    }
}
