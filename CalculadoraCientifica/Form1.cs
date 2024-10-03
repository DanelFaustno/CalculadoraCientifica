using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CalculadoraCientifica
{
    public partial class Form1 : Form
    {
        // Instancias de las clases de operaciones
        private OperacionesBasicas operacionesBasicas = new OperacionesBasicas();
        private OperacionesTrigonométricas operacionesTrigonométricas = new OperacionesTrigonométricas();
        private Exponenciacion exponenciacion = new Exponenciacion();
        private bool operadorPresionado = false;
        private OperacionesAvanzadas operacionesAvanzadas = new OperacionesAvanzadas();

        public Form1()
        {
            InitializeComponent();
            AdjuntarManejadoresDeEventos();
        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                string expresion = textBox1.Text;
                double resultado = CalcularExpresion(expresion);
                textBox2.Text = resultado.ToString();
            }
            catch (Exception ex)
            {
                textBox2.Text = "Error: " + ex.Message;
            }
        }

        private double CalcularExpresion(string expresion)
        {
            var operacionesBasicas = new OperacionesBasicas();
            var operacionesAvanzadas = new OperacionesAvanzadas();
            var numeros = new List<double>();
            var operadores = new List<string>();

            // Función para obtener el último número ingresado
            double GetLastNumber()
            {
                if (numeros.Count > 0)
                    return numeros[numeros.Count - 1];
                throw new ArgumentException("No hay un número válido para usar como 'x' o 'y'.");
            }

            for (int i = 0; i < expresion.Length; i++)
            {
                if (char.IsDigit(expresion[i]) || expresion[i] == '.')
                {
                    string numero = "";
                    while (i < expresion.Length && (char.IsDigit(expresion[i]) || expresion[i] == '.'))
                    {
                        numero += expresion[i];
                        i++;
                    }
                    i--; // Retroceder un paso para no saltar el siguiente carácter
                    numeros.Add(double.Parse(numero));
                }
                else if (expresion[i] == '+' || expresion[i] == '-' || expresion[i] == 'x' || expresion[i] == '÷' || expresion[i] == '%')
                {
                    operadores.Add(expresion[i].ToString());
                }
                else if (expresion[i] == '^')
                {
                    if (i < expresion.Length - 1)
                    {
                        if (expresion[i + 1] == '2')
                        {
                            operadores.Add("^2");
                            i++;
                        }
                        else if (expresion[i + 1] == '3')
                        {
                            operadores.Add("^3");
                            i++;
                        }
                        else if (expresion[i + 1] == 'y')
                        {
                            operadores.Add("^y");
                            i++;
                            numeros.Add(GetLastNumber()); // Añadir 'y' como el último número ingresado
                        }
                        else
                        {
                            operadores.Add("^");
                        }
                    }
                    else
                    {
                        operadores.Add("^");
                    }
                }
                else if (expresion[i] == '√')
                {
                    if (i > 0 && char.IsDigit(expresion[i - 1]))
                    {
                        operadores.Add("y√x");
                        numeros.Add(GetLastNumber()); // Añadir 'x' como el último número ingresado
                    }
                    else
                    {
                        operadores.Add("√");
                        if (numeros.Count == 0 || numeros.Count == operadores.Count - 1)
                        {
                            numeros.Add(2); // Raíz cuadrada por defecto
                        }
                    }
                }
                else if (expresion.Substring(i).StartsWith("π"))
                {
                    numeros.Add(operacionesAvanzadas.Pi());
                    i += "π".Length - 1;
                }
                else if (expresion.Substring(i).StartsWith("EXP"))
                {
                    operadores.Add("EXP");
                    i += "EXP".Length - 1;
                }
                else if (expresion.Substring(i).StartsWith("log"))
                {
                    operadores.Add("log");
                    i += "log".Length - 1;
                }
                else if (expresion.Substring(i).StartsWith("10^"))
                {
                    operadores.Add("10^");
                    i += "10^".Length - 1;
                }
                else if (!char.IsWhiteSpace(expresion[i]))
                {
                    throw new ArgumentException($"Token no válido: {expresion[i]}");
                }
            }

            while (operadores.Count > 0)
            {
                int index = 0;
                var priorityOps = new[] { "^", "^2", "^3", "^y", "√", "y√x", "log", "EXP", "10^" };
                var foundPriorityOp = operadores.FindIndex(op => priorityOps.Contains(op));
                if (foundPriorityOp != -1)
                    index = foundPriorityOp;
                else if (operadores.Contains("x") || operadores.Contains("÷"))
                    index = operadores.FindIndex(op => op == "x" || op == "÷");

                double resultado;
                switch (operadores[index])
                {
                    case "√":
                        resultado = operacionesBasicas.RaizCuadrada(numeros[index + 1]);
                        numeros[index] = operacionesBasicas.Multiplicar(numeros[index], resultado);
                        numeros.RemoveAt(index + 1);
                        break;
                    case "y√x":
                        resultado = operacionesAvanzadas.RaizN(numeros[index + 1], numeros[index]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "^":
                    case "^y":
                        resultado = operacionesAvanzadas.Potencia(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "^2":
                        resultado = operacionesAvanzadas.Cuadrado(numeros[index]);
                        numeros[index] = resultado;
                        break;
                    case "^3":
                        resultado = operacionesAvanzadas.Cubo(numeros[index]);
                        numeros[index] = resultado;
                        break;
                    case "10^":
                        resultado = operacionesAvanzadas.DiezElevadoA(numeros[index]);
                        numeros[index] = resultado;
                        break;
                    case "EXP":
                        resultado = operacionesAvanzadas.Exp(numeros[index]);
                        numeros[index] = resultado;
                        break;
                    case "log":
                        resultado = operacionesAvanzadas.Log(numeros[index]);
                        numeros[index] = resultado;
                        break;
                    case "x":
                        resultado = operacionesBasicas.Multiplicar(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "÷":
                        resultado = operacionesBasicas.Dividir(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "+":
                        resultado = operacionesBasicas.Sumar(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "-":
                        resultado = operacionesBasicas.Restar(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                    case "%":
                        resultado = operacionesBasicas.Porcentaje(numeros[index], numeros[index + 1]);
                        numeros[index] = resultado;
                        numeros.RemoveAt(index + 1);
                        break;
                }
                operadores.RemoveAt(index);
            }

            return numeros[0];
        }

        private void BotonOperacionAvanzada_Click(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            string operacion = boton.Text;
            string textoActual = textBox1.Text;

            switch (operacion)
            {
                case "x^y":
                    if (!string.IsNullOrEmpty(textoActual) && char.IsDigit(textoActual[textoActual.Length - 1]))
                        textBox1.Text += "^";
                    else
                        MessageBox.Show("Ingrese un número antes de usar x^y");
                    break;
                case "x^2":
                    if (!string.IsNullOrEmpty(textoActual) && char.IsDigit(textoActual[textoActual.Length - 1]))
                        textBox1.Text += "^2";
                    else
                        MessageBox.Show("Ingrese un número antes de usar x^2");
                    break;
                case "x^3":
                    if (!string.IsNullOrEmpty(textoActual) && char.IsDigit(textoActual[textoActual.Length - 1]))
                        textBox1.Text += "^3";
                    else
                        MessageBox.Show("Ingrese un número antes de usar x^3");
                    break;
                case "π":
                    textBox1.Text += "π";
                    break;
                case "3√x":
                    if (!string.IsNullOrEmpty(textoActual) && char.IsDigit(textoActual[textoActual.Length - 1]))
                        textBox1.Text += "√";
                    else
                        textBox1.Text += "3√";
                    break;
                case "y√x":
                    if (!string.IsNullOrEmpty(textoActual) && char.IsDigit(textoActual[textoActual.Length - 1]))
                        textBox1.Text += "√";
                    else
                        MessageBox.Show("Ingrese un número antes de usar y√x");
                    break;
                case "10^x":
                    textBox1.Text += "10^";
                    break;
                case "EXP":
                    textBox1.Text += "EXP";
                    break;
                case "log":
                    textBox1.Text += "log";
                    break;
            }
        }

        // Manejador para realizar la operación trigonométrica al presionar el botón
        private void BotonTrigonometrico_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    Button boton = (Button)sender;
                    double valor = Convert.ToDouble(textBox1.Text);
                    double resultado = 0;

                    // Determinar la operación trigonométrica según el botón presionado
                    if (boton.Text == "sin")
                    {
                        textBox1.Text = $"sin({valor})";
                        resultado = operacionesTrigonométricas.Seno(valor);
                    }
                    else if (boton.Text == "cos")
                    {
                        textBox1.Text = $"cos({valor})";
                        resultado = operacionesTrigonométricas.Coseno(valor);
                    }
                    else if (boton.Text == "tan")
                    {
                        textBox1.Text = $"tan({valor})";
                        resultado = operacionesTrigonométricas.Tangente(valor);
                    }

                    // Mostrar el resultado en textBox2 y actualizar textBox1 para permitir encadenar operaciones
                    textBox2.Text = resultado.ToString();
                    //textBox1.Text = resultado.ToString();
                }
            }
            catch (Exception ex)
            {
                textBox2.Text = "Error: " + ex.Message;
            }
        }


        // Manejador para agregar el valor de los botones numéricos al textBox1
        private void BotonNumero_Click(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            textBox1.Text += boton.Text;
        }

        // Manejador para agregar operadores al textBox1
        private void BotonOperador_Click(object sender, EventArgs e)
        {
            Button boton = (Button)sender;
            textBox1.Text += " " + boton.Text + " ";
        }

        // Limpiar el contenido de ambos textBox
        private void BotonLimpiar_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        // Método para adjuntar los manejadores de eventos
        private void AdjuntarManejadoresDeEventos()
        {
            // Adjuntar BotonNumero_Click a los botones de números
            button17.Click += BotonNumero_Click; // 0
            button18.Click += BotonNumero_Click; // 1
            button21.Click += BotonNumero_Click; // 2
            button25.Click += BotonNumero_Click; // 3
            button19.Click += BotonNumero_Click; // 4
            button22.Click += BotonNumero_Click; // 5
            button26.Click += BotonNumero_Click; // 6
            button20.Click += BotonNumero_Click; // 7
            button23.Click += BotonNumero_Click; // 8
            button27.Click += BotonNumero_Click; // 9
            button24.Click += BotonNumero_Click; // .

            // Adjuntar BotonOperador_Click a los botones de operadores
            button28.Click += BotonOperador_Click; // +
            button29.Click += BotonOperador_Click; // -
            button31.Click += BotonOperador_Click; // x
            button30.Click += BotonOperador_Click; // ÷
            button34.Click += BotonOperador_Click; // %
            button6.Click += BotonOperacionAvanzada_Click;  // x^y
            button11.Click += BotonOperacionAvanzada_Click;  // x^3
            button15.Click += BotonOperacionAvanzada_Click;  // x^2
            button10.Click += BotonOperacionAvanzada_Click;  // 3√x
            button14.Click += BotonOperacionAvanzada_Click;  // y√x
            button7.Click += BotonOperacionAvanzada_Click;  // 10^x
            button9.Click += BotonOperacionAvanzada_Click;  // EXP
            button13.Click += BotonOperacionAvanzada_Click; // log


            // Adjuntar manejadores de eventos a los botones de operaciones científicas
            button35.Click += BotonOperador_Click; // raiz
            button8.Click += BotonOperador_Click;  // π

            // Adjuntar BotonLimpiar_Click a los botones de limpieza
            button2.Click += BotonLimpiar_Click; // CE
            button1.Click += BotonLimpiar_Click; // C

            // Adjuntar el manejador de eventos al botón de igual
            button32.Click += button32_Click;
            button4.Click += Button_Borrar_Click; // Borrar último carácter

            // Adjuntar manejadores de eventos a los botones de operaciones trigonométricas
            button5.Click += BotonTrigonometrico_Click;  // sen
            button12.Click += BotonTrigonometrico_Click; // cos
            button16.Click += BotonTrigonometrico_Click; // tan
        }

        private void Button_Borrar_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                if (textBox1.Text.EndsWith(" "))
                {
                    textBox1.Text = textBox1.Text.Trim();
                    operadorPresionado = false;
                }
            }
        }

        public class Exponenciacion
        {
            public double Potencia(double baseNumero, double exponente) => Math.Pow(baseNumero, exponente);
        }
    }
}