using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos
{
    public static class Palette
    {
        public static Color black = new Color(20, 12, 28);
        public static Color darkRed = new Color(68, 36, 52);
        public static Color darkBlue = new Color(48, 52, 109);
        public static Color darkGray = new Color(78, 74, 78);
        public static Color brown = new Color(133, 76, 48);
        public static Color green = new Color(52, 101, 36);
        public static Color red = new Color(208, 70, 72);
        public static Color gray = new Color(117, 113, 97);
        public static Color blue = new Color(89, 125, 206);
        public static Color yellow = new Color(218, 212, 94);
        public static Color tan = new Color(210, 170, 153);
        public static Color lightBrown = new Color(210, 125, 44);
        public static Color lightGray = new Color(133, 149, 161);
        public static Color lightGreen = new Color(109, 170, 44);
        public static Color lightBlue = new Color(109, 194, 202);
        public static Color white = new Color(222, 238, 214);
        public static Dictionary<String, Color> colors = new Dictionary<String, Color>();

        public static void Init()
        {
            colors.Add("black", black);
            colors.Add("darkRed", darkRed);
            colors.Add("darkBlue", darkBlue);
            colors.Add("darkGray", darkGray);
            colors.Add("brown", brown);
            colors.Add("green", green);
            colors.Add("red", red);
            colors.Add("gray", gray);
            colors.Add("blue", blue);
            colors.Add("yellow", yellow);
            colors.Add("tan", tan);
            colors.Add("lightBrown", lightBrown);
            colors.Add("lightGray", lightGray);
            colors.Add("lightGreen", lightGreen);
            colors.Add("lightBlue", lightBlue);
            colors.Add("white", white);
        }
    }
}
