
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

        public int Que(string Rotation, string framuga, string Furn, int FFH, int FFB)
        {
            //Варианты запросов в БД с разными параметрами, выбранными в форме
            if (Furn == "Maco_Eco")
            {
                //(высота до 900, ширина до 431)
                if (FFH >= 601 && FFH <= 900 && FFB <= 430 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }

                //*********************

                //Высота до 1300, ширина до 431
                else if (FFH >= 901 && FFH <= 1300 && FFB <= 430 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                //*********************

                //Высота от1301 до 1800, ширина до 431
                else if (FFH >= 1301 && FFH <= 1800 && FFB <= 430 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                //*********************

                //Высота от 1801 до 2350, ширина до 431
                else if (FFH >= 1801 && FFH <= 2350 && FFB <= 430 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                //*********************

                //(высота до 900, ширина до 600)
                else if (FFH <= 900 && FFB >= 431 && FFB <= 600 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 2;
                }

                //*********************

                //Высота до 1300, ширина до 600
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 431 && FFB <= 600 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }

                //*********************

                //Высота от1301 до 1800, ширина до 600
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 431 && FFB <= 600 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота от 1801 до 2350, ширина до 600
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 431 && FFB <= 600 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                //ШИРИНА 601-800
                //Высота до 900, ширина 601-800

                else if (FFH <= 900 && FFB >= 601 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }

                //*********************

                //Высота до 1300, ширина 601-800
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 601 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }

                //*********************

                //Высота от1301 до 1800, ширина 601-800
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 601 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                //Высота от 1801 до 2350, ширина до 600
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 601 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }

                //*********************

                //ШИРИНА 801-1300
                //Высота до 900, ширина 801-1300
                else if (FFH <= 900 && FFB >= 801 && FFB <= 1300 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота до 1300, ширина 801-1300
                else if (FFH >= 901 && FFH <= 1300 && FFB >= 801 && FFB <= 1300 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                //Высота от1301 до 1800, ширина 801-1300
                else if (FFH >= 1301 && FFH <= 1800 && FFB >= 801 && FFB <= 1300 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }

                //*********************

                //Высота от 1801 до 2350, ширина 801-1300
                else if (FFH >= 1801 && FFH <= 2350 && FFB >= 801 && FFB <= 1300 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                //Поворотная и Фрамуга
                else if (Rotation == "Да" && FFH >= 300 && FFH <= 1300)
                {
                    return quantityBar = 1;
                }
                else if (Rotation == "Да" && FFH >= 1301 && FFH <= 2350)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 300 && FFH <= 1300)
                {
                    return quantityBar = 1;
                }
                else if (framuga == "Да" && FFH >= 1301 && FFH <= 2350)
                {
                    return quantityBar = 2;
                }
                else
                {
                    return 0;
                }
            }

            //MACO_MM********************************************************************************

            else if (Furn == "Maco_MM")
            {
                if (FFH >= 470 && FFH <= 800 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 1;
                }
                else if (FFH >= 470 && FFH <= 800 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 2;
                }
                else if (FFH >= 470 && FFH <= 800 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                //**********************
                else if (FFH >= 801 && FFH <= 1250 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 801 && FFH <= 1250 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 801 && FFH <= 1250 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }

                //*********************

                else if (FFH >= 1251 && FFH <= 1350 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 1251 && FFH <= 1350 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 1251 && FFH <= 1350 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                else if (FFH >= 1351 && FFH <= 1500 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 1351 && FFH <= 1500 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1351 && FFH <= 1500 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }

                //*********************

                else if (FFH >= 1501 && FFH <= 1750 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1501 && FFH <= 1750 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1501 && FFH <= 1750 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }

                //*********************

                else if (FFH >= 1751 && FFH <= 2000 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1751 && FFH <= 2000 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 1751 && FFH <= 2000 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 9;
                }

                //*********************

                else if (FFH >= 2001 && FFH <= 2250 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 2001 && FFH <= 2250 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 2001 && FFH <= 2250 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
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

                //Фрамуга 
                else if (framuga == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 1001 && FFH <= 1800)
                {
                    return quantityBar = 3;
                }
                else if (framuga == "Да" && FFH >= 1801 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }


                else
                {
                    return 0;
                }
            }

            //VORNE********************************************************************************

            else if (Furn == "Vorne")
            {
                if (FFH >= 450 && FFH <= 700 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 450 && FFH <= 700 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 450 && FFH <= 700 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                //**********************
                if (FFH >= 701 && FFH <= 1200 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 701 && FFH <= 1200 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 701 && FFH <= 1200 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                //**********************
                if (FFH >= 1201 && FFH <= 1700 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 1201 && FFH <= 1700 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1201 && FFH <= 1700 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                //**********************
                if (FFH >= 1701 && FFH <= 1900 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1701 && FFH <= 1900 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 1701 && FFH <= 1900 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 9;
                }
                //**********************
                if (FFH >= 1901 && FFH <= 2400 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1901 && FFH <= 2400 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 1901 && FFH <= 2400 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 10;
                }
                //**********************
                //Поворотная 
                else if (Rotation == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да" && FFH >= 1001 && FFH <= 1600)
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 1601 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }
                //Фрамуга
                else if (framuga == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 1001 && FFH <= 1600)
                {
                    return quantityBar = 3;
                }
                else if (framuga == "Да" && FFH >= 1601 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }

                else
                {
                    return 0;
                }
            }

            //Roto_NT************************************************************************************

            else if (Furn == "Roto_NT" || Furn == "Roto_NX")
            {
                if (FFH >= 310 && FFH <= 620 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 310 && FFH <= 620 && FFB >= 601 && FFB <= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 310 && FFH <= 620 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                //**********************
                if (FFH >= 621 && FFH <= 1200 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 621 && FFH <= 1200 && FFB >= 601 && FFB <= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 621 && FFH <= 1200 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }

                //*********************

                if (FFH >= 1201 && FFH <= 2000 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1201 && FFH <= 2000 && FFB >= 601 && FFB <= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1201 && FFH <= 2000 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }

                //*********************

                else if (FFH >= 2001 && FFH <= 2400 && FFB <= 700 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 2001 && FFH <= 2400 && FFB >= 701 && FFB <= 800 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 9;
                }
                else if (FFH >= 2001 && FFH <= 2400 && FFB >= 801 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 11;
                }


                //Поворотная 
                else if (Rotation == "Да" && FFH >= 251 && FFH <= 500)
                {
                    return quantityBar = 1;
                }
                else if (Rotation == "Да" && FFH >= 501 && FFH <= 800)
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да" && FFH >= 801 && FFH <= 1400)
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 1401 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }
                //Поворотная 
                else if (framuga == "Да" && FFH >= 251 && FFH <= 500)
                {
                    return quantityBar = 1;
                }
                else if (framuga == "Да" && FFH >= 501 && FFH <= 800)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 801 && FFH <= 1400)
                {
                    return quantityBar = 3;
                }
                else if (framuga == "Да" && FFH >= 1401 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }
                else
                {
                    return 0;
                }
            }

            //Internika***************************************************************************************
            else if (Furn == "Internika")
            {
                if (FFH >= 420 && FFH <= 600 && FFB <= 610 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 420 && FFH <= 600 && FFB >= 610 && FFB <= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 420 && FFH <= 600 && FFB >= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                //**********************

                else if (FFH >= 601 && FFH <= 1600 && FFB <= 610 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 601 && FFH <= 1600 && FFB >= 610 && FFB <= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 5;
                }
                else if (FFH >= 601 && FFH <= 1600 && FFB >= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }

                //*********************

                else if (FFH >= 1601 && FFH <= 2400 && FFB <= 610 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1601 && FFH <= 2400 && FFB >= 610 && FFB <= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1601 && FFH <= 2400 && FFB >= 810 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }


                //Поворотная 
                else if (Rotation == "Да" && FFH >= 501 && FFH <= 900)
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да" && FFH >= 901 && FFH <= 1400)
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 1401 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }

                //Фрамуга 
                else if (framuga == "Да" && FFH >= 501 && FFH <= 900)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 901 && FFH <= 1400)
                {
                    return quantityBar = 3;
                }
                else if (framuga == "Да" && FFH >= 1401 && FFH <= 2400)
                {
                    return quantityBar = 4;
                }
                else
                {
                    return 0;
                }
            }

            //Akpen*******************************************************************************************************
            else if (Furn == "Akpen")
            {
                if (FFH >= 500 && FFH <= 700 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 1;
                }
                else if (FFH >= 500 && FFH <= 700 && FFB >= 651 && FFB <= 900 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 3;
                }
                else if (FFH >= 500 && FFH <= 700 && FFB >= 951 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                //**********************
                if (FFH >= 701 && FFH <= 1400 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 4;
                }
                else if (FFH >= 701 && FFH <= 1400 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 701 && FFH <= 1400 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                //**********************
                if (FFH >= 1401 && FFH <= 1900 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 6;
                }
                else if (FFH >= 1401 && FFH <= 1900 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 7;
                }
                else if (FFH >= 1401 && FFH <= 1900 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 9;
                }
                //**********************
                if (FFH >= 1901 && FFH <= 2400 && FFB <= 650 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 8;
                }
                else if (FFH >= 1901 && FFH <= 2400 && FFB >= 651 && FFB <= 850 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 9;
                }
                else if (FFH >= 1901 && FFH <= 2400 && FFB >= 851 && Rotation == "Нет" && framuga == "Нет")
                {
                    return quantityBar = 11;
                }
                //**********************
                //Поворотная 
                else if (Rotation == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (Rotation == "Да" && FFH >= 1001 && FFH <= 1600)
                {
                    return quantityBar = 3;
                }
                else if (Rotation == "Да" && FFH >= 1601 && FFH <= 2200)
                {
                    return quantityBar = 4;
                }
                //Фрамуга
                else if (framuga == "Да" && FFH >= 300 && FFH <= 1000)
                {
                    return quantityBar = 2;
                }
                else if (framuga == "Да" && FFH >= 1001 && FFH <= 1600)
                {
                    return quantityBar = 3;
                }
                else if (framuga == "Да" && FFH >= 1601 && FFH <= 2200)
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

        //СРЕДНИЕ ПРИЖИМЫ++++++++++++++++++++++++++++

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
                    return quantitySrPr = 2;
                }
                else
                {
                    return 0;
                }            
            }
            else if (Rotation == "Да" && (Furn == "Vorne"|| Furn == "Akpen"))
            {
                if (FFH >= 300 && FFH <= 1400)
                {
                    return quantitySrPr = 1;
                }
                else if (FFH >= 1401 && FFH <= 2000)
                {
                    return quantitySrPr = 2;
                }
                else if (FFH >= 2001 && FFH <= 2400)
                {
                    return quantitySrPr = 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (Rotation == "Да" && (Furn == "Roto_NT" || Furn == "Roto_NX"))
            {
                if (FFH >= 251 && FFH <= 1400)
                {
                    return quantitySrPr = 1;
                }
                else if (FFH >= 1401 && FFH <= 2400)
                {
                    return quantitySrPr = 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (Rotation == "Да" && Furn == "Internika")
            {
                if (FFH >= 501 && FFH <= 1400)
                {
                    return quantitySrPr = 1;
                }
                else if (FFH >= 1401 && FFH <= 2400)
                {
                    return quantitySrPr = 2;
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

