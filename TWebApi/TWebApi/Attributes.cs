using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace customApiApp_3
{
    public interface IHttpVerbAttribute
    {
        HttpVerbs HttpVerb { get; }   
    }

    public class TypeAlias : Attribute
    {
        private readonly string _alias;

        public TypeAlias(string alias)
        {
            this._alias = alias;
        }

        public string Alias
        {
            get { return _alias; }
        }
    }

    public class TypeAliases : Attribute
    {
        private readonly IEnumerable<string> _aliases;

        public TypeAliases(params string[] aliases)
        {
            this._aliases = aliases;
        }

        public IEnumerable<string> Aliases
        {
            get { return _aliases; }
        }

        public bool Contains(string alias)
        {
            return Aliases.Any(x => string.Equals(x, alias, StringComparison.CurrentCultureIgnoreCase));
        }
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
