using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculadoraCientifica
{
    // Clases de operaciones
    public class OperacionesBasicas
    {
        public double Sumar(double a, double b) => a + b;
        public double Restar(double a, double b) => a - b;
        public double Multiplicar(double a, double b) => a * b;
        public double Dividir(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("No se puede dividir por cero.");
            return a / b;
        }

        // Método para la raíz cuadrada
        public double RaizCuadrada(double a)
        {
            if (a < 0)
                throw new ArgumentException("No se puede calcular la raíz cuadrada de un número negativo.");
            return Math.Sqrt(a);
        }

        // Método para el porcentaje
        public double Porcentaje(double a, double b) => (a * b) / 100;
    }
}
