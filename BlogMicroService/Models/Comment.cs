using BlogMicroService.Models.Interfaces;

namespace BlogMicroService.Models
{
    public class Comment : IEntityKey<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.Empty;
        public string CommentBody { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }

    public class CommentDto : IEntityDtoKey<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = Guid.Empty;
        public string CommentBody { get; set; }
        public Guid PostId { get; set; }

        public Comment Map()
        {
            return new Comment
            {
                Id = this.Id,
                UserId = this.UserId,
                CommentBody = this.CommentBody,
                PostId = this.PostId
            };
        }
    }
}
