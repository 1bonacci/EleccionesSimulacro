using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELECCIONES
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        clsElecciones clsElecciones = new clsElecciones();

        private void button1_Click(object sender, EventArgs e)
        {
            clsElecciones.Boton(listView1, chart1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clsElecciones.CargarList(listView1);
        }
    }
}
