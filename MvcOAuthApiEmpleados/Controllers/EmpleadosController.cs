using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.Filters;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;
        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int idempleado)
        {
             Empleado empleado = await this.service.FindEmpleadoAsync(idempleado);
             return View(empleado);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> PerfilEmpleado()
        {
            //NECESITAMOS BUSCAR EL EMPLEADO CON SU CLAIM Y
            //NAME IDENTIFIER
            var data = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            int idEmpleado = int.Parse(data);
            Empleado empleado = await this.service.FindEmpleadoAsync(idEmpleado);
            return View(empleado);
        }
    }
}
