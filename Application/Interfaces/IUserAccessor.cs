using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserAccessor
    {
        string GetCurrentUsername();        //Since we use token in request header so no parameter is necessary
    }
}
