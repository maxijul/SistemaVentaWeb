using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<RolMenu> RolMenus { get; } = new List<RolMenu>();

    public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
}
