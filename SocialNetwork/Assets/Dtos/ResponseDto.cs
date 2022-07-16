namespace SocialNetwork.Assets.Dtos
{
    public class ResponseDto
    {
        public bool Result { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
    }

    public class DetailedResponse
    {
        public string Detail { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }
}
