using BlogMicroService.Models;

namespace BlogMicroService.DALS.Repositories
{
    public class PostRepository : EfCoreRepository<Post, ApplicationContext>
    {
        public PostRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}
