using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business.Tree
{
    public class MarriageNode
    {
        public PersonNode spouse { get; set; }
        public List<PersonNode> children { get; set; }
    }

    public class Extra
    {
    }

    public class PersonNode
    {
        private PersonNode personNode;

        public PersonNode()
        {
            // empty contstructor
        }

        public PersonNode(PersonNode personNode)
        {
            this.name = personNode.name + " (duplicate!)";
            this.@class = personNode.@class;
        }

        // The name of the node:
        public string name { get; set; }

        // The CSS class of the node:
        public string @class { get; set; }

        // The CSS class of the text in the node:
        public string textClass { get; set; }

        // Generational height offset:
        public int? depthOffset { get; set; }

        // Marriages is a list of nodes:
        public List<MarriageNode> marriages { get; set; }

        // Custom data passed to the renderers:
        public Extra extra { get; set; }

        public void AddMarriage(MarriageNode marriageNode)
        {
            if (this.marriages == null)
                this.marriages = new List<MarriageNode>();

            this.marriages.Add(marriageNode);
        }
    }
}
