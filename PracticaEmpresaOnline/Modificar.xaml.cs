using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PracticaEmpresaOnline
{
    /// <summary>
    /// Lógica de interacción para Modificar.xaml
    /// </summary>
    public partial class Modificar : Window
    {
        private int id;
        private SqlConnection conn;

        public Modificar(int id)
        {
            this.id = id;

            InitializeComponent();

            string empresaOnlineConexion = ConfigurationManager.ConnectionStrings["PracticaEmpresaOnline.Properties.Settings.EmpresaOnlineConnectionString"].ConnectionString;

            conn = new SqlConnection(empresaOnlineConexion);
        }

        private int Id { get => id; set => id = value; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sql = "UPDATE TCliente SET cNombre = @nombre, cApellidos = @apellidos, cDNI = @dni, nTelefono = @telefono, cEmail = @email WHERE nClienteID = " + id.ToString();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();

                cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                cmd.Parameters.AddWithValue("@apellidos", txtApellidos.Text);
                cmd.Parameters.AddWithValue("@dni", txtDNI.Text);
                cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }

            this.Close();
        }
    }
}
