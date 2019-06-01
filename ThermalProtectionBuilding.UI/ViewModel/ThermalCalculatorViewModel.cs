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
        /// Вид ограждающей конструкции.
        /// </summary>
        private string _typeWall;

        /// <summary>
        /// Относительная влажность воздуха внутри здания.
        /// Расчетная относительная влажность внутреннего воздуха из условия не выпадения конденсата 
        /// на внутренних поверхностях наружных ограждений.
        /// (СНиП 23-02-2003 п.4.3. табл.1 для нормального влажностного режима)
        /// [проценты]
        /// </summary>
        private double _humidityAir_inside;

        /// <summary>
        /// Оптимальная температура воздуха внутри здания в холодный период.
        /// (ГОСТ 30494-96 табл.1)
        /// [градусы цельсия]
        /// </summary>
        private double _temperatureAir_inside;

        /// <summary>
        /// Расчетная температура наружного воздуха, 
        /// определяемая по температуре наиболее холодной пятидневки обеспеченностью 
        /// (СНиП 23-01-99 табл. 1 столбец 5);
        /// [градусы цельсия]
        /// </summary>
        private double _temperatureAir_outside;

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
        private double _temperatureAverage_heatingPeriod_outside;

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
        /// Нормируемый температурный перепад между температурой внутреннего воздуха
        /// и температурой внутренней поверхности ограждающей конструкции.
        /// СП 50.13330.2012 таблица 5 стр 7
        /// [°С]
        /// </summary>
        private double _deltaTemperatureNormalized;

        /// <summary>
        /// Коэффициент теплоотдачи внутренней поверхности ограждающей конструкции.
        /// СП 50.13330.2012 таблица 4 стр 6
        /// [Вт/(м2×°С)]
        /// </summary>
        private double _coeff_alpha_inside;

        /// <summary>
        /// Коэффициент теплоотдачи наружной поверхности огрождающей конструкии
        /// СП 50.13330.2012 таблица 6 стр 8
        /// [Вт/(м2×°С)]
        /// </summary>
        private double _coeff_alpha_outside;

        /// <summary>
        /// Коэффициент теплотехнической однородности ограждающей конструкции, 
        /// учитывающий влияние стыков, откосов проемов, обрамляющих ребер, гибких связей и других теплопроводных включений
        /// </summary>
        private double _coefficientHomogeneity;

        /// <summary>
        /// Результат расчётов.
        /// Базовое значение требуемого сопротивления теплопередачи 
        /// м2·°С/Вт.
        /// </summary>
        private double _result_resistanceHeatTransfer_baseRequired;

        /// <summary>
        /// Приведенное сопротивление теплопередаче для стены.
        /// м2·°С/Вт.
        /// </summary>
        private double _result_resistanceHeatTransfer_reduce_wall;

        /// <summary>
        /// True - ограждающая конструкция соответствует требованиям по теплопередаче.
        /// </summary>
        private bool _result_isMeetsStandarts;


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
        /// Тип ограждающей конструкии.
        /// </summary>
        public string TypeWall
        {
            get => _typeWall;
            set
            {
                SetValueNotifyProperty(ref _typeWall, value);
            }
        }

        /// <summary>
        /// Относительная влажность воздуха внутри здания.
        /// Расчетная относительная влажность внутреннего воздуха из условия не выпадения конденсата 
        /// на внутренних поверхностях наружных ограждений.
        /// (СНиП 23-02-2003 п.4.3. табл.1 для нормального влажностного режима)
        /// [проценты]
        /// </summary>
        public double HumidityAir_inside
        {
            get => _humidityAir_inside;
            set
            {
                SetValueNotifyProperty(ref _humidityAir_inside, value);
            }
        }

        /// <summary>
        /// Оптимальная температура воздуха внутри здания в холодный период.
        /// (ГОСТ 30494-96 табл.1)
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAir_inside
        {
            get => _temperatureAir_inside;
            set
            {
                SetValueNotifyProperty(ref _temperatureAir_inside, value);
            }
        }

        /// <summary>
        /// Расчетная температура наружного воздуха, 
        /// определяемая по температуре наиболее холодной пятидневки обеспеченностью 
        /// (СНиП 23-01-99 табл. 1 столбец 5);
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAir_outside
        {
            get => _temperatureAir_outside;
            set
            {
                SetValueNotifyProperty(ref _temperatureAir_outside, value);
            }
        }

        /// <summary>
        /// Средняя температура наружного воздуха за отопительный период.
        /// (СНиП 23-01-99 табл. 1 столбец 12).
        /// [градусы цельсия]
        /// </summary>
        public double TemperatureAverage_heatingPeriod_outside
        {
            get => _temperatureAverage_heatingPeriod_outside;
            set
            {
                SetValueNotifyProperty(ref _temperatureAverage_heatingPeriod_outside, value);
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
        /// Нормируемый температурный перепад между температурой внутреннего воздуха
        /// и температурой внутренней поверхности ограждающей конструкции.
        /// СП 50.13330.2012 таблица 5 стр 7
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
        /// Коэффициент теплоотдачи внутренней поверхности ограждающей конструкции.
        /// СП 50.13330.2012 таблица 4 стр 6
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
        /// Коэффициент теплоотдачи наружной поверхности огрождающей конструкии
        /// СП 50.13330.2012 таблица 6 стр 8
        /// [Вт/(м2×°С)]
        /// </summary>
        public double Coeff_alpha_outside
        {
            get => _coeff_alpha_outside;
            set
            {
                SetValueNotifyProperty(ref _coeff_alpha_outside, value);
            }
        }

        /// <summary>
        /// Коэффициент теплотехнической однородности ограждающей конструкции, 
        /// учитывающий влияние стыков, откосов проемов, обрамляющих ребер, гибких связей и других теплопроводных включений
        /// </summary>
        public double CoefficientHomogeneity
        {
            get => _coefficientHomogeneity;
            set
            {
                SetValueNotifyProperty(ref _coefficientHomogeneity, value);
            }
        }

        /// <summary>
        /// Результат расчётов.
        /// Базовое значение требуемого сопротивления теплопередачи 
        /// м2·°С/Вт.
        /// </summary>
        public double Result_resistanceHeatTransfer_baseRequired
        {
            get => _result_resistanceHeatTransfer_baseRequired;
            set
            {
                SetValueNotifyProperty(ref _result_resistanceHeatTransfer_baseRequired, value);
            }
        }

        /// <summary>
        ///  Результат расчётов.
        /// Приведенное сопротивление теплопередаче для стены.
        /// м2·°С/Вт.
        /// </summary>
        public double Result_resistanceHeatTransfer_reduce_wall
        {
            get => _result_resistanceHeatTransfer_reduce_wall;
            set
            {
                SetValueNotifyProperty(ref _result_resistanceHeatTransfer_reduce_wall, value);
            }
        }

        /// <summary>
        ///  Результат расчётов.
        /// True - ограждающая конструкция соответствует требованиям по теплопередаче.
        /// </summary>
        public bool Result_isMeetsStandards
        {
            get => _result_isMeetsStandarts;
            set
            {
                SetValueNotifyProperty(ref _result_isMeetsStandarts, value);
            }
        }

        #endregion


        #region ====== METHODS public =========================================

        public void Calculate_IsMeetsStandards()
        {
            // Источник http://rascheta.net/
            // комментарии будет для конкретного случая - demo data

            // Согласно таблицы 1 СП 50.13330.2012 при температуре внутреннего воздуха здания tint = 20°C 
            // и относительной влажности воздуха φint = 55 % влажностный режим помещения устанавливается, как нормальный.

            // Определим базовое значение требуемого сопротивления теплопередаче Roтр 
            // исходя из нормативных требований к приведенному сопротивлению теплопередаче п. 5.2 СП 50.13330.2012 согласно формуле:
            // Roтр = a·ГСОП + b
            // где а и b-коэффициенты, значения которых следует приниматься по данным таблицы 3 СП 50.13330.2012 для соответствующих групп зданий.
            // Так для ограждающей конструкции вида - наружные стены и типа здания - жилые а=0.00035;b=1.4

            // Определим градусо-сутки отопительного периода ГСОП, 0С·сут по формуле (5.2) СП 50.13330.2012
            // ГСОП = (tв - tот)zот

            double temp_inside = _temperatureAir_inside;
            double temp_outside = _temperatureAir_outside;
            double temp_average_outside = _temperatureAverage_heatingPeriod_outside;
            double degreeDayHeatingPeriod = (temp_inside - temp_average_outside) * _durationHeatingPeriod;

            //  По формуле в таблице 3 СП 50.13330.2012 определяем базовое значение требуемого сопротивления теплопередачи Roтр (м2·°С/Вт).
            double resistanceHeatTransfer_baseRequired = degreeDayHeatingPeriod * _coeff_A_heatTransfer + _coeff_B_heatTransfer;

            // Поскольку населенный пункт Санкт-Петербург относится к зоне влажности - влажной, 
            // при этом влажностный режим помещения - нормальный, то в соответствии 
            // с таблицей 2 СП50.13330.2012 теплотехнические характеристики материалов ограждающих конструкций 
            // будут приняты, как для условий эксплуатации Б.

            // Определим условное сопротивление теплопередаче R0усл, (м2°С / Вт) определим по формуле E.6 СП 50.13330.2012:
            var layers = _layers.ToList();
            layers.ForEach(x => x.Update());
            double resistanceHeatTransfer_sumLayers = layers.Sum(x => x.ThermalResistance);

            // сопротивление теплообмену на внутренней поверхности;
            double resistanceHeatTransfer_inside = 1 / _coeff_alpha_inside;
            double resistanceHeatTransfer_outside = 1 / _coeff_alpha_outside;

            double resistanceHeatTransfer_wall = resistanceHeatTransfer_inside + resistanceHeatTransfer_outside + resistanceHeatTransfer_sumLayers;

            // Приведенное сопротивление теплопередаче R0пр, (м2°С/Вт) определим по формуле 11 СП 23-101-2004:
            double resistanceHeatTransfer_reduced_wall = resistanceHeatTransfer_wall * _coefficientHomogeneity;

            bool isMeetsStandards = resistanceHeatTransfer_reduced_wall > resistanceHeatTransfer_baseRequired;

            Result_isMeetsStandards = isMeetsStandards;
            Result_resistanceHeatTransfer_baseRequired = resistanceHeatTransfer_baseRequired;
            Result_resistanceHeatTransfer_reduce_wall = resistanceHeatTransfer_reduced_wall;
        }

        public void SetDemoData()
        {
            Result_resistanceHeatTransfer_baseRequired = 0;
            Result_resistanceHeatTransfer_reduce_wall = 0;
            Result_isMeetsStandards = false;

            Layers.Clear();

            NameCity = "Санкт-Петербург";
            TypeBuilding = "Жилое";
            TypeWall = "Наружные стены";
            HumidityAir_inside = 55;
            TemperatureAir_inside = 20;
            TemperatureAir_outside = -24;
            TemperatureAverage_heatingPeriod_outside = -1.3;
            DurationHeatingPeriod = 213;
            Coeff_A_heatTransfer = 0.00035;
            Coeff_B_heatTransfer = 1.4;
            DeltaTemperatureNormalized = 4.0;
            Coeff_alpha_inside = 8.7;
            Coeff_alpha_outside = 23.0;
            CoefficientHomogeneity = 0.92;

            var layer_1 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Кладка из керамического пустотного кирпича ГОСТ 530(p=1300кг/м.куб)",
                Thickness = 0.12,
                ThermalConductivity = 0.58,
            };

            var layer_2 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Маты минераловатные ГОСТ 21880 (p=125 кг/м.куб)",
                Thickness = 0.25,
                ThermalConductivity = 0.07,
            };

            var layer_3 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Железобетон (ГОСТ 26633)",
                Thickness = 0.2,
                ThermalConductivity = 2.04,
            };

            var layer_4 = new LayerMaterialViewModel()
            {
                TypeMaterial = "Фанера клееная (ГОСТ 8673)",
                Thickness = 0.01,
                ThermalConductivity = 0.18,
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
