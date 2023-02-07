using System;
using System.Collections.Generic;

namespace Module7
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
    //TODO можно создать отдельный метод в Delivery, который проверяет, есть ли рабочий в аккредитованном списке 
    abstract class Delivery
    {
        /// <summary>
        /// Этот метод возвращает предполагаемую дату доставки, рассчитывая от текущей даты, учитывая рабочее время
        /// </summary>
        /// <param name="DaysToDeliver"></param>
        /// <returns></returns>
        protected virtual DateTime SetEstDeliveryDate(int DaysToDeliver)
        {
            DateTime orderDate = DateTime.Now.TimeOfDay.Hours >= 18 ? DateTime.Now.AddDays(1) : DateTime.Now;

            for (int i = 0; i < DaysToDeliver; i++)
            {
                orderDate = orderDate.AddDays(1);
                switch (orderDate.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        DaysToDeliver++;
                        break;
                    case DayOfWeek.Saturday:
                        DaysToDeliver++;
                        break;
                }
            }
            return orderDate.Date;
        }

    }

    class HomeDelivery : Delivery
    {
        private ExternalDeliveryEmploye deliveryEmpl;
        public static int DaysToDeliver = 5; 

        public HomeDelivery(ExternalDeliveryEmploye deliveryEmpl)
        {
            if (deliveryEmpl.WorkZone == Zones.City || deliveryEmpl.WorkZone == Zones.Suburb)
                this.deliveryEmpl = deliveryEmpl;
        }
    }

    class PickPointDelivery : Delivery
    {
        private ExternalDeliveryEmploye deliveryEmpl;
        public static int DaysToDeliver = 4;

        public PickPointDelivery(ExternalDeliveryEmploye deliveryEmpl)
        {
            if (deliveryEmpl.WorkZone == Zones.Pickpoint)
            this.deliveryEmpl = deliveryEmpl;
        }
    }

    class ShopDelivery : Delivery
    {
        private InternalDeliveryEmploye deliveryEmpl;
        public static int DaysToDeliver = 3;

        public ShopDelivery(InternalDeliveryEmploye deliveryEmpl)
        {
            if (deliveryEmpl.WorkZone == Zones.Shop)
            this.deliveryEmpl = deliveryEmpl;
        }
    }

    class Order<TDelivery, TStruct> where TDelivery: Delivery where TStruct : Adress
    {
        public TDelivery Delivery;
        public TStruct Adress;
        public int Number;
        public string Description;

        public void DisplayAddress()
        {
            Console.WriteLine(Adress.StreetAdress);
        }
    }
    public enum Zones
    {
          City = 1,
          Suburb = 2,
          Shop = 3,
          Pickpoint = 4
    }

    class Adress
    {
        public string StreetAdress { get { return StreetAdress + " в зоне номер " + CustomerZone; } set { StreetAdress = value; } }
        public int DaysToDeliver { get; private set; }
        public Zones CustomerZone { get; private set; }

        public void SetZone()
        {
            Console.WriteLine("Выберите зону доставки (от 1 - 4).");
            bool isInputCorrect = int.TryParse(Console.ReadLine(), out int tempZone);
            if (isInputCorrect && 0 < tempZone && tempZone <= Enum.GetValues(typeof(Zones)).Length)
                CustomerZone = (Zones)tempZone;
            else
            {
                Console.WriteLine("Вы ввели некорректные данные, попробуйте снова.");
                SetZone();
            }
        }
    }


    


    abstract class Employe
    {
        public string Name
        {
            get;
            protected set;
        }
        public bool IsOnWork
        {
            get;
            protected set;
        }
        
        public Employe()
        {
            Name = "Имя не инициализировано";
            IsOnWork = false;
        }

        public Employe(string EmplName)
        {
            Name = EmplName;
            IsOnWork = false;
        }

        public Employe(string EmplName, bool IsWorking)
        {
            Name = EmplName;
            IsOnWork = IsWorking;
        }
        /// <summary>
        /// Переключает состояние На работе/Не на работе
        /// </summary>
        /// <param name="isOnShift"></param>
        public void ChangeShifts(bool isOnShift)
        {
            if (isOnShift)
                IsOnWork = false;
            else
                IsOnWork = true;
        }
    }
    
    class ExternalDeliveryEmploye : Employe
    {
        public Zones WorkZone
        {
            get;
            protected set;
        }
        //Устанавливаем зону обслуживания рабочему доставки, так как у нас 3 разные внешние зоны - City, Suburb и Pickpoint
        public ExternalDeliveryEmploye(string emplName, bool isWorking, Zones workZone) : base(emplName,isWorking)
        {
            WorkZone = workZone;
        }
    }
    
    class InternalDeliveryEmploye : Employe
    {
        public int ShopID { get; private set; }
        // У рабочих в магазине всегда одна зона - Shop
        public Zones WorkZone { get; private set; }
        public InternalDeliveryEmploye(string emplName, bool isWorking, int shopID) : base(emplName, isWorking)
        {
            ShopID = shopID;
            WorkZone = Zones.Shop;
        }
    }



    //Создаем статический лист со всеми аккредитованными рабочими и создаем метод расширения, который добавит или уберёт рабочего в/из списка.

    static class EmployeExtensions
    {
        public static List<Employe> AccreditedEmployeList; 
        public static void AddAccreditedEmploye(this Employe employe)
        {
            AccreditedEmployeList.Add(employe);
        }
        public static void RemoveEmployeFromAccrList(this Employe employe)
        {
            AccreditedEmployeList.Remove(employe);
        }
    }
}
