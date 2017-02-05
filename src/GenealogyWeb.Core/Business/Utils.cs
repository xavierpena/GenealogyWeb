using GenealogyWeb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Business
{
    public static class Utils
    {
        public const string UnknownStr = "✗";
        public const string MaleSignStr = "♂";
        public const string FemaleSignStr = "♀";
        public const string MarriageSignStr = "⚤";

        public const string AlertStr = "⚠";

        /// <summary>
        /// Full person description formatted as:
        /// `male/female_sign name/surname1/surname2 (birth=>death) id=XXX`
        /// </summary>
        public static string GetPersonDescription(Persona person)
            => $"{ (person.home ? MaleSignStr : FemaleSignStr) } {GetStr(person.nom)}/{GetStr(person.llinatge_1)}/{GetStr(person.llinatge_2)}"
                + $" ({GetYear(person.naixement_data) }=>{GetYear(person.mort_data)})"
                + $" id={person.id}";

        /// <summary>
        /// Cleans the string.
        /// If the string shows no info, returns `UnknownStr`
        /// </summary>
        public static string GetStr(string inputStr)
        {
            if (inputStr == null)
                return UnknownStr;

            inputStr = inputStr.Trim();
            if (inputStr == "")
                return UnknownStr;

            return inputStr;
        }

        /// <summary>
        /// Gets only the year from the date.
        /// Expectects a date formatted as 'yyyy-MM-dd'
        /// </summary>
        public static string GetYear(string inputStr)
        {
            // Clean the input string first:
            var transformed = GetStr(inputStr);

            if (transformed != UnknownStr)
                return inputStr.Split('-')[0];
            else
                return transformed;
        }

        internal static string GetMarriageDescription(Matrimoni marriage)
            => $"{MarriageSignStr} Marriage ({ Utils.GetYear(marriage.data) }) id={marriage.id}";

        public static string GetDuplicateStr(Persona person)
            => $"[ {AlertStr} DUPLICATE ] {GetPersonDescription(person)}";

        public static string GetDuplicateStr(string description)
            => $"[ {AlertStr} DUPLICATE ] {description}" ;
    }
}
