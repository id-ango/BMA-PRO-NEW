using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Fungsi.Function
{
    public class Fungsi
    {
       
        public static string BilanganToText(decimal n)
        {
            return NumberToText((long) n) + ConvertToText(n); 
        }


        private static string ConvertToText(decimal n)
        {
            string desimal = n.ToString();
            string points = "";
            string andStr = "";
            string pointStr = "";

            int decimalPlace = -1;

             if(desimal.IndexOf(",") > 0)
            {
                decimalPlace = desimal.IndexOf(",");
                
            } 
            else
            {
                decimalPlace = desimal.IndexOf(".");
            }

            if (decimalPlace > 0)
            {
               
                points = desimal.Substring(decimalPlace + 1);
                if (Convert.ToInt32(points) > 0)
                {
                    andStr = "koma";// just to separate whole numbers from points/cents    
                 //   endStr = "cent " + endStr;//Cents    
                    pointStr = ConvertDecimals(points);
                }
            }
            return andStr+ " "+pointStr;
        }

        private static string NumberToText(long n)
        {
           
            if (n < 0)
                return "Minus " + NumberToText(-n);
            else if (n == 0)
                return "";           
            else if (n <= 19)
                return new string[] {"Satu", "Dua", "Tiga", "Empat", "Lima", "Enam", "Tujuh", "Delapan",
                                "Sembilan", "Sepuluh", "Sebelas", "Duabelas", "Tigabelas", "Empatbelas", "Limabelas", "Enambelas",
                                "Tujuhbelas", "Delapanbelas", "Sembilanbelas"}[n - 1] + " ";
            else if (n <= 99)
                return new string[] {"Duapuluh", "Tigapuluh", "Empatpuluh", "Limapuluh", "Enampuluh", "Tujuhpuluh",
                                "Delapanpuluh", "Sembilanpuluh"}[n / 10 - 2] + " " + NumberToText(n % 10);
            else if (n <= 199)
                return "Seratus " + NumberToText(n % 100);
            else if (n <= 999)
                return NumberToText(n / 100) + "Ratus " + NumberToText(n % 100);
            else if (n <= 1999)
                return "Seribu " + NumberToText(n % 1000);
            else if (n <= 999999)
                return NumberToText(n / 1000) + "Ribu " + NumberToText(n % 1000);
            else if (n <= 1999999)
                return "Satu Juta " + NumberToText(n % 1000000);
            else if (n <= 999999999)
                return NumberToText(n / 1000000) + "Juta " + NumberToText(n % 1000000);
            else if (n <= 1999999999)
                return "Satu Milyard " + NumberToText(n % 1000000000);
            else
                return NumberToText(n / 1000000000) + "Milyard " + NumberToText(n % 1000000000);
        }

        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "Satu";
                    break;
                case 2:
                    name = "Dua";
                    break;
                case 3:
                    name = "Tiga";
                    break;
                case 4:
                    name = "Empat";
                    break;
                case 5:
                    name = "Lima";
                    break;
                case 6:
                    name = "Enam";
                    break;
                case 7:
                    name = "Tujuh";
                    break;
                case 8:
                    name = "Delapan";
                    break;
                case 9:
                    name = "Sembilan";
                    break;
            }
            return name;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Nol";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }

       
    }
}
