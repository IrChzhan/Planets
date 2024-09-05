using System.Drawing.Drawing2D;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Numerics;
using System.Media;
using System.Timers;
using System.Reflection.Metadata;

namespace Planets
{
    public partial class Form1 : Form
    {
        private List<Star> stars = new List<Star>();
        private List<Planet> planets = new List<Planet>();
        private bool isDraggingStar = false;
        private Star selectedStar;
        private PlanetDialog planetDialog;
        private Point mouseClickPoint;
        private System.Timers.Timer animationTimer;
        public double R(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.BackColor = Color.Black;
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += new MouseEventHandler(PictureBox1_MouseDown);
            pictureBox1.MouseUp += new MouseEventHandler(PictureBox1_MouseUp);
            pictureBox1.MouseMove += new MouseEventHandler(PictureBox1_MouseMove);
            pictureBox1.MouseClick += new MouseEventHandler(PictureBox1_MouseClick);
            button1.Click += new EventHandler(button1_Click);
            planetDialog = new PlanetDialog();
            Icon newIcon = new Icon("C:\\Users\\yasya\\source\\repos\\Planets\\котик.ico");
            this.Icon = newIcon;
            // Инициализация таймера для анимации
            animationTimer = new System.Timers.Timer();
            animationTimer.Interval = 20; // Интервал обновления в миллисекундах
            animationTimer.Elapsed += AnimationTimer_Elapsed;
            animationTimer.Start();
        }

        private void AnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Planet planet in planets)
            {
                planet.UpdatePosition();
            }
            pictureBox1.Invalidate();
        }
        // Обработчик события Paint
        public void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (Star star in stars)
            {
                e.Graphics.FillEllipse(new SolidBrush(star.Color), star.X - star.Radius, star.Y - star.Radius, 2 * star.Radius, 2 * star.Radius);
            }
            foreach (Planet planet in planets)
            {
                // Рисуем орбиту
                e.Graphics.DrawEllipse(new Pen(Color.LightGray, 1), planet.Star.X - planet.OrbitRadius, planet.Star.Y - planet.OrbitRadius, 2 * planet.OrbitRadius, 2 * planet.OrbitRadius);
                // Рисуем планету
                e.Graphics.FillEllipse(new SolidBrush(planet.Color), planet.X - planet.Radius, planet.Y - planet.Radius, 2 * planet.Radius, 2 * planet.Radius);
                // Рисуем атмосферу
                if (planet.AtmosphereThickness > 0)
                {
                    e.Graphics.DrawEllipse(new Pen(planet.AtmosphereColor, planet.AtmosphereThickness), planet.X - planet.Radius - planet.AtmosphereThickness, planet.Y - planet.Radius - planet.AtmosphereThickness, 2 * (planet.Radius + planet.AtmosphereThickness), 2 * (planet.Radius + planet.AtmosphereThickness));
                }
                // Рисуем наклон оси вращения
                if (planet.AxisTilt > 0)
                {

                    double angle = planet.AxisTilt * Math.PI / 180; // Преобразуем угол в радианы
                    int x1 = planet.X - (int)(planet.Radius * Math.Cos(angle));
                    int y1 = planet.Y - (int)(planet.Radius * Math.Sin(angle));
                    int x2 = planet.X + (int)(planet.Radius * Math.Cos(angle));
                    int y2 = planet.Y + (int)(planet.Radius * Math.Sin(angle));
                    e.Graphics.DrawLine(new Pen(Color.White, 1), x1, y1, x2, y2);
                }
            }
        }
        private Color GetColorFromText(string colorText)
        {
            switch (colorText.ToLower())
            {
                case "white":
                    return Color.White;
                case "blue":
                    return Color.Blue;
                case "yellow":
                    return Color.Yellow;
                case "red":
                    return Color.Red;
                default:
                    return Color.Empty;
            }
        }

        // Обработчик события MouseDown
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedStar == null)
                {
                    foreach (Star star in stars)
                    {
                        if (Math.Pow(e.X - star.X, 2) + Math.Pow(e.Y - star.Y, 2) <= Math.Pow(star.Radius, 2))
                        {
                            selectedStar = star;
                            isDraggingStar = true;
                            break;
                        }
                    }
                    mouseClickPoint = new Point(e.X, e.Y);
                }
                else
                {
                    // Сохраняем точку клика для создания планеты 
                    mouseClickPoint = new Point(e.X, e.Y);
                    Planet newPlanet = new Planet(selectedStar, mouseClickPoint.X, mouseClickPoint.Y)
                    {
                        Radius = planetDialog.Radius,
                        AtmosphereThickness = planetDialog.AtmosphereThickness,
                        AtmosphereColor = planetDialog.AtmosphereColor,
                        AxisTilt = planetDialog.AxisTilt,
                        Color = planetDialog.SurfaceColor
                    };
                    bool intersects = false;
                    foreach (Planet planet in planets)
                    {
                        if (planet != newPlanet && planet.Star != newPlanet.Star && (R(newPlanet.Star.X, newPlanet.Star.Y, planet.Star.X, planet.Star.Y) <= planet.OrbitRadius + planet.Radius + planet.AtmosphereThickness + // Между планетой и своей звездой
                        newPlanet.OrbitRadius + newPlanet.Radius + newPlanet.AtmosphereThickness)) // Растояние между newPlanet и ее звездой
                        {
                            intersects = true;
                            break;
                        }
                        if (planet != newPlanet && planet.Star == newPlanet.Star && (Math.Abs(planet.OrbitRadius - newPlanet.OrbitRadius) <= newPlanet.Radius + newPlanet.AtmosphereThickness + planet.Radius + planet.AtmosphereThickness)) // Радиусы планет и атмосфер
                        {
                            intersects = true;
                            break;
                        }
                    }
                    if (newPlanet.OrbitRadius <= newPlanet.Radius + newPlanet.AtmosphereThickness + newPlanet.Star.Radius)
                        intersects = true;

                    if (intersects)
                        MessageBox.Show("Планета не может находиться в этом месте.");
                    else
                    {
                        // Добавляем планету
                        planets.Add(newPlanet);
                        pictureBox1.Invalidate();
                        StartAnimation();
                        var create_p = new SoundPlayer("C:\\Users\\yasya\\source\\repos\\Planets\\create.wav");
                        create_p.Play();
                    }
                    intersects = false;
                }
                var create = new SoundPlayer("C:\\Users\\yasya\\source\\repos\\Planets\\create.wav");
                create.Play();
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Удаление звезды
                foreach (Star star in stars)
                {
                    if (Math.Pow(e.X - star.X, 2) + Math.Pow(e.Y - star.Y, 2) <= Math.Pow(star.Radius, 2))
                    {
                        stars.Remove(star);
                        planets.RemoveAll(p => p.Star == star);
                        pictureBox1.Invalidate();
                        var delete = new SoundPlayer("C:\\Users\\yasya\\source\\repos\\Planets\\delete.wav");
                        delete.Play();
                        break;
                    }
                }
                foreach (Planet planet in planets)
                {
                    // Проверяем, находится ли курсор на орбите планеты
                    if (Math.Pow(e.X - planet.Star.X, 2) + Math.Pow(e.Y - planet.Star.Y, 2) <= Math.Pow(planet.OrbitRadius, 2))
                    {
                        // Удаляем планету
                        planets.Remove(planet);
                        pictureBox1.Invalidate();
                        var delete = new SoundPlayer("C:\\Users\\yasya\\source\\repos\\Planets\\delete.wav");
                        delete.Play();
                        break;
                    }
                }
            }
        }
        // Обработчик события MouseUp
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDraggingStar = false;
                selectedStar = null;
            }
        }
        // Обработчик события MouseMove
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingStar)
            {
                foreach (Star star in stars)
                    if (Math.Pow(e.X - star.X, 2) + Math.Pow(e.Y - star.Y, 2) <= Math.Pow(star.Radius, 2))
                        selectedStar = star;
                // Перемещение звезды и ее планет
                int deltaX = e.X - selectedStar.X;
                int deltaY = e.Y - selectedStar.Y;
                selectedStar.X = e.X;
                selectedStar.Y = e.Y;
                foreach (Planet planet in planets)
                {
                    if (planet.Star == selectedStar)
                    {
                        planet.X += deltaX;
                        planet.Y += deltaY;
                    }
                }
                pictureBox1.Invalidate();
            }
        }
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Получаем цвет и радиус из текстбоксов
                if (int.TryParse(textBox1.Text, out int radius))
                {
                    Color color = GetColorFromText(textBox2.Text);
                    if (color != Color.Empty)
                    {
                        // Проверяем, пересекается ли новая звезда с существующими объектами
                        bool intersects = false;
                        foreach (Star star in stars)
                        {
                            if (R(e.X, e.Y, star.X, star.Y) <= radius + star.Radius)
                            {
                                intersects = true;
                                break;
                            }
                        }
                        foreach (Planet planet in planets)
                        {
                            if (R(e.X, e.Y, planet.Star.X, planet.Star.Y) <= planet.OrbitRadius + radius)
                            {
                                intersects = true;
                                break;
                            }
                        }
                        if (!intersects)
                        {
                            stars.Add(new Star(e.X, e.Y, radius, color));
                            selectedStar = stars[stars.Count - 1];
                            pictureBox1.Invalidate();
                            textBox1.Text = "";
                            textBox2.Text = "";
                        }
                        else
                            MessageBox.Show("Звезда не может быть создана в этом месте!");
                    }
                    else
                        MessageBox.Show("Введите корректное значение цвета (white, blue, yellow, red).");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (stars.Count > 0)
                selectedStar = stars[stars.Count - 1];
            // Проверяем, выбрана ли звезда
            if (selectedStar != null)
            {
                Point mousePosition = pictureBox1.PointToClient(Cursor.Position);
                planetDialog.X = mouseClickPoint.X;
                planetDialog.Y = mouseClickPoint.Y;

                //// Покажем диалоговое окно
                if (planetDialog.ShowDialog() == DialogResult.OK)
                {
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите звезду!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void StartAnimation()
        {
            animationTimer.Start();
        }
        private void StopAnimation()
        {
            animationTimer.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Создание звезды: введите параметры звезды в пустые окна и кликните по экрану левой кнопкой мыши.\n\n" +
                "Удаление объекта: нажмите на объект правой кнопкой мыши.\n\n" +
                "Создание планеты: введите в диалоговое окно радиус планеты, радиус атмосферы, цвет планеты, угол вращения и цвет атмосферы и нажмите на экран левой кнопкой мыши",
                "Инструкции",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}