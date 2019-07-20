using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstBot.Helpers
{
    public class AccessToken
    {
        public string token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }

    }
}
