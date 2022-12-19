using BlogMicroService.DALS.Repositories;
using BlogMicroService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostRepository _postsRepository;
        public PostsController(PostRepository postRepository)
        {
            _postsRepository = postRepository;
        }

        // GET: api/[controller]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> Get()
        {
            return await _postsRepository.GetAll();
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> Get(Guid id)
        {
            var post = await _postsRepository.Get(id);
            if (post == null)
            {
                return NotFound();
            }
            return post;
        }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, PostDto postDto)
        {
            if (id != postDto.Id)
            {
                return BadRequest();
            }
            await _postsRepository.Update(postDto.Map());
            return NoContent();
        }

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult<Post>> Post(PostDto postDto)
        {
            await _postsRepository.Add(postDto.Map());
            return CreatedAtAction("Get", new { id = postDto.Id }, postDto);
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> Delete(Guid id)
        {
            var post = await _postsRepository.Delete(id);
            if (post == null)
            {
                return NotFound();
            }
            return post;
        }
    }
}
