using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Corrector_ortografico
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string NombreArchivo;
        string Texto;
        string Parametros = @"[\.\?\!]\s+([a-z])";
        bool Guardado = false;
        bool Negritas = false;
        bool Cursiva = false;
        bool Subrayado = false;
        int Cantidad_Ventanas = 1;
        SaveFileDialog GuardarArchivo = new SaveFileDialog();
        OpenFileDialog AbrirArchivo = new OpenFileDialog();

        public void NuevoArchivo()
        {
            TxtTexto.Clear();
            NombreArchivo = null;
        }

        public void Abrir()
        {
            if (string.IsNullOrEmpty(TxtTexto.Text))
            {
                AbrirArchivo.Filter = "Texto|*.txt";
                if (AbrirArchivo.ShowDialog() == true)  
                {
                    NombreArchivo = AbrirArchivo.FileName;
                    using (StreamReader SR = new StreamReader(NombreArchivo))
                    {
                        TxtTexto.Text = SR.ReadToEnd();
                    }
                }
            }
            else if (string.IsNullOrEmpty(TxtTexto.Text) == false && Guardado == false)
            {
                MessageBoxResult Dialogo = MessageBox.Show("Quieres guardar los cambios?", "Abrir nuevo archivo",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (Dialogo == MessageBoxResult.Yes)
                {
                    Guardar();
                    AbrirArchivo.Filter = "Texto|*.txt";
                    if (AbrirArchivo.ShowDialog() == true)
                    {
                        NombreArchivo = AbrirArchivo.FileName;
                        using (StreamReader SR = new StreamReader(NombreArchivo))
                        {
                            TxtTexto.Text = SR.ReadToEnd();
                        }
                    }
                }
                else if (Dialogo == MessageBoxResult.No)
                {
                    AbrirArchivo.Filter = "Texto|*.txt";
                    if (AbrirArchivo.ShowDialog() == true)
                    {
                        NombreArchivo = AbrirArchivo.FileName;
                        using (StreamReader SR = new StreamReader(NombreArchivo))
                        {
                            TxtTexto.Text = SR.ReadToEnd();
                        }
                    }
                }
            }
        }

        public void Guardar()
        {
            if (string.IsNullOrEmpty(TxtTexto.Text) == true)
            {
                MessageBoxResult Dialogo = MessageBox.Show("No hay texto", "Texto vacio",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                TxtTexto.Focus();
                return;
            }
            GuardarArchivo.Filter = "Texto|*.txt";
            if (NombreArchivo != null)
            {
                using (StreamWriter SW = new StreamWriter(NombreArchivo))
                {
                    SW.Write(TxtTexto.Text);
                }
            }
            else
            {
                if (GuardarArchivo.ShowDialog() == true)
                {
                    NombreArchivo = GuardarArchivo.FileName;
                    using (StreamWriter SW = new StreamWriter(GuardarArchivo.FileName))
                    {
                        SW.Write(TxtTexto.Text);
                        Guardado = true;
                    }
                }
            }
        }

        public void ArchivoNuevo()
        {
            if (Guardado == false && string.IsNullOrEmpty(TxtTexto.Text))
            {
                Cantidad_Ventanas++;
                MainWindow Ventana = new MainWindow();
                Ventana.Show();
            }
            if (Guardado == false && string.IsNullOrEmpty(TxtTexto.Text) == false)
            {
                MessageBoxResult Dialogo = MessageBox.Show("En la misma ventana?", "Nuevo archivo",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (Dialogo == MessageBoxResult.Yes)
                {
                    NuevoArchivo();
                }
                else if (Dialogo == MessageBoxResult.No)
                {
                    Cantidad_Ventanas++;
                    MainWindow Ventana = new MainWindow();
                    Ventana.Show();
                }
            }
        }

        public void Imprimir()
        {
            PrintDialog ImprimirDocumento = new PrintDialog();
            if (string.IsNullOrEmpty(TxtTexto.Text) == false)
            {
                if (ImprimirDocumento.ShowDialog() == true)
                {
                    FlowDocument Documento = new FlowDocument();
                    Documento.PagePadding = new Thickness(50);
                    Documento.Blocks.Add(new Paragraph(new Run(TxtTexto.Text)));
                    ImprimirDocumento.PrintDocument((((IDocumentPaginatorSource)Documento).DocumentPaginator),
                        "Using Paginator");
                }
            }
            else
            {
                MessageBoxResult Dialogo = MessageBox.Show("No hay texto", "Documento vacio",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                TxtTexto.Focus();
                return;
            }
        }

        private void Txt_Changed(object sender, TextChangedEventArgs e)
        {
            Guardado = false;
            if (TxtTexto.Text.Length <= 0) return;
            string Letra = TxtTexto.Text.Substring(0, 1);
            if (Letra != Letra.ToUpper())
            {
                int PrimeraLetra = TxtTexto.SelectionStart;
                int LongitudPalabra = TxtTexto.SelectionLength;
                TxtTexto.SelectionStart = 0;
                TxtTexto.SelectionLength = 1;
                TxtTexto.SelectedText = Letra.ToUpper();
                TxtTexto.SelectionStart = PrimeraLetra;
                TxtTexto.SelectionLength = LongitudPalabra;
            }

        }

        private void Windows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Guardado == false && string.IsNullOrEmpty(TxtTexto.Text) == false)
            {
                MessageBoxResult Dialogo = MessageBox.Show("Quieres guardar los cambios?", "Salir",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (Dialogo == MessageBoxResult.Yes)
                {
                    Guardar();
                }
                else if (Dialogo == MessageBoxResult.No)
                {
                    if (Cantidad_Ventanas >= 1)
                    {
                        this.Visibility = Visibility.Hidden;
                        Cantidad_Ventanas--;
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                else if (Dialogo == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    TxtTexto.Focus();
                }
            }
        }

        private void Windows_Load(object sender, RoutedEventArgs e)
        {
            TxtTexto.Focus();
        }

        private void Txt_KeyUp(object sender, KeyEventArgs e)
        {
            Texto = TxtTexto.Text;

            if ((Char)KeyInterop.VirtualKeyFromKey(e.Key) == (Char)KeyInterop.VirtualKeyFromKey(Key.Enter)
                || (Char)KeyInterop.VirtualKeyFromKey(e.Key) == (Char)KeyInterop.VirtualKeyFromKey(Key.Space))
            {
                char[] charArray = Texto.ToCharArray();
                foreach (Match match in Regex.Matches(Texto, Parametros, RegexOptions.Singleline))
                {
                    charArray[match.Groups[1].Index] = Char.ToUpper(charArray[match.Groups[1].Index]);
                }
                string output = new string(charArray);
                TxtTexto.Clear();
                TxtTexto.Text = output;
                TxtTexto.CaretIndex = TxtTexto.Text.Length;
            }
        }

        private void CbbTipoDeLetra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtTexto.Focus();
        }

        private void CbbTamanoDeLetra_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            ComboBoxItem cbbi = (ComboBoxItem)cbb.SelectedItem;
            string TamanoLetra = (string)cbbi.Content;

            int temp;
            if (Int32.TryParse(TamanoLetra, out temp))
            {
                if (TxtTexto.Text != null)
                    TxtTexto.FontSize = temp;
            }
            TxtTexto.Focus();
        }

        private void BtnTextoAIzquierda_Click(object sender, RoutedEventArgs e)
        {
            TxtTexto.TextAlignment = TextAlignment.Left;
        }

        private void BtnTextoAlCentro_Click(object sender, RoutedEventArgs e)
        {
            TxtTexto.TextAlignment = TextAlignment.Center;
        }

        private void BtnTextoADerecha_Click(object sender, RoutedEventArgs e)
        {
            TxtTexto.TextAlignment = TextAlignment.Right;
        }

        private void BtnTextoJustificado_Click(object sender, RoutedEventArgs e)
        {
            TxtTexto.TextAlignment = TextAlignment.Justify;
        }

        private void BtnPrintFile_Click(object sender, RoutedEventArgs e)
        {
            Imprimir();
        }

        private void BtnNewFile_Click(object sender, RoutedEventArgs e)
        {
            ArchivoNuevo();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Abrir();
        }

        private void BtnNegrita_Click(object sender, RoutedEventArgs e)
        {
            if (Negritas == false)
            {
                TxtTexto.FontWeight = FontWeights.UltraBold;
                Negritas = true;
            }
            else if (Negritas == true)
            {
                TxtTexto.FontWeight = FontWeights.Normal;
                Negritas = false;
            }
        }

        private void BtnCursiva_Click(object sender, RoutedEventArgs e)
        {
            if (Cursiva == false)
            {
                TxtTexto.FontStyle = FontStyles.Italic;
                Cursiva = true;
            }
            else if (Cursiva == true)
            {
                TxtTexto.FontStyle = FontStyles.Normal;
                Cursiva = false;
            }
        }

        private void BtnSudbrayado_Click(object sender, RoutedEventArgs e)
        {
            if (Subrayado == false)
            {
                TxtTexto.TextDecorations = TextDecorations.Underline;
                Subrayado = true;
            }
            else if (Subrayado == true)
            {
                TxtTexto.TextDecorations = null;
                Subrayado = false;
            }
        }

        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            Guardar();
        }
    }
}