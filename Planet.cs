using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planets
{
    public class Planet
    {
        public Star Star { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        public Color Color { get; set; }
        public int OrbitRadius { get; set; }
        public int AtmosphereThickness { get; set; } // Толщина атмосферы
        public Color AtmosphereColor { get; set; } // Цвет атмосферы
        public int AxisTilt { get; set; } // Наклон оси вращения
        public double Angle { get; set; }

        public Planet(Star star, int x, int y)
        {
            Star = star;
            X = x;
            Y = y;
            OrbitRadius = (int)Math.Sqrt(Math.Pow(x - star.X, 2) + Math.Pow(y - star.Y, 2));
            Angle = 0;
        }

        // Метод для обновления координат планеты
        public void UpdatePosition()
        {
            Angle += 0.01; // Увеличиваем угол вращения
            if (Angle >= 2 * Math.PI) // Если угол превысил 360 градусов, сбрасываем его
                Angle = 0;
            // Вычисляем новые координаты планеты на основе угла вращения
            X = Star.X + (int)(OrbitRadius * Math.Cos(Angle));
            Y = Star.Y + (int)(OrbitRadius * Math.Sin(Angle));
        }
    }
}
