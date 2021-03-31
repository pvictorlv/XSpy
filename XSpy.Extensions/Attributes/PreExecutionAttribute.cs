using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace XSpy.Extensions.Attributes
{
    public class PreExecutionAttribute : ActionFilterAttribute
    {
    }
}
