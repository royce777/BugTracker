namespace BugTracker.Models
{
    public class Chart
    {
        public string type { get; set; }
        public bool responsive { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public string[] labels { get; set; }
            public List<Datasets> datasets { get; set; }

            public class Datasets
            {
                public string label { get; set; }
                public int[] data { get; set; }
                public string[] backgroundColor { get; set; }

            }
        }

    }
}
