using System;

namespace BookSamples.Components.Filters
{
    public class DefaultViewSelector : IViewSelector
    {
        public String GetViewName(String viewName, String browserName)
        {
            return String.Format("{0}_{1}", viewName, browserName);
        }

        public String GetMasterName(String masterName, String browserName)
        {
            return String.Format("{0}_{1}", masterName, browserName);
        }
    }
}
