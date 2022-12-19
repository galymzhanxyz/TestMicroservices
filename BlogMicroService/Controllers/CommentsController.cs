using BlogMicroService.DALS.Repositories;
using BlogMicroService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.EventBus.Core.Models;
using RabbitMQ.EventBus.Producer;

namespace BlogMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentRepository _commentRepository;

        public CommentsController(CommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // GET: api/[controller]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> Get()
        {
            return await _commentRepository.GetAll();
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> Get(Guid id)
        {
            var comment = await _commentRepository.Get(id);
            if (comment == null)
            {
                return NotFound();
            }
            return comment;
        }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CommentDto commentDto)
        {
            if (id != commentDto.Id)
            {
                return BadRequest();
            }
            await _commentRepository.Update(commentDto.Map());
            return NoContent();
        }

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult<Comment>> Post(CommentDto commentDto)
        {
            await _commentRepository.Add(commentDto.Map());
            return CreatedAtAction("Get", new { id = commentDto.Id }, commentDto);
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> Delete(Guid id)
        {
            var comment = await _commentRepository.Delete(id);
            if (comment == null)
            {
                return NotFound();
            }
            return comment;
        }
    }
}
