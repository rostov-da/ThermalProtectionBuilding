using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalProtectionBuilding.UI.ViewModel
{
    /// <summary>
    /// Теплотехнический расчет стены из нескольких слоёв.
    /// </summary>1
    public class ThermalCalculatorViewModel : NotifyObject
    {
        #region ====== FIELDS =================================================

        /// <summary>
        /// Слой стены, от внешней к внутренней поверхности стены здания.
        /// Внешняя - на улице, внутренняя - внутри помещения.
        /// </summary>
        private readonly ObservableCollection<LayerMaterialViewModel> _layers;

        /// <summary>
        /// Имя города, например "Нижний Новгород".
        /// </summary>
        private string _nameCity;

        /// <summary>
        /// Тип здания (назначение здания, например "жилое")
        /// </summary>
        private string _typeBuilding;

        /// <summary>
        /// Относительная влажность воздуха внутри здания.
        /// Расчетная относительная влажность внутреннего воздуха из условия не выпадения конденсата 
        /// на внутренних поверхностях наружных ограждений.
        /// (СНиП 23-02-2003 п.4.3. табл.1 для нормального влажностного режима)
        /// [проценты]
        /// </summary>
        private double _humidityAir_InsideBuilding;

        /// <summary>
        /// Оптимальная температура воздуха внутри здания в холодный период.
        /// (ГОСТ 30494-96 табл.1)
        /// [градусы цельсия]
        /// </summary>
        private double _temperatureAirOptimum_InsideBuilding;

        /// <summary>
        /// Расчетная температура наружного воздуха, 
        /// определяемая по температуре наиболее холодной пятидневки обеспеченностью 
        /// (СНиП 23-01-99 табл. 1 столбец 5);
        /// [градусы цельсия]
        /// </summary>
        private double _temperatureAir_OutsideBuilding;

        /// <summary>
        /// Продолжительность отопительного периода со средней суточной температурой наружного воздуха 8°С.
        /// (СНиП 23-01-99 табл. 1 столбец 11);
        /// [сутки]
        /// </summary>
        private double _durationHeatingPeriod;

        /// <summary>
        /// Средняя температура наружного воздуха за отопительный период.
        /// (СНиП 23-01-99 табл. 1 столбец 12).
        /// [градусы цельсия]
        /// </summary>
        private double _temperatureAverage_HeatingPeriod_OutsideBuilding;

        /// <summary>
        /// Коэффициент для расчёта нормативного значения приведенного сопротивления теплопередаче.
        /// Берётся из СНиП 23-02-2003 таблице 4 
        /// или если для стен жилого здания СП 50.13330.2012 таблице 3 столбец 3
        /// </summary>
        private double _coeff_A_heatTransfer;

        /// <summary>
        /// Коэффициент для расчёта нормативного значения приведенного сопротивления теплопередаче.
        /// Берётся из СНиП 23-02-2003 таблице 4 
        /// или если для стен жилого здания СП 50.13330.2012 таблице 3 столбец 3
        /// </summary>
        private double _coeff_B_heatTransfer;

        /// <summary>
        /// Коэффициент для определения нормативного(максимально допустимого) сопротивления теплопередаче.
        /// берётся из [СНиП 23-02-2003(СП 50.13330.2012)] таблица 6 для наружной стены;
        /// </summary>
        private double _coeff_n;

        /// <summary>
        /// Коэффициент для определения нормативного(максимально допустимого) сопротивления теплопередаче.
        /// Нормируемый температурный перепад между температурой внутреннего воздуха и 
        /// температурой внутренней поверхности ограждающей конструкции, 
        /// принимается по [СНиП 23-02-2003(СП 50.13330.2012)] таблица 5.
        /// [°С]
        /// </summary>
        private double _deltaTemperatureNormalized;

        /// <summary>
        /// Коэффициент теплопередачи внутренней поверхности ограждающей конструкции,
        /// принимается по [СНиП 23-02-2003(СП 50.13330.2012)] таблица 5 для наружных стен.
        /// [Вт/(м2×°С)]
        /// </summary>
        private double _coeff_alpha_inside;

        /// <summary>
        /// Сопротивление теплообмену на наружной поверхности,
        /// принимается по [Пособие.Е.Г.Малявина "Теплопотери здания. Справочное пособие"] таблица 14 для наружных стен.
        /// </summary>
        private double _coeff_alpha_outside;

        #endregion


        #region ====== CONSTRUCTORS ===========================================

        public ThermalCalculatorViewModel()
        {
            _layers = new ObservableCollection<LayerMaterialViewModel>();
            
        }

        #endregion


        #region ====== PROPERTIES public ======================================

        /// <summary>
        /// Слой стены, от внешней к внутренней поверхности стены здания.
        /// Внешняя - на улице, внутренняя - внутри помещения.
        /// </summary>
        public ObservableCollection<LayerMaterialViewModel> Layers => _layers;

        /// <summary>
        /// Имя города, например "Нижний Новгород".
        /// </summary>
        public string NameCity
        {
            get => _nameCity;
            set
            {
                SetValueNotifyProperty(ref _nameCity, value);
            }
        }

        /// <summary>
        /// Тип здания (назначение здания, например "жилое")
        /// </summary>
        public string TypeBuilding
        {
            get => _typeBuilding;
            set
            {
                SetValueNotifyProperty(ref _typeBuilding, value);
            }
        }

        /// <summary>
        /// Относительная влажность воздуха внутри здания.
        /// Расчетная относительная влажность внутреннего воздуха из условия не выпадения конденсата 
        /// на внутренних поверхностях наружных ограждений.
        /// (СНиП 23-02-2003 п.4.3. табл.1 для нормального влажностного режима)
        /// [проценты]
        /// </summary>
        public double HumidityAir_InsideBuilding
        {
            get => _humidityAir_InsideBuilding;
            set
            {
                SetValueNotifyProperty(ref _humidityAir_InsideBuilding, value);
            }
        }

        /// <summary>
        /// Оптимальная температура воздуха внутри здания в холодный период.
        /// (ГОСТ 30494-96 табл.1)
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAirOptimum_InsideBuilding
        {
            get => _temperatureAirOptimum_InsideBuilding;
            set
            {
                SetValueNotifyProperty(ref _temperatureAirOptimum_InsideBuilding, value);
            }
        }

        /// <summary>
        /// Расчетная температура наружного воздуха, 
        /// определяемая по температуре наиболее холодной пятидневки обеспеченностью 
        /// (СНиП 23-01-99 табл. 1 столбец 5);
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAir_OutsideBuilding
        {
            get => _temperatureAir_OutsideBuilding;
            set
            {
                SetValueNotifyProperty(ref _temperatureAir_OutsideBuilding, value);
            }
        }

        /// <summary>
        /// Средняя температура наружного воздуха за отопительный период.
        /// (СНиП 23-01-99 табл. 1 столбец 12).
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAverage_HeatingPeriod_OutsideBuilding
        {
            get => _temperatureAverage_HeatingPeriod_OutsideBuilding;
            set
            {
                SetValueNotifyProperty(ref _temperatureAverage_HeatingPeriod_OutsideBuilding, value);
            }
        }

        /// <summary>
        /// Продолжительность отопительного периода со средней суточной температурой наружного воздуха 8°С.
        /// (СНиП 23-01-99 табл. 1 столбец 11);
        /// [сутки]
        /// </summary>
        public double DurationHeatingPeriod
        {
            get => _durationHeatingPeriod;
            set
            {
                SetValueNotifyProperty(ref _durationHeatingPeriod, value);
            }
        }

        /// <summary>
        /// Коэффициент для расчёта нормативного значения приведенного сопротивления теплопередаче.
        /// Берётся из СНиП 23-02-2003 таблице 4 
        /// или если для стен жилого здания СП 50.13330.2012 таблице 3 столбец 3
        /// </summary>
        public double Coeff_A_heatTransfer
        {
            get => _coeff_A_heatTransfer;
            set
            {
                SetValueNotifyProperty(ref _coeff_A_heatTransfer, value);
            }
        }

        /// <summary>
        /// Коэффициент для расчёта нормативного значения приведенного сопротивления теплопередаче.
        /// Берётся из СНиП 23-02-2003 таблице 4 
        /// или если для стен жилого здания СП 50.13330.2012 таблице 3 столбец 3
        /// </summary>
        public double Coeff_B_heatTransfer
        {
            get => _coeff_B_heatTransfer;
            set
            {
                SetValueNotifyProperty(ref _coeff_B_heatTransfer, value);
            }
        }

        /// <summary>
        /// Коэффициент для определения нормативного(максимально допустимого) сопротивления теплопередаче.
        /// берётся из [СНиП 23-02-2003(СП 50.13330.2012)] таблица 6 для наружной стены;
        /// </summary>
        public double Coeff_n
        {
            get => _coeff_n;
            set
            {
                SetValueNotifyProperty(ref _coeff_n, value);
            }
        }

        /// <summary>
        /// Коэффициент для определения нормативного(максимально допустимого) сопротивления теплопередаче.
        /// Нормируемый температурный перепад между температурой внутреннего воздуха и 
        /// температурой внутренней поверхности ограждающей конструкции, 
        /// принимается по [СНиП 23-02-2003(СП 50.13330.2012)] таблица 5.
        /// [°С]
        /// </summary>
        public double DeltaTemperatureNormalized
        {
            get => _deltaTemperatureNormalized;
            set
            {
                SetValueNotifyProperty(ref _deltaTemperatureNormalized, value);
            }
        }

        /// <summary>
        /// Коэффициент теплопередачи внутренней поверхности ограждающей конструкции,
        /// принимается по [СНиП 23-02-2003(СП 50.13330.2012)] таблица 5 для наружных стен.
        /// [Вт/(м2×°С)]
        /// </summary>
        public double Coeff_alpha_inside
        {
            get => _coeff_alpha_inside;
            set
            {
                SetValueNotifyProperty(ref _coeff_alpha_inside, value);
            }
        }

        /// <summary>
        /// Сопротивление теплообмену на наружной поверхности,
        /// принимается по [Пособие.Е.Г.Малявина "Теплопотери здания. Справочное пособие"] таблица 14 для наружных стен.
        /// </summary>
        public double Coeff_alpha_outside
        {
            get => _coeff_alpha_outside;
            set
            {
                SetValueNotifyProperty(ref _coeff_alpha_outside, value);
            }
        }

        #endregion


        #region ====== METHODS public =========================================

        /// <summary>
        /// Вычисляет необходимую толщину утеплителя.
        /// [м]
        /// </summary>
        /// <returns></returns>
        public double CalculateThickness()
        {
            // Формулы взяты http://svoydomtoday.ru/utepleniye-konstrukciy/210-teplotehnicheskiy-raschet-s-primerom.html

            // [1]: СНиП 23-02-2003(СП 50.13330.2012). "Тепловая защита зданий".Актуализированная редакция от 2012 года.
            // [2]: СНиП 23-01-99 (СП 131.13330.2012). "Строительная климатология".Актуализированная редакция от 2012 года.
            // [3]: СП 23-101-2004 "Проектирование тепловой защиты зданий".
            // [4]: ГОСТ 30494-96(заменен на ГОСТ 30494-2011 с 2011 года). "Здания жилые и общественные. Параметры микроклимата в помещениях".
            // [5]: Пособие.Е.Г.Малявина "Теплопотери здания. Справочное пособие".

            double temp_inside = _temperatureAirOptimum_InsideBuilding;
            double temp_outside = _temperatureAir_OutsideBuilding;
            double temp_average_outside = _temperatureAverage_HeatingPeriod_OutsideBuilding;

            // Определение градусо-суток отопительного периода [°C * сут]
            // по п.5.3 СНиП 23-02-2003
            double degreeDayHeatingPeriod = (temp_inside - temp_average_outside) * _durationHeatingPeriod;

            // Нормативное значение приведенного сопротивления теплопередаче
            // СНИП 23-02-2003 (табл.4)
            double resistanceHeatTransfer_energySaving = degreeDayHeatingPeriod * _coeff_A_heatTransfer + _coeff_B_heatTransfer;


            // Определение нормативного (максимально допустимого) сопротивления теплопередаче 
            // по условию санитарии (формула 3 СНиП 23-02-2003)
            double n = _coeff_n;
            double deltaTemp = _deltaTemperatureNormalized;
            double alpha = _coeff_alpha_inside;
            double resistanceHeatTransfer_sanitation = n * (temp_inside - temp_outside) / (deltaTemp * alpha);

            // Требуемое сопротивление теплопередачи выбираем из условия энергосбережения
            // почему?
            double resistanceHeatTransfer_required = resistanceHeatTransfer_energySaving;

            // Для каждого слоя заданной стены необходимо рассчитать термическое сопротивление по формуле

            List<LayerMaterialViewModel> layers_existing = _layers.Where(x => !double.IsNaN(x.Thickness)).ToList();
            List<LayerMaterialViewModel> layers_nonexisting = _layers.Where(x => double.IsNaN(x.Thickness)).ToList();
            if (layers_nonexisting.Count != 1)
                throw new Exception("Должн быть один слой, для которого не задано значение толщины thickness = NaN, для этого слоя будет рассчитана его толщина.");
            LayerMaterialViewModel layer_nonexisting = layers_nonexisting[0];

            layers_existing.ForEach(x => x.Update());
            double resistanceHeatTransfer_sumExisting = layers_existing.Sum(x => x.ThermalResistance);

            // сопротивление теплообмену на внутренней поверхности;
            double resistanceHeatTransfer_inside = 1 / _coeff_alpha_inside;
            double resistanceHeatTransfer_outside = 1 / _coeff_alpha_outside;


            double R_req = resistanceHeatTransfer_required;
            double R_in = resistanceHeatTransfer_inside;
            double R_out = resistanceHeatTransfer_outside;
            double R_sum = resistanceHeatTransfer_sumExisting;
            double resistanceHeatTransfer_minimum = R_req - (R_in + R_out + R_sum);

            // толщина утеплителя равна (формула 5,7 [Пособие.Е.Г.Малявина "Теплопотери здания. Справочное пособие"])
            double thickness = layer_nonexisting.ThermalConductivity * resistanceHeatTransfer_minimum;
            return thickness;
        }

        public void SetDemoData()
        {
            Layers.Clear();
            NameCity = "Нижний Новгород";
            TypeBuilding = "жилое";
            HumidityAir_InsideBuilding = 55;
            TemperatureAirOptimum_InsideBuilding = 20;
            TemperatureAir_OutsideBuilding = -31;
            DurationHeatingPeriod = 215;
            TemperatureAverage_HeatingPeriod_OutsideBuilding = -4.1;
            Coeff_A_heatTransfer = 0.00035;
            Coeff_B_heatTransfer = 1.4;
            Coeff_n = 1.0;
            DeltaTemperatureNormalized = 4.0;
            Coeff_alpha_inside = 8.7;
            Coeff_alpha_outside = 23.0;

            var layer_1 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Кирпич декоративный (бессер) на цементно-песчаном растворе",
                Thickness = 90.0 / 1000.0,
                Density = 2300.0,
                ThermalConductivity = 0.96,
                VaporPermeability = 0.1,
            };

            var layer_2 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Утеплитель (минераловатная плита)",
                Thickness = double.NaN,
                Density = 250.0,
                ThermalConductivity = 0.085,
                VaporPermeability = 0.41,
            };

            var layer_3 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Cиликатный кирпич на цементно-песчаном растворе",
                Thickness = 250.0 / 1000.0,
                Density = 1800.0,
                ThermalConductivity = 0.87,
                VaporPermeability = 0.11,
            };

            var layer_4 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Штукатурка (сложный раствор)",
                Thickness = 20.0 / 1000.0,
                Density = 1700.0,
                ThermalConductivity = 0.87,
                VaporPermeability = 0.098,
            };

            _layers.Add(layer_1);
            _layers.Add(layer_2);
            _layers.Add(layer_3);
            _layers.Add(layer_4);
        }

        #endregion


        #region ====== METHODS private ========================================



        #endregion
    }
}
