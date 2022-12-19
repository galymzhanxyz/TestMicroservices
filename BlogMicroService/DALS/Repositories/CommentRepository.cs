using BlogMicroService.Models;

namespace BlogMicroService.DALS.Repositories
{
    public class CommentRepository : EfCoreRepository<Comment, ApplicationContext>
    {
        public CommentRepository(ApplicationContext context)
            : base(context)
        {
        }
    }
}
