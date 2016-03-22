using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RsmgrImporter
{
    class Conversions
    {
        private Logger logger = new Logger();

        /// <summary>
        /// Converts a string into a DateTime, ensure that the format of string is correct.
        /// </summary>
        /// <param name="d">String in proper DateTime format</param>
        /// <returns>Result - is defaulted to DateTime.Now</returns>
        public DateTime ConvertToDate(string d)
        {
            DateTime adate = DateTime.Now;
            DateTime result = DateTime.Now;

            try
            {
                Convert.ToDateTime(d);
            }
            catch (Exception e)
            {
                logger.writeToLog("Cannot convert string " + d + " into DateTime. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts a string into a DateTime, ensure that the string format is correct.
        /// </summary>
        /// <param name="d">The string parameter to convert into DateTime.</param>
        /// <param name="def">The default parameter incase the string fails to convert</param>
        /// <returns>The DateTime conversion of the string.</returns>
        public DateTime ConvertToDate(string d, DateTime def)
        {
            DateTime adate = DateTime.Now;
            DateTime result = def;

            try
            {
                Convert.ToDateTime(d);
            }
            catch (Exception e)
            {
                logger.writeToLog("Cannot convert string " + d + " into DateTime. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts string into DateTime, ensure that the format of string is correct.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days">Number of days either + or - to default to.  Use 0 if you want current DateTime.</param>
        /// <returns>Result - is defaulted to DateTime.Now.AddDays(days)</returns>
        public DateTime ConvertToDate(string date, int days)
        {
            DateTime adate = DateTime.Now;
            DateTime result = DateTime.Now;
            try
            {
                Convert.ToDateTime(date);
            }
            catch (Exception e)
            {
                result.AddMonths(days);
                logger.writeToLog("Cannot convert string " + date + " into DateTime. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts integer to decimal.
        /// </summary>
        /// <param name="num">Int num to convert to decimal</param>
        /// <returns>Decimal - Default value is 0.</returns>
        public decimal ConvertToDecimal(int num)
        {
            decimal result = 0;
            try
            {
                result = Convert.ToDecimal(num);
            }
            catch (Exception e)
            {
                logger.writeToLog("Cannot convert int " + num.ToString() + " into decimal. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts string to decimal.
        /// </summary>
        /// <param name="num">string num to convert to decimal.</param>
        /// <returns>Decimal - Default value is 0.</returns>
        public decimal ConvertToDecimal(string num)
        {
            decimal result = 0;
            try
            {
                result = Convert.ToDecimal(num);
            }
            catch (Exception e)
            {
                logger.writeToLog("Cannot convert int " + num.ToString() + " into decimal. " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Converts string to int.
        /// </summary>
        /// <param name="num">String to convert to integer.</param>
        /// <returns>int - default is 0</returns>
        public int ConvertToInt(string num)
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(num);
            }
            catch (Exception e)
            {
                logger.writeToLog("Cannot convert int " + num.ToString() + " into decimal. " + e.Message);
            }
            return result;
        }

    }
}

