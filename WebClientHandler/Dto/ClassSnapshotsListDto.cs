using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientHandler.Dto
{
    public class ClassSnapshotsListDto
    {
        public CriteriaDto ClassSubjectId { get; set; }

        public List<string> FullTermsData { get; set; }
    }
}
