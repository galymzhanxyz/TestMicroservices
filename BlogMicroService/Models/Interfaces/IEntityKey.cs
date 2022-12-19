namespace BlogMicroService.Models.Interfaces
{
    public interface IEntityKey<T>
    {
        T Id { get; set; }
    }

    public interface IEntityDtoKey<T>
    {
        T Id { get; set; }
    }
}
