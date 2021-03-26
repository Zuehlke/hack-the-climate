namespace HackTheClimate.Data
{
    public class Topic
    {
        public string DocumentId { get; set; }
        public TopicWord[] Words { get; set; }
    }

    public class TopicWord
    {
        public string Word { get; set; }
        public double Score { get; set; }
    }
}
