using System;
using System.Web.Mvc;

namespace customApiApp_3
{
    public interface IHttpVerbAttribute
    {
        HttpVerbs HttpVerb { get; }   
    }

    public class AllowGet : Attribute, IHttpVerbAttribute
    {
        public HttpVerbs HttpVerb { get; private set; }
        public AllowGet()
        {
            HttpVerb = HttpVerbs.Get;
        }
    }
    public class AllowPost : Attribute, IHttpVerbAttribute
    {
        public HttpVerbs HttpVerb { get; private set; }
        public AllowPost()
        {
            HttpVerb = HttpVerbs.Post;
        }
    }
    public class AllowDelete : Attribute, IHttpVerbAttribute
    {
        public HttpVerbs HttpVerb { get; private set; }
        public AllowDelete()
        {
            HttpVerb = HttpVerbs.Delete;
        }
    }
}
