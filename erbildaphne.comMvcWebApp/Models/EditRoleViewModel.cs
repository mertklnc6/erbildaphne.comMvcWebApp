using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erbildaphne.comMvcWebApp.Models
{
    public class EditRoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string[] UserIdsToAdd { get; set; } = new string[0];
        public string[] UserIdsToDelete { get; set; } = new string[0];
    }
}
