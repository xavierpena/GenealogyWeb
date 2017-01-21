using GenealogyWeb.Core.Business.DownwardTree;
using GenealogyWeb.Core.Business.UpwardTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Sandbox
{
    public class DataTreeTests
    {
        private DownwardTreeBuilder _downwardTreeBuilder;
        private UpwardTreeBuilder _upwardTreeBuilder;
        public DataTreeTests(DownwardTreeBuilder downwardTreeBuilder,
            UpwardTreeBuilder upwardTreeBuilder)
        {
            _downwardTreeBuilder = downwardTreeBuilder;
            _upwardTreeBuilder = upwardTreeBuilder;
        }

        public void Process()
        {
            //var test = _downwardTreeBuilder.GetResult();
            var test = _upwardTreeBuilder.GetResult(1);
        }
    }
}
