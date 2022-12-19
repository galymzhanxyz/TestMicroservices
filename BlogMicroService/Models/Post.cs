using BlogMicroService.Models.Interfaces;

namespace BlogMicroService.Models
{
    public class Post : IEntityKey<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PostHeader { get; set; }
        public string PostBody { get; set; }

        public IList<Comment> Comments { get; set; }
    }


    public class PostDto : IEntityDtoKey<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PostHeader { get; set; }
        public string PostBody { get; set; }
        public IList<Comment> Comments { get; set; }

        public Post Map()
        {
            return new Post
            {
                Id = this.Id,
                PostHeader = this.PostHeader,
                PostBody = this.PostBody,
                Comments = this.Comments
            };
        }
    }
}
