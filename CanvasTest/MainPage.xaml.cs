using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CanvasTest
{
    /// <summary>
    /// Test d'un Canvas pour tablette
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int minPenSize = 2;
        const int penSizeIncrement = 5;
        int penSize;


        public MainPage()
        {
            this.InitializeComponent();

            penSize = minPenSize + penSizeIncrement * PenThickness.SelectedIndex;

            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Red;
            drawingAttributes.Size = new Size(penSize, penSize);
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // rootPage = MainPage.Current;     Utilisé quand on passe de la main page à celle-ci
            outputGrid.Width = Window.Current.Bounds.Width;
            outputGrid.Height = Window.Current.Bounds.Height;
            inkCanvas.Width = Window.Current.Bounds.Width;
            inkCanvas.Height = Window.Current.Bounds.Height;
        }

        private void OnPenTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inkCanvas != null)
            {
                InkDrawingAttributes drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                string value = ((ComboBoxItem)PenType.SelectedItem).Content.ToString();

                if (value == "Ballpoint")
                {
                    drawingAttributes.Size = new Size(penSize, penSize);
                    drawingAttributes.PenTip = PenTipShape.Circle;
                    drawingAttributes.DrawAsHighlighter = false;
                    drawingAttributes.PenTipTransform = System.Numerics.Matrix3x2.Identity;
                }
                else if (value == "Highlighter")
                {
                    if (Eraser_checkbox.IsChecked == true)   // If eraser enable, highlighter unavailable => conflict Transparent/highlight
                    {
                        System.Diagnostics.Debug.WriteLine("Pas de gomme possible en highlight");
                        Eraser_checkbox_Unchecked(Eraser_checkbox, new RoutedEventArgs());
                        Eraser_checkbox.IsChecked = false;
                        PenType.SelectedItem = default_combobox_item;
                        return;
                    }
                    // Make the pen rectangular for highlighter
                    drawingAttributes.Size = new Size(penSize, penSize * 2);
                    drawingAttributes.PenTip = PenTipShape.Rectangle;
                    drawingAttributes.DrawAsHighlighter = true;
                    drawingAttributes.PenTipTransform = System.Numerics.Matrix3x2.Identity;

                }
                if (value == "Calligraphy")
                {
                    drawingAttributes.Size = new Size(penSize, penSize * 2);
                    drawingAttributes.PenTip = PenTipShape.Rectangle;
                    drawingAttributes.DrawAsHighlighter = false;

                    // Set a 45 degree rotation on the pen tip
                    double radians = 45.0 * Math.PI / 180;
                    drawingAttributes.PenTipTransform = System.Numerics.Matrix3x2.CreateRotation((float)radians);
                }
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
                System.Diagnostics.Debug.WriteLine("aprés validation");

            }
        }

        private void OnPenThicknessChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inkCanvas != null)
            {
                System.Diagnostics.Debug.WriteLine("InkCanvas = true");
                InkDrawingAttributes drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                penSize = minPenSize + penSizeIncrement * PenThickness.SelectedIndex;
                string value = ((ComboBoxItem)PenType.SelectedItem).Content.ToString();
                if (value == "Highlighter" || value == "Calligraphy")
                {
                    // Make the pen tip rectangular for highlighter and calligraphy pen
                    drawingAttributes.Size = new Size(penSize, penSize * 2);
                }
                else
                {
                    drawingAttributes.Size = new Size(penSize, penSize);
                }
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            }
        }

        private void OnPenColorChanged(object sender, RoutedEventArgs e)
        {
            if (inkCanvas != null)
            {
                InkDrawingAttributes drawingAttributes = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();

                // Use button's background to set new pen's color
                var btnSender = sender as Button;
                var brush = btnSender.Background as SolidColorBrush;
                drawingAttributes.Color = brush.Color;
                System.Diagnostics.Debug.WriteLine(drawingAttributes.ToString());
                inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
            }
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void Eraser_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if(PenType.SelectedItem == highlighter_combobox_item)
            {
                Eraser_checkbox.IsChecked = false;
                System.Diagnostics.Debug.WriteLine("Eraser not available in highlight mode");
                return;
            }
            Button transparent_button = new Button();
            transparent_button.Background = new SolidColorBrush(Colors.Transparent);
            OnPenColorChanged(transparent_button,new RoutedEventArgs());
        }

        private void Eraser_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OnPenColorChanged(default_button,new RoutedEventArgs());
        }

    }
}

