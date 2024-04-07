﻿namespace Domain.Models.Users
{
    public class UsuarioPermissao
    {
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int PermissaoId { get; set; }
        public Permissao Permissao { get; set; }
    }

}
