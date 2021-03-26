using System;
using System.Collections.Generic;
using System.Text;

namespace HackTheClimate.Tools.Data
{
    public class LegislationEntry
    {
        public string Id { get; set; }
        public string Law_Id { get; set; }
        public string Title { get; set; }
        public string Legislation_type { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }
        public string Geography { get; set; }
        public string Geography_iso { get; set; }
        public string Sector { get; set; }
        public string Frameworks { get; set; }
        public string Responses { get; set; }
        public string Document_types { get; set; }
        public string Keywords { get; set; }
        public string Natural_hazards { get; set; }
        public string Visibility_status { get; set; }

    }
}
