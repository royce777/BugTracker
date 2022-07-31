namespace BugTracker.Models
{
    public class Chart
    {
        public string type { get; set; }
        public bool responsive { get; set; }
        public Data data { get; set; }
        public Options options { get; set; }


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

        public class Options 
        {
            public Plugins plugins { get; set; }
            public class Plugins 
            {
                public Title title { get; set; }
                public Legend legend { get; set; }
                public class Title
                {
                    public bool display { get; set; }
                    public string text { get; set; }
                }
                public class Legend
                {
                    public bool display { get; set; }
                }
            }
        }

    }
}
