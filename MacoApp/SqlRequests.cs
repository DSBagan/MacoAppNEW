
namespace MacoApp
{
    internal class SqlRequests
    {
        public int System;
        public string Side;
        public int FFH;
        public int FFB;
        public string Lower_loop;
        public string Micro_ventilation;
        public string queryString;
        public int quantityBar = 0;
        public int quantitySrPr = 0;

        public int Que(string Rotation, string Furn, int FFH, int FFB)
        {
            //Варианты запросов в БД с разными параметрами, выбранными в форме
            if (Furn == "Maco_Eco")
            {
                //(высота до 900, ширина до 431)
                if (FFH >= 601 && FFH <= 900 && FFB <= 430 && Rotation == "Нет")
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да"&& FFH >= 601 && FFH <= 900)
                {
                    return quantityBar = 1;
                }

                //*********************

                //Высота до 1300, ширина до 431
                else if (FFH >= 901 && FFH <= 1300 && FFB <= 430 && Rotation == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 901 && FFH <= 1300)
                {
                    return quantityBar = 2;
                }
                //*********************

                //Высота от1301 до 1800, ширина до 431
                else if (FFH >= 1301 && FFH <= 1800 && FFB <= 430 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (Rotation == "Да" && FFH >= 1301 && FFH <= 1800)
                {
                    return quantityBar = 2;
                }
                //*********************

                //Высота от 1801 до 2350, ширина до 431
                else if (FFH >= 1801 && FFH <= 2350 && FFB <= 430 && Rotation == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (Rotation == "Да" && FFH >= 1801 && FFH <= 2350)
                {
                    return quantityBar = 3;
                }
                //*********************

                //(высота до 900, ширина до 600)
                else if (FFH <= 900 && FFB >= 431 && FFB <= 600 && Rotation == "Нет")
                {
                    return quantityBar = 2;
                }

                //*********************

                //Высота до 1300, ширина до 600
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 431 && FFB <= 600 && Rotation == "Нет")
                {
                    return quantityBar = 3;
                }

                //*********************

                //Высота от1301 до 1800, ширина до 600
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 431 && FFB <= 600 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота от 1801 до 2350, ширина до 600
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 431 && FFB <= 600 && Rotation == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                //ШИРИНА 601-800
                //Высота до 900, ширина 601-800

                else if (FFH <= 900 && FFB >= 601 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 2;
                }

                //*********************

                //Высота до 1300, ширина 601-800
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 601 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 3;
                }

                //*********************

                //Высота от1301 до 1800, ширина 601-800
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 601 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота от 1801 до 2350, ширина до 600
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 601 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 7;
                }

                //*********************

                //ШИРИНА 801-1050
                //Высота до 900, ширина 801-1050
                else if (FFH <= 900 && FFB >= 801 && FFB <= 1050 && Rotation == "Нет")
                {
                    return quantityBar = 4;
                }

                //*********************

                //Высота до 1300, ширина 801-1050
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 801 && FFB <= 1050 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота от1301 до 1800, ширина 801-1050
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 801 && FFB <= 1050 && Rotation == "Нет")
                {
                    return quantityBar = 7;
                }

                //*********************

                //Высота от 1801 до 2350, ширина 801-1050
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 801 && FFB <= 1050 && Rotation == "Нет")
                {
                    return quantityBar = 8;
                }

                else
                {
                    return 0;
                }
            }
            else if (Furn == "Maco_MM")
            {
                if (FFH >= 470 && FFH <= 800 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 1;
                }
                else if (FFH >= 470 && FFH <= 800 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 2;
                }
                else if (FFH >= 470 && FFH <= 800 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 3;
                }
                //**********************
                else if (FFH >= 801 && FFH <= 1250 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 801 && FFH <= 1250 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 801 && FFH <= 1250 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                else if (FFH >= 1251 && FFH <= 1350 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 1251 && FFH <= 1350 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 1251 && FFH <= 1350 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                else if (FFH >= 1351 && FFH <= 1500 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 1351 && FFH <= 1500 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1351 && FFH <= 1500 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 8;
                }

                //*********************

                else if (FFH >= 1501 && FFH <= 1750 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1501 && FFH <= 1750 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1501 && FFH <= 1750 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 8;
                }

                //*********************

                else if (FFH >= 1751 && FFH <= 2000 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1751 && FFH <= 2000 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 1751 && FFH <= 2000 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 9;
                }

                //*********************

                else if (FFH >= 2001 && FFH <= 2250 && FFB <= 700 && Rotation == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 2001 && FFH <= 2250 && FFB >= 701 && FFB <= 800 && Rotation == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 2001 && FFH <= 2250 && FFB >= 801 && Rotation == "Нет")
                {
                    return quantityBar = 9;
                }

                //Поворотная 
                else if (Rotation == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да" && FFH >= 1001 && FFH <= 1800)
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 1801 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }
                else
                {
                    return 0;
                }
            }
            else { return 0; }


        }

        public int QueSrPr(string Rotation, string Furn, int FFH)
        {
            if (Rotation == "Да" && (Furn == "Maco_MM" || Furn == "Maco_Eco"))
            {
                if (FFH >= 300 && FFH <= 1300)
                {
                    return quantitySrPr = 1;
                }
                else if (FFH >= 1301 && FFH <= 1800)
                {
                    return quantitySrPr = 2;
                }
                else if (FFH >= 1801 && FFH <= 2400)
                {
                    return quantitySrPr = 3;
                }
                else
                {
                    return 0;
                }
                
            }
            else
                return 0;
        }
    }
}

