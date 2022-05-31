using System.Drawing;

namespace _132134412312
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            var label = new Label();
            label.Location = new Point(0, 0);
            label.Size = new Size(100, 20);
            label.Text = "я лох";
            Controls.Add(label);
        }
    }
}