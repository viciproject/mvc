using System;
using Vici.Core.Parser;

namespace Vici.Mvc
{
    internal class ViciCompiledTemplate : ICompiledTemplate
    {
        public ViciCompiledTemplate(CompiledTemplate compiledTemplate)
        {
            CompiledTemplate = compiledTemplate;
        }

        public string FileName
        {
            get { return CompiledTemplate.FileName; }
            set { CompiledTemplate.FileName = value; }
        }

        public readonly CompiledTemplate CompiledTemplate;
    }
}