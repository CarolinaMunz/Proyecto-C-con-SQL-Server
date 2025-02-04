using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

//Tiene que estar conectado a una base de datos de un servidor local de SQL Server 
//Ademas de tener instalado los paquetes NuGet EntityFrameWork y Microsoft.EntityFrameworkCore.Tools

namespace EjercicioGrupo16
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Producto
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }
        }

        public class SupermercadoContext : DbContext
        {
            public SupermercadoContext() : base("name=EjercicioGrupo16.Properties.Settings.SupermercadoConnectionString") //Pon name de connectionStrings de App.config
            {
            }
            public DbSet<Producto> Productos { get; set; }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text) || !decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Por favor, ingresa datos válidos para el producto.");
                return;
            }

            var producto = new Producto
            {
                Descripcion = txtDescripcion.Text,
                Precio = precio
            };

            try
            {
                using (var context = new SupermercadoContext())
                {
                    context.Productos.Add(producto);
                    context.SaveChanges();
                }

                MessageBox.Show("Producto guardado exitosamente.");

                txtDescripcion.Clear();
                txtPrecio.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                int productoId = (int)dgvProductos.SelectedRows[0].Cells["Id"].Value;

                DialogResult confirmacion = MessageBox.Show(
                    "¿Estás seguro de que deseas eliminar este producto?",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmacion == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new SupermercadoContext())
                        {
                            var producto = context.Productos.FirstOrDefault(p => p.Id == productoId);
                            if (producto != null)
                            {
                                context.Productos.Remove(producto);
                                context.SaveChanges();
                                MessageBox.Show("Producto eliminado con éxito.");
                            }
                            else
                            {
                                MessageBox.Show("No se encontró el producto para eliminar.");
                            }
                        }

                        CargarProductos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CargarProductos()
        {
            try
            {
                using (var context = new SupermercadoContext())
                {
                    var productos = context.Productos.ToList();
                    dgvProductos.DataSource = productos;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
