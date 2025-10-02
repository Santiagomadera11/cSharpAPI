using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace USUARIOS.Models;

public partial class UsuariosContext : DbContext
{
    public UsuariosContext()
    {
    }

    public UsuariosContext(DbContextOptions<UsuariosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioRole> UsuarioRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK_Roles");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Nombre, "UQ_Roles_NombreRol").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("rolId");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK_Usuarios");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.NroDoc, "UQ_Usuarios_NumeroDocumento").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NroDoc)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("nroDoc");
            entity.Property(e => e.TipoDoc)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("tipoDoc");
        });

        modelBuilder.Entity<UsuarioRole>(entity =>
        {
            entity.HasKey(e => e.UsuarioRolId).HasName("PK_UsuarioRoles");

            entity.ToTable("usuarioRoles");

            entity.HasIndex(e => new { e.UsuarioId, e.RolId }, "UQ_UsuarioRoles").IsUnique();

            entity.Property(e => e.UsuarioRolId).HasColumnName("usuarioRolId");
            entity.Property(e => e.RolId).HasColumnName("rolId");
            entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRoles_Roles");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRoles)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRoles_Usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
