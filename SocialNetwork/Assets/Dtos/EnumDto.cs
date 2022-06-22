namespace SocialNetwork.Assets.Dtos
{
    public class EnumDto
    {
        public object Value { get; set; }
        public string Description { get; set; }

        public EnumDto(object value, string description)
        {
            Value = value;
            Description = description;
        }
    }
}
