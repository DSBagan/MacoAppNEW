using MaterialDesignMessageBox;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacoApp
{
    class ClassError
    {
        public int ErrorPortal(string Furn, int FFH, int FFB, int quantity)
        {
            if (Furn == "Maco SKB")
            {
                if (FFB < 841)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 841 мм");
                    return 1;
                }
                else if (FFH < 620)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 620 мм");
                    return 1;
                }
                else if (FFB > 2250)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2250 мм");
                    return 1;
                }
                else if (FFH > 1650)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1650 мм");
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
            else if (Furn == "Vorne")
            {
                if (FFB < 450)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 450 мм");
                    return 1;
                }
                else if (FFH < 600)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 600 мм");
                    return 1;
                }
                else if (FFB > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2400 мм");
                    return 1;
                }
                else if (FFH > 1350)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1350 мм");
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
            else if (Furn == "Vorne")
            {
                if (FFB < 621)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 621 мм");
                    return 1;
                }
                else if (FFH < 600)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 600 мм");
                    return 1;
                }
                else if (FFB > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2400 мм");
                    return 1;
                }
                else if (FFH > 1650)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1650 мм");
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

        public int Err(string Furn, int FFH, int FFB, int quantity, string rotation, string framuga, string konst, string shtulp, string shtulpTreeArg)
        {
            if (Furn == "Maco_Eco" && framuga == "Нет")
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
            else if (Furn == "Maco_Eco" && framuga == "Да")
            {
                if (FFH < 601)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 601 мм");
                    return 1;
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 2350 мм");
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
            else if (Furn == "Maco_MM" && rotation == "Нет" && framuga == "Нет" && shtulp == "Нет")
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
            else if (Furn == "Maco_MM" && framuga == "Да")
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
            else if (Furn == "Maco_MM" && framuga == "Да")
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
            else if (Furn == "Maco_MM" && shtulp == "Да" && shtulpTreeArg == "Запор")
            {
                if (FFH < 840)
                {
                    MaterialMessageBox.ShowDialog("Высота штульпового окна с запором не может быть менее 840 мм");
                    return 1;
                }
                else if (FFH > 2250)
                {
                    MaterialMessageBox.ShowDialog("Высота штульпового окна с запором не может быть более 2250 мм");
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

            else if (Furn == "Maco_MM" && shtulp == "Да" && shtulpTreeArg == "Шпингалет")
            {
                if (FFH < 470)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 470 мм");
                    return 1;
                }
                else if (FFB < 320)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 320 мм");
                    return 1;
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2250 мм");
                    return 1;
                }
                else if (FFB > 1300)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 1300 мм");
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


            else if (Furn == "Vorne" && rotation == "Нет" && framuga == "Нет")
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
            else if (Furn == "Vorne" && framuga == "Да")
            {
                if (FFH < 300)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 300 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 2400 мм");
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
            else if (Furn == "Akpen" && rotation == "Нет" && framuga == "Нет")
            {
                if (FFH < 500)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Akpen не может быть менее 500 мм");
                    return 1;
                }
                else if (FFB < 350)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Akpen не может быть менее 350 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Akpen не может быть более 2400 мм");
                    return 1;
                }
                else if (FFB > 1100)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Akpen не может быть ,более 1150 мм");
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
            else if (Furn == "Akpen" && rotation == "Да")
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
            else if (Furn == "Akpen" && framuga == "Да")
            {
                if (FFH < 300)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 300 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 2400 мм");
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


            else if ((Furn == "Roto_NT" || Furn == "Roto_NX") && rotation == "Нет" && framuga == "Нет")
            {
                if (FFH < 310)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Roto не может быть менее 310 мм");
                    return 1;
                }
                else if (FFB < 290)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Roto не может быть менее 290 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Высота п/о Roto не может быть более 2400 мм");
                    return 1;
                }
                else if (FFB > 1400)
                {
                    MaterialMessageBox.ShowDialog("Ширина п/о Roto не может быть ,более 1400 мм");
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
            else if ((Furn == "Roto_NT" || Furn == "Roto_NX") && rotation == "Да")
            {
                if (FFH < 251)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 251 мм");
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
            else if ((Furn == "Roto_NT" || Furn == "Roto_NX") && framuga == "Да")
            {
                if (FFH < 251)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 251 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 2400 мм");
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
            else if (Furn == "Internika" && rotation == "Нет" && framuga == "Нет")
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
            else if (Furn == "Internika" && framuga == "Да")
            {
                if (FFH < 501)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 501 мм");
                    return 1;
                }
                else if (FFH > 2400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть более 2400 мм");
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
