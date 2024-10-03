using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculadoraCientifica
{
    public class OperacionesAvanzadas
    {
        public double Potencia(double x, double y)
        {
            return Math.Pow(x, y);
        }

        public double Cubo(double x)
        {
            return Math.Pow(x, 3);
        }

        public double Cuadrado(double x)
        {
            return Math.Pow(x, 2);
        }

        public double Pi()
        {
            return Math.PI;
        }

        public double RaizCubica(double x)
        {
            return Math.Pow(x, 1.0 / 3.0);
        }

        public double RaizN(double x, double n)
        {
            return Math.Pow(x, 1.0 / n);
        }

        public double DiezElevadoA(double x)
        {
            return Math.Pow(10, x);
        }

        public double Exp(double x)
        {
            return Math.Exp(x);
        }

        public double Log(double x)
        {
            return Math.Log10(x);
        }
    }

    public class Exponenciacion
    {
        public double Potencia(double baseNumero, double exponente) => Math.Pow(baseNumero, exponente);
    }
}
