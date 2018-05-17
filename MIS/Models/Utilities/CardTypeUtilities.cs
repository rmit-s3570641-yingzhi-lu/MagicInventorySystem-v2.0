using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MIS.Utilities
{
    public enum CardType
    {
        Unknown = 0,
        MasterCard = 1,
        VISA = 2,
        Amex = 3
    }

    // Class to hold credit card type information
    public class CardTypeInfo
    {
        public CardTypeInfo(string regEx, int length, CardType type)
        {
            RegEx = regEx;
            Length = length;
            Type = type;
        }

        public string RegEx { get; set; } 
        public int Length { get; set; } 
        public CardType Type { get; set; }

        // Array of CardTypeInfo objects.
        // Used by GetCardType() to identify credit card types.
        private readonly static CardTypeInfo[] _cardTypeInfo =
        {
            new CardTypeInfo("^(51|52|53|54|55)", 16, CardType.MasterCard),
            new CardTypeInfo("^(4)", 16, CardType.VISA),
            new CardTypeInfo("^(4)", 13, CardType.VISA),
            new CardTypeInfo("^(34|37)", 15, CardType.Amex)
        };

        public static CardType GetCardType(string cardNumber)
        {
            if(cardNumber != null)
            {
                foreach(CardTypeInfo info in _cardTypeInfo)
                {
                    if(cardNumber.Length == info.Length &&
                       Regex.IsMatch(cardNumber, info.RegEx))
                    {
                        return info.Type;
                    }
                }
            }

            return CardType.Unknown;
        }
    }
}
