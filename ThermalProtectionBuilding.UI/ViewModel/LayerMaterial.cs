using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalProtectionBuilding.UI.ViewModel
{
    public class LayerMaterial
    {
        #region ====== FIELDS =================================================

        /// <summary>
        /// Тип материала, например "Кирпич декоративный (бессер) на цементно-песчаном растворе".
        /// </summary>
        private string _typeMaterial = null;

        /// <summary>
        /// Толщина слоя
        /// [мм]
        /// </summary>
        private double _thickness = double.NaN;

        /// <summary>
        /// Плотность
        /// [кг/м^3]
        /// </summary>
        private double _density = double.NaN;

        /// <summary>
        /// Теплопроводность
        /// [Вт/(м * °C)]
        /// </summary>
        private double _thermalConductivity = double.NaN;

        /// <summary>
        /// Паропроницаемость
        /// [мг/(м*ч*Па)]
        /// </summary>
        private double _vaporPermeability = double.NaN;

        /// <summary>
        /// Термическое сопротивление.
        /// Рассчитывается на основе других свойств.
        /// [(м^2 * °C)/Вт]
        /// </summary>
        private double _thermalResistance = double.NaN;

        #endregion


        #region ====== CONSTRUCTORS ===========================================

        public LayerMaterial()
        {

        }

        #endregion


        #region ====== PROPERTIES public ======================================

        /// <summary>
        /// Тип материала, например "Кирпич декоративный (бессер) на цементно-песчаном растворе".
        /// </summary>
        public string TypeMaterial
        {
            get => _typeMaterial;
            set => _typeMaterial = value;
        }

        /// <summary>
        /// Толщина слоя
        /// [м]
        /// </summary>
        public double Thickness
        {
            get => _thickness;
            set => _thickness = value;
        }

        /// <summary>
        /// Плотность
        /// [кг/м^3]
        /// </summary>
        public double Density
        {
            get => _density;
            set => _density = value;

        }

        /// <summary>
        /// Теплопроводность
        /// [Вт/(м * °C)]
        /// </summary>
        public double ThermalConductivity
        {
            get => _thermalConductivity;
            set => _thermalConductivity = value;
        }

        /// <summary>
        /// Паропроницаемость
        /// [мг/(м*ч*Па)]
        /// </summary>
        public double VaporPermeability
        {
            get => _vaporPermeability;
            set => _vaporPermeability = value;
        }

        /// <summary>
        /// Термическое сопротивление.
        /// Рассчитывается на основе других свойств.
        /// [(м^2 * °C)/Вт]
        /// </summary>
        public double ThermalResistance
        {
            get => _thermalResistance;
            private set => _thermalResistance = value;
        }

        #endregion


        #region ====== METHODS public =========================================

        public void Update()
        {
            ThermalResistance = _thickness / _thermalConductivity;
        }

        public override string ToString()
        {
            return _typeMaterial;
        }

        #endregion


        #region ====== METHODS private ========================================



        #endregion
    }
}
