using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHookHub.Models
{
    public class Text
    {
        public string name { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string innerHTML { get; set; }
    }

    public class Child
    {
        public Text text { get; set; }
        public string image { get; set; }
        public bool? pseudo { get; set; }

        public List<Child> children { get; set; }
        public string HTMLclass { get; set; }
        public bool? stackChildren { get; set; }
        public int? childrenDropLevel { get; set; }
        public bool? collapsable { get; set; }
        public bool? collapsed { get; set; }
    }

    public class NodeStructure
    {
        public Text text { get; set; }
        public string HTMLclass { get; set; }
        public List<Child> children { get; set; }
    }

}
