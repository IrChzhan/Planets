using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planets
{
    public class PlanetDialog : Form
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        public int AtmosphereThickness { get; set; }
        public Color AtmosphereColor { get; set; }
        public Color SurfaceColor { get; set; }
        public int AxisTilt { get; set; }

        private TextBox textBoxRadius;
        private NumericUpDown numericUpDownAtmosphereThickness;
        private Button buttonAtmosphereColor;
        private Button buttonSurfaceColor;
        private NumericUpDown numericUpDownAxisTilt;

        public PlanetDialog()
        {
            // Задаем размер и заголовок диалогового окна
            this.Size = new Size(500, 350);
            this.Text = "Введите данные о планете";

            // Создаем элементы управления
            textBoxRadius = new TextBox();
            numericUpDownAtmosphereThickness = new NumericUpDown();
            buttonAtmosphereColor = new Button();
            buttonSurfaceColor = new Button();
            numericUpDownAxisTilt = new NumericUpDown();

            // Устанавливаем свойства элементов управления
            textBoxRadius.Location = new Point(10, 10);
            textBoxRadius.Size = new Size(100, 30);
            textBoxRadius.Text = ""; // Значение по умолчанию

            numericUpDownAtmosphereThickness.Location = new Point(10, 70);
            numericUpDownAtmosphereThickness.Size = new Size(100, 20);
            numericUpDownAtmosphereThickness.Minimum = 1;
            numericUpDownAtmosphereThickness.Maximum = 50;
            numericUpDownAtmosphereThickness.Value = 5; // Значение по умолчанию

            buttonAtmosphereColor.Location = new Point(10, 170);
            buttonAtmosphereColor.Size = new Size(100, 20);
            buttonAtmosphereColor.Text = "Цвет атмосферы";
            buttonAtmosphereColor.Click += ButtonAtmosphereColor_Click;

            buttonSurfaceColor.Location = new Point(10, 100);
            buttonSurfaceColor.Size = new Size(100, 20);
            buttonSurfaceColor.Text = "Цвет поверхности";
            buttonSurfaceColor.Click += ButtonSurfaceColor_Click;

            numericUpDownAxisTilt.Location = new Point(10, 130);
            numericUpDownAxisTilt.Size = new Size(100, 20);
            numericUpDownAxisTilt.Minimum = 0;
            numericUpDownAxisTilt.Maximum = 90;
            numericUpDownAxisTilt.Value = 23; // Значение по умолчанию

            // Добавляем элементы управления в диалоговое окно
            this.Controls.Add(textBoxRadius);
            this.Controls.Add(numericUpDownAtmosphereThickness);
            this.Controls.Add(buttonAtmosphereColor);
            this.Controls.Add(buttonSurfaceColor);
            this.Controls.Add(numericUpDownAxisTilt);

            // Создаем кнопки "ОК" и "Отмена"
            Button buttonOK = new Button();
            buttonOK.Location = new Point(50, 200);
            buttonOK.Size = new Size(75, 40);
            buttonOK.Text = "ОК";
            buttonOK.Click += ButtonOK_Click;
            this.Controls.Add(buttonOK);

            Button buttonCancel = new Button();
            buttonCancel.Location = new Point(150, 200);
            buttonCancel.Size = new Size(110, 40);
            buttonCancel.Text = "Отмена";
            buttonCancel.Click += ButtonCancel_Click;
            this.Controls.Add(buttonCancel);
        }

        private void ButtonAtmosphereColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                AtmosphereColor = colorDialog.Color;
                buttonAtmosphereColor.BackColor = AtmosphereColor;
            }
        }

        private void ButtonSurfaceColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                SurfaceColor = colorDialog.Color;
                buttonSurfaceColor.BackColor = SurfaceColor;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            int Radius;
            int AtmosphereThickness;
            int AxisTilt;

            // Получаем данные из элементов управления
            if (int.TryParse(textBoxRadius.Text, out Radius) &&
                int.TryParse(numericUpDownAtmosphereThickness.Value.ToString(), out AtmosphereThickness) &&
                int.TryParse(numericUpDownAxisTilt.Value.ToString(), out AxisTilt))
            {
                this.Radius = Radius;
                this.AtmosphereThickness = AtmosphereThickness;
                this.AxisTilt = AxisTilt;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите корректные значения.");
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void InitializeComponent()
        {
            SuspendLayout(); 
            ClientSize = new Size(284, 261);
            Name = "PlanetDialog";
            ResumeLayout(false);
        }
    }
}
