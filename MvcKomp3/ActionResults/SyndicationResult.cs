using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace BookSamples.Components.ActionResults
{
    public class SyndicationResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }
        public FeedType Type { get; set; }

        public SyndicationResult()
        {
            Type = FeedType.Rss;
        }
        public SyndicationResult(string title, string description, Uri uri, IEnumerable<SyndicationItem> items)
        {
            Type = FeedType.Rss;
            Feed = new SyndicationFeed(title, description, uri, items);
        }
        public SyndicationResult(SyndicationFeed feed)
        {
            Type = FeedType.Rss;
            Feed = feed;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = GetContentType();

            var feedFormatter = GetFeedFormatter();
            var writer = XmlWriter.Create(context.HttpContext.Response.Output);
            feedFormatter.WriteTo(writer);
            writer.Close();
        }

        private string GetContentType()
        {
            if (Type == FeedType.Atom)
                return "application/atom+xml";

            return "application/rss+xml";
        }

        private SyndicationFeedFormatter GetFeedFormatter()
        {
            if (Type == FeedType.Atom)
                return new Atom10FeedFormatter(Feed);

            return new Rss20FeedFormatter(Feed);
        }
    }
}