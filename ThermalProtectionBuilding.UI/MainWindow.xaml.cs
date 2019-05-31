using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThermalProtectionBuilding.UI.ViewModel;

namespace ThermalProtectionBuilding.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ====== FIELDS =================================================

        private readonly ThermalCalculatorViewModel _thermalCalculatorViewModel;

        #endregion


        #region ====== CONSTRUCTORS ===========================================

        public MainWindow()
        {
            _thermalCalculatorViewModel = new ThermalCalculatorViewModel();
            InitializeComponent();
        }

        #endregion


        #region ====== PROPERTIES public ======================================



        #endregion


        #region ====== METHODS private ========================================

        private void Button_Click_SetDemoDat(object sender, RoutedEventArgs e)
        {
            _thermalCalculatorViewModel.SetDemoData();
        }

        private void Button_Click_CalculateThickness(object sender, RoutedEventArgs e)
        {
            double thickness_meter = _thermalCalculatorViewModel.CalculateThickness();
            double thickness_millimeter = thickness_meter * 1000.0;
            string thickness_millimeter_asString = thickness_millimeter.ToString("0.0");
            __runTextBlock_thicknessCalculation.Text = thickness_millimeter_asString;
        }

        #endregion
    }
}
