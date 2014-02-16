namespace SampleApp.WorkerServices
{
    public class HomeServices : IHomeServices
    {
        public void GetIndexViewModel(dynamic viewBag)
        {
            viewBag.Message = "Welcome to ASP.NET MVC!";
        }
    }
}