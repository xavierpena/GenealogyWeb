using GenealogyWeb.Core.Business.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Sandbox
{
    public class DataTreeTests
    {
        private TreeBuilder _treeBuilder;
        public DataTreeTests(TreeBuilder treeBuilder)
        {
            _treeBuilder = treeBuilder;
        }

        public void Process()
        {
            var test = _treeBuilder.GetResult();
        }
    }
}
