namespace Foundry.Domain.ApiModel
{
    public class UserFavoriteModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrgnisationId { get; set; }
        public bool IsFavorite { get; set; }
        public string SessionId { get; set; }
    }
}
