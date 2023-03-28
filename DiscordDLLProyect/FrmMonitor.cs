using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordDLLProyect
{
    public partial class FrmMonitor : Form
    {
        public FrmMonitor()
        {
            InitializeComponent();
        }

        Task runBot;

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            //iniciamos el task que enciende al bot
            runBot.Start();
            MessageBox.Show("El bot esta corriendo", "Ojo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //Desactivamos el boton para encender al bot
            BtnActivate.Enabled = false;
        }

        private void FrmMonitor_Load(object sender, EventArgs e)
        {
            //Hacemos una instancia de la clase bot
            ClsBot bot = new ClsBot();
            //Asignamos el metodo para imprimir en el richTextbox al delegado estatico
            ClsComandos.Print = Print;
            //Iniciamos una nueva task con el metodo para encender el bot
            runBot = new Task(() =>
            {
                bot.RunAsync().GetAwaiter().GetResult();
            });
        }

        public void Print(string text)
        {
            Invoke(new Action(() => { richTextBox1.Text += text + Environment.NewLine; }));
        }

        private void salirDeLaAplicacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BtnActivate.Enabled == false)
            {
                if (MessageBox.Show("¿Estas seguro que deseas salir?\nEl bot dejara de funcionar si sales",
                    "Atencion", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("Gracias, vuelve pronto", "Farewell!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
        }
    }
}
