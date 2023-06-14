using MaterialDesignMessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacoApp
{
    class ClassError
    {
        public int Err(string Furn, int FFH, int FFB, int quantity, string rotation)
        {
            if (Furn == "Maco_Eco")
            {
                if (FFH < 601)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 601 мм");
                    return 1;
                }
                else if (FFB < 400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 400 мм");
                    return 1;
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2350 мм");
                    return 1;
                }
                else if (FFB > 1300)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1300 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Maco_MM" && rotation == "Нет")
            {
                if (FFH < 470)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Maco MM не может быть менее 470 мм");
                    return 1;
                }
                else if (FFB < 320)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Maco MM не может быть менее 320 мм");
                    return 1;
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Maco MM не может быть более 2250 мм");
                    return 1;
                }
                else if (FFB > 1300)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Maco MM не может быть ,более 1300 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Maco_MM" && rotation == "Да")
            {
                if (FFH < 300)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 300 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2400 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Vorne" && rotation == "Нет")
            {
                if (FFH < 450)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Vorne не может быть менее 450 мм");
                    return 1;
                }
                else if (FFB < 350)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Vorne не может быть менее 350 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Vorne не может быть более 2400 мм");
                    return 1;
                }
                else if (FFB > 1100)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Vorne не может быть ,более 1100 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Vorne" && rotation == "Да")
            {
                if (FFH < 300)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 300 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2400 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Roto_NT" && rotation == "Нет")
            {
                if (FFH < 310)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Roto_NT не может быть менее 310 мм");
                    return 1;
                }
                else if (FFB < 290)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Roto_NT не может быть менее 290 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Roto_NT не может быть более 2400 мм");
                    return 1;
                }
                else if (FFB > 1400)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Roto_NT не может быть ,более 1400 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Internika" && rotation == "Нет")
            {
                if (FFH < 420)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Internika не может быть менее 420 мм");
                    return 1;
                }
                else if (FFB < 325)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Internika не может быть менее 325 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Internika не может быть более 2400 мм");
                    return 1;
                }
                else if (FFB > 1010)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Internika не может быть ,более 1010 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Furn == "Internika" && rotation == "Да")
            {
                if (FFH < 501)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 501 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2400 мм");
                    return 1;
                }
                else if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }
}
