using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class NotaDebitoRequestModel
    {
        public List<NotaDebitoModel> Notas { get; set; }
        public LoginApiData Login { get; set; }
    }
}
