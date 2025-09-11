namespace pinklet.Dto
{
    public class RatingDto
    {
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int Rate { get; set; }
        public string? Review { get; set; }
    }
}
