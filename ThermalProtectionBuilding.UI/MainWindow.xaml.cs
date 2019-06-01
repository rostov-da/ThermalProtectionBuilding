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

        public ThermalCalculatorViewModel ThermalCalculator => _thermalCalculatorViewModel;

        #endregion


        #region ====== METHODS private ========================================

        private void Button_Click_SetDemoDat(object sender, RoutedEventArgs e)
        {
            _thermalCalculatorViewModel.SetDemoData();
        }

        private void Button_Click_Calculate_IsMeetsStandards(object sender, RoutedEventArgs e)
        {
            _thermalCalculatorViewModel.Calculate_IsMeetsStandards();

            double R_minRequired = _thermalCalculatorViewModel.Result_resistanceHeatTransfer_baseRequired;
            double R_actual = _thermalCalculatorViewModel.Result_resistanceHeatTransfer_reduce_wall;
            bool isMeetsStandards = _thermalCalculatorViewModel.Result_isMeetsStandards;
            string text = "";
            if (isMeetsStandards)
                text = $"СООТВЕТСТВУЕТ стандартам, т. к. R_actual = {R_actual:0.0} > R_minRequired = {R_minRequired:0.0}";
            else
                text = $"НЕ СООТВЕТСТВУЕТ стандартам, т. к. R_actual = {R_actual:0.0} < R_minRequired = {R_minRequired:0.0}";

            text += Environment.NewLine;
            text += "R_actual - приведенное сопротивление теплопередачи, м2·°С/Вт.";
            text += Environment.NewLine;
            text += "R_actual - базовое значение требуемого сопротивления теплопередачи , м2·°С/Вт.";

            __textBlock_result_isMeetsStandards.Text = text;
        }

        #endregion


    }
}
