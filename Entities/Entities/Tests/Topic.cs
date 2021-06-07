using AppCore.Records.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Entities.Tests
{
    public class Topic: Record
    {
        public string Name { get; set; }
        public List<QuestionTest> QuestionTests { get; set; }
        public List<SpaceTest> SpaceTests { get; set; }
    }
}
