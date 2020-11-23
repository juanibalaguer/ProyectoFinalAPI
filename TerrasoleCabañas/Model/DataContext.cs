using Microsoft.EntityFrameworkCore;

namespace TerrasoleCabañas.Model
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<Estadia> Estadias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoLinea> PedidoLineas { get; set; }
        public DbSet<Producto_Servicio> Productos_Servicios { get; set; }
        public DbSet<TerrasoleCabañas.Model.Cabania> Cabanias { get; set; }

    }
}
