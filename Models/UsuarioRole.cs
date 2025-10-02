using System;
using System.Collections.Generic;

namespace USUARIOS.Models;

public partial class UsuarioRole
{
    public int UsuarioRolId { get; set; }

    public int UsuarioId { get; set; }

    public int RolId { get; set; }

    public virtual Role Rol { get; set; } = null!;

    public virtual Usuario? Usuario { get; set; }
}
