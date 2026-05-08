using Microsoft.AspNetCore.Mvc;
using SomaShare.Components.Model;

namespace SomaShare.Components.Functions
{
    public class UserLogic : Controller
    {
        private SomaContext context;

        public UserLogic (SomaContext context)
        {
            this.context = context;
        }

     
    }
}
