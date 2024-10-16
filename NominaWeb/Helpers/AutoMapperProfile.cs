﻿using AutoMapper;
using NominaWeb.Dto.Empleado;
using NominaWeb.Dto.Nomina;
using NominaWeb.Models;

namespace NominaWeb.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeo de Empleado a EmpleadoDTO y viceversa
            CreateMap<Empleado, EmpleadoDTO>().ReverseMap();

            // Mapeo de CreateEmpleadoDTO a Empleado (para crear nuevos empleados)
            CreateMap<CreateEmpleadoDTO, Empleado>();

            // Mapeo de UpdateEmpleadoDTO a Empleado (para actualizar empleados existentes)
            CreateMap<UpdateEmpleadoDTO, Empleado>();

            CreateMap<NominaCreateDto, Nominas>();
            CreateMap<Nominas, NominaDto>();

            CreateMap<NominaEmpleado, EmpleadoNominaDto>().ReverseMap();
        }
    }
}
