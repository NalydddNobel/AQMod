using Aequus.Common.ID;

namespace Aequus.Content
{
    public interface ITemperatureEntity
    {        
        /// <summary>
        /// The entity's temperature.
        /// </summary>
        int Temperature { get; set; }
        /// <summary>
        /// A conversion property automatically given by <see cref="ITemperatureEntity"/>
        /// </summary>
        TemperatureType CurrentTemperature => TemperatureTypeConverter.FromTimer(Temperature);

        /// <summary>
        /// Allows the entity to adjust temperature stats which will be given to them.
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="minTemperature"></param>
        /// <param name="maxTemperature"></param>
        void ModifyTemperatureApplication(ref int temperature, ref int minTemperature, ref int maxTemperature);
    }
}