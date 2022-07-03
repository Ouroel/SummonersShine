using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace SummonersShine.DataStructures
{
    public class Parabola
    {
        float vertexX;
        float vertexY;
        float coefficient;

        Parabola(float vertexX, float vertexY, float coefficient)
        {
            this.vertexX = vertexX;
            this.vertexY = vertexY;
            this.coefficient = coefficient;
        }
        public static Parabola CreateParabola(Vector2 first, Vector2 second, float boingHeight) {
            float vertexY = Math.Min(first.Y, second.Y) - boingHeight;

            float yDist1 = MathF.Sqrt(first.Y - vertexY);
            float yDist2 = MathF.Sqrt(second.Y - vertexY);

            float ratio = MathF.Sqrt((first.Y - vertexY) / (second.Y - vertexY));
            float vertexX = (second.X * ratio + first.X) / (ratio + 1);
            float coefficient = (vertexY - second.Y) / ((second.X - vertexX) * (second.X - vertexX));

            return new Parabola(vertexX, vertexY, coefficient);

        }

        public float CalculateY(float X) {
            return -coefficient * MathF.Pow((X - vertexX), 2) + vertexY;
        }
    }
}
