using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class TestResult
    {
        public UserInfo UserInfo { get; set; }
        public IList<ExperimentLog> ClickHistoryList { get; set; }
        public MappingEnum Mapping { get; set; }
        public TestResult(MappingEnum mapping)
        {
            this.Mapping = mapping;
        }
    }
}
