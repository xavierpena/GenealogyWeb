using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business
{
    public class JsonItem
    {
        public int nodeDepth { get; set; } = 0;
        public string name { get; set; }
        public List<JsonItem> children { get; set; }

        public JsonItem(string name)
        {
            this.name = name;
            this.children = null;
        }

        /// <summary>
        /// This constructor is only used when a node already exists.
        /// The goal is to avoid circular dependency when transforming the object to json string.
        /// </summary>
        public JsonItem(JsonItem item)
        {
            this.name = Utils.GetDuplicateStr(item.name);
        }

        public void AddChild(JsonItem jsonItem)
        {
            if (this.children == null)
                this.children = new List<JsonItem>();

            this.children.Add(jsonItem);
        }

        public void AddChildren(params JsonItem[] jsonItems)
        {
            foreach (var jsonItem in jsonItems)
                this.AddChild(jsonItem);
        }

        public override string ToString()
        {
            if (this.children == null)
            {
                return $"{{ \"name\": \"{this.name}\"}}";
            }
            else
            {
                var str = $"{{ \"name\": \"{this.name}\", ";

                var childrenStrList = new List<string>();
                foreach (var child in this.children)
                    childrenStrList.Add(child.ToString());

                str += "\"children\": [" + string.Join(",", childrenStrList) + "] }";

                return str;
            }
        }
    }
}
