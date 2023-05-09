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
        public void Err(string Furn, int FFH, int FFB, int quantity)
        {
            if (Furn == "Maco_Eco")
            {
                if (FFH < 601)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 601 мм");
                    //MessageBox.Show("Высота не может быть менее 601 мм", "Не вышло");
                }
                else if (FFB < 400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 400 мм");
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2350 мм");
                }
                else if (FFB > 1050)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1050 мм");
                }
                if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                }
            }
            return;
        }
    }
}
