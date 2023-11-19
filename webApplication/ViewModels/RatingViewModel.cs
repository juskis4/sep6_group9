namespace webApplication.ViewModels
{
    public class RatingViewModel
    {
        public int MovieId { get; set; }
        public float DbValue { get; set; }
        public int Votes { get; set; }

        //For the API OMDB
        public string Source { get; set; }
        public string Value { get; set; }
    }
}