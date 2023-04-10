
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


        public int Que(int FFH, int FFB)
        {
            //Варианты запросов в БД с разными параметрами, выбранными в форме

            //(высота до 900, ширина до 431)
            if (FFH >= 601 && FFH <= 900 && FFB <= 430)
            {
                return quantityBar = 2;
            }

            //*********************

            //Высота до 1300, ширина до 431
            else if (FFH >= 901 && FFH <= 1300 && FFB <= 430)
            {
                return quantityBar = 3;
            }

            //*********************

            //Высота от1301 до 1800, ширина до 431
            else if (FFH >= 1301 && FFH <= 1800 && FFB <= 430)
            {
                return quantityBar = 5;
            }

            //*********************

            //Высота от 1801 до 2350, ширина до 431
            else if (FFH >= 1801 && FFH <= 2350 && FFB <= 430)
            {
                return quantityBar = 6;
            }

            //*********************

            //(высота до 900, ширина до 600)
            else if (FFH <= 900 && FFB >= 431 && FFB <= 600)
            {
                return quantityBar = 2;
            }

            //*********************

            //Высота до 1300, ширина до 600
            else if (FFH >= 901 && FFH <= 1300 && FFB >= 431 && FFB <= 600)
            {
                return quantityBar = 3;
            }

            //*********************

            //Высота от1301 до 1800, ширина до 600
            else if (FFH >= 1301 && FFH <= 1800 && FFB >= 431 && FFB <= 600)
            {
                return quantityBar = 5;
            }

            //*********************

            //Высота от 1801 до 2350, ширина до 600
            else if (FFH >= 1801 && FFH <= 2350 && FFB >= 431 && FFB <= 600)
            {
                return quantityBar = 6;
            }

            //*********************

            //ШИРИНА 601-800
            //Высота до 900, ширина 601-800

            else if (FFH <= 900 && FFB >= 601 && FFB <= 800)
            {
                return quantityBar = 2;
            }

            //*********************

            //Высота до 1300, ширина 601-800
            else if (FFH >= 901 && FFH <= 1300 && FFB >= 601 && FFB <= 800)
            {
                return quantityBar = 3;
            }

            //*********************

            //Высота от1301 до 1800, ширина 601-800
            else if (FFH >= 1301 && FFH <= 1800 && FFB >= 601 && FFB <= 800)
            {
                return quantityBar = 5;
            }

            //*********************

            //Высота от 1801 до 2350, ширина до 600
            else if (FFH >= 1801 && FFH <= 2350 && FFB >= 601 && FFB <= 800)
            {
                return quantityBar = 7;
            }

            //*********************

            //ШИРИНА 801-1050
            //Высота до 900, ширина 801-1050
            else if (FFH <= 900 && FFB >= 801 && FFB <= 1050)
            {
                return quantityBar = 4;
            }

            //*********************

            //Высота до 1300, ширина 801-1050
            else if (FFH >= 901 && FFH <= 1300 && FFB >= 801 && FFB <= 1050)
            {
                return quantityBar = 5;
            }

            //*********************

            //Высота от1301 до 1800, ширина 801-1050
            else if (FFH >= 1301 && FFH <= 1800 && FFB >= 801 && FFB <= 1050)
            {
                return quantityBar = 7;
            }

            //*********************

            //Высота от 1801 до 2350, ширина 801-1050
            else if (FFH >= 1801 && FFH <= 2350 && FFB >= 801 && FFB <= 1050)
            {
                return quantityBar = 8;
            }

            else
            {
                return 0;
            }
        }

    }
}

