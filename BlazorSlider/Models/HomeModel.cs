namespace BlazorSlider.Models
{
    public class HomeModel
    {
        public HomeDataContract Data { get; set; } = new();

        public class HomeDataContract
        {
            public Eq5dWalk Eq5dq1 { get; set; }

            public int Eq5dq6 { get; set; }

            public int Eq5dq6V2 { get; set; }

            public enum Eq5dWalk
            {
                NoProblems = 0,
                SomeProblems = 1,
                MediumProblems = 2,
                LargeProblems = 3,
                Incapable = 4
            }
        }
    }
}
