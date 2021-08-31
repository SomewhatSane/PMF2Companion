namespace PMF2Companion.Objects
{
    public class WatchlistResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Offence { get; set; }
        public int OffenceNo { get; set; }
        public int BanDuration { get; set; }
        public string Comments { get; set; }
        public string AddedBy { get; set; }
        public string DateTime { get; set; }
    }
}
