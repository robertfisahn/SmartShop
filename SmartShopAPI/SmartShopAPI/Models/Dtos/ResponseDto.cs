namespace SmartShopAPI.Models.Dtos
{
    public class ResponseDto
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
    }
}
