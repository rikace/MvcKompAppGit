using System;

namespace BookSamples.Components.Filters
{
    public interface IViewSelector
    {
        String GetViewName(String viewName, String browserName);
        String GetMasterName(String masterName, String browserName);
    }
}