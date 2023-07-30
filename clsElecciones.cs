using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Windows.Forms.DataVisualization.Charting;

namespace ELECCIONES
{
    internal class clsElecciones
    {
        private OleDbConnection cnx = new OleDbConnection();
        private OleDbCommand cmd = new OleDbCommand();
        private OleDbDataAdapter adap = new OleDbDataAdapter();

        private string CadenaConexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ELECCIONES.mdb";
        private string vSQL = "";


        public void CargarList(ListView List)
        {
            cnx.ConnectionString = CadenaConexion;
            cnx.Open();

            string query = "SELECT * FROM Departamentos";

            OleDbCommand command = new OleDbCommand(query, cnx);

            OleDbDataReader reader = command.ExecuteReader();
            List.Items.Clear();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader["nombre"].ToString());
                item.SubItems.Add(reader["departamento"].ToString());
                // Agrega más subítems según las columnas de tu tabla
                List.Items.Add(item);
            }

            reader.Close();
            cnx.Close();
        }

        public void Boton(ListView list, Chart chart)
        {
            // Obtén los departamentos seleccionados en el ListView
            List<string> departamentosSeleccionados = new List<string>();
            foreach (ListViewItem item in list.CheckedItems)
            {
                departamentosSeleccionados.Add(item.Text);
            }

            // Abre la base de datos Access
            cnx.ConnectionString = CadenaConexion;
            cnx.Open();

            // Crea una consulta SQL para obtener la cantidad total de votos por lista en los departamentos seleccionados
            string query = "SELECT L.gobernador, SUM(V.votos) AS TotalVotos " +
                   "FROM (Votos V INNER JOIN Listas L ON V.lista = L.lista) " +
                   "INNER JOIN Departamentos D ON V.departamento = D.departamento " +
                   "WHERE D.nombre IN (";
            for (int i = 0; i < departamentosSeleccionados.Count; i++)
            {
                query += "'" + departamentosSeleccionados[i] + "'";
                if (i < departamentosSeleccionados.Count - 1)
                {
                    query += ",";
                }
            }
            query += ") GROUP BY L.gobernador";

            // Crea un objeto OleDbCommand y OleDbDataReader para ejecutar la consulta
            OleDbCommand command = new OleDbCommand(query, cnx);
            OleDbDataReader reader = command.ExecuteReader();

            // Limpia el Chart antes de agregar nuevos datos
            chart.Series.Clear();

            // Crea una nueva serie en el Chart para representar los resultados
            Series serieResultados = new Series("Resultados");
            serieResultados.ChartType = SeriesChartType.Bar; // Cambia el tipo de gráfico a barra

            // Agrega los puntos de datos al Chart
            while (reader.Read())
            {
                string gobernador = reader["gobernador"].ToString();
                int totalVotos = Convert.ToInt32(reader["TotalVotos"]);
                serieResultados.Points.AddXY(gobernador, totalVotos);
            }

            // Agrega la serie al Chart
            chart.Series.Add(serieResultados);

            // Cierra el OleDbDataReader y la conexión a la base de datos
            reader.Close();
            cnx.Close();

            // Configura el título del gráfico
            chart.Titles.Clear();
            chart.Titles.Add("Elecciones a Gobernador 2023");

            // Configura las leyendas de los ejes "X" y "Y"
            chart.ChartAreas[0].AxisX.Title = "Gobernadores";
            chart.ChartAreas[0].AxisY.Title = "Votos";
        }
    }
}
