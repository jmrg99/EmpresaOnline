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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace PracticaEmpresaOnline
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Data Source=LAPTOP-812783H7;Initial Catalog=EmpresaOnline;Integrated Security=True
        //EmpresaOnlineConnectionString
        //EmpresaOnlineDataSet

        SqlConnection conn;


        public MainWindow()
        {
            InitializeComponent();

            string empresaOnlineConexion = ConfigurationManager.ConnectionStrings
            ["PracticaEmpresaOnline.Properties.Settings.EmpresaOnlineConnectionString"].ConnectionString;

            conn = new SqlConnection(empresaOnlineConexion);

            mostrarClientes();
        }

        private void mostrarClientes()
        {
            string sql = "SELECT * FROM TCliente";

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);

                DataTable clientesTab = new DataTable();
                da.Fill(clientesTab);

                listaClientes.DisplayMemberPath = "cNombre";
                listaClientes.SelectedValuePath = "nClienteID";

                listaClientes.ItemsSource = clientesTab.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } //Mostrar clientes

        private void listClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaClientes.SelectedValue != null)
            {
                mostrarPedidosCliente();
            }
        } //SelectionChanged

        private void mostrarPedidosCliente()
        {
            string sql = "SELECT * FROM TPedido P INNER JOIN TCliente C ON P.nClienteID = C.nClienteID WHERE C.nClienteID = @clienteId";

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                cmd.Parameters.AddWithValue("@clienteId", listaClientes.SelectedValue);

                DataTable pedidosTab = new DataTable();
                da.Fill(pedidosTab);

                listaPedidos.DisplayMemberPath = "dCompra";
                listaPedidos.SelectedValuePath = "nNumPedido";
                listaPedidos.ItemsSource = pedidosTab.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //Mostrar los pedidos del cliente

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (listaClientes.SelectedValue != null)
            {

                string sql = "DELETE FROM TCliente WHERE nClienteID = @id";

                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@id", listaClientes.SelectedValue);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                mostrarClientes();
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún cliente.");
            }
        } //Boton eliminar

        private int dameIdNuevoCliente()
        {
            string sql = "SELECT ISNULL(MAX(nClienteID) + 1, 1) FROM TCliente";
            object ret = null;

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);

                ret = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (ret.GetType() != typeof(DBNull))
            {
                return Convert.ToInt32(ret);
            }
            else
            {
                return 0;
            }
        } //Metodo Dame un nuevo ID cliente

        private void btnInsertar_Click(object sender, RoutedEventArgs e)
        {
            string sql = "INSERT INTO TCliente (nClienteID, cNombre, cApellidos, nTelefono, cEmail, cDni)" +
            " VALUES (@id, @nombre, @apellidos, @telefono, @email, @dni)";

            if (txtNombre.Text != "" && txtApellidos.Text != "" && txtTelefono.Text != "" && txtEmail.Text != "" && txtDNI.Text !="")
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    conn.Open();

                    cmd.Parameters.AddWithValue("@id", dameIdNuevoCliente().ToString());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@apellidos", txtApellidos.Text);
                    cmd.Parameters.AddWithValue("@dni", txtDNI.Text);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //Limpiar campos de inserción...
                txtNombre.Text = "";
                txtApellidos.Text = "";
                txtDNI.Text = "";
                txtTelefono.Text = "";
                txtEmail.Text = "";

                mostrarClientes();
            }
           
        } //Insertar cliente

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {

            if (listaClientes.SelectedIndex < 0)
            {
                MessageBox.Show("NO EXISTE NINGÚN EMPLEADO EN LA PLANTILLA");
                return;
            }

            if (listaClientes.SelectedItems.Count > 1)
            {
                MessageBox.Show("HAS SELECCIONADO DEMASIADOS EMPLEADOS");
                return;
            }

            if (listaClientes.SelectedValue != null)
            {
                Modificar mod = new Modificar(Convert.ToInt32(listaClientes.SelectedValue.ToString()));

                string sql = "SELECT * FROM TCliente WHERE nClienteID = @id";

                try
                {
                    SqlCommand cm = new SqlCommand(sql, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cm);
                    cm.Parameters.AddWithValue("@id", listaClientes.SelectedValue);
                    DataTable cselec = new DataTable();
                    da.Fill(cselec);

                    if (cselec.Rows.Count == 1)
                    {
                        mod.txtNombre.Text = cselec.Rows[0]["cNombre"].ToString();
                        mod.txtApellidos.Text = cselec.Rows[0]["cApellidos"].ToString();
                        mod.txtDNI.Text = cselec.Rows[0]["cDNI"].ToString();
                        mod.txtTelefono.Text = cselec.Rows[0]["nTelefono"].ToString();
                        mod.txtEmail.Text = cselec.Rows[0]["cEmail"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo acceder a la información del cliente seleccionado");
                    }
                }
                catch (Exception p)
                {
                    MessageBox.Show(p.Message);
                }

                mod.ShowDialog();

                mostrarClientes();
            }
        }
    }
}
