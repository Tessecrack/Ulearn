using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    // In real aplication it whould be the place where database is used to find driver by its Id.
    // But in this exercise it is just a mock to simulate database
    public class DriversRepository
    {
        public void FillDriverToOrder(int driverId, TaxiOrder order)
        {
            if (driverId == 15)
            {
                order.AddDriver(new Driver(driverId,
                    new PersonName("Drive", "Driverson"),
                    "Baklazhan",
                    "Lada sedan",
                    "A123BT 66"));
            }
            else
                throw new Exception("Unknown driver id " + driverId);
        }
    }

    public class TaxiApi : ITaxiApi<TaxiOrder>
    {
        private readonly DriversRepository driversRepo;
        private readonly Func<DateTime> currentTime;
        private int idCounter;

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            this.driversRepo = driversRepo;
            this.currentTime = currentTime;
        }

        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, 
		string building)
        {
            var taxiOrder = new TaxiOrder(idCounter++);
            taxiOrder.AddClient(new PersonName(firstName, lastName));
            taxiOrder.AddStartPoint(new Address(street, building));
            taxiOrder.AddCreationTime(currentTime());
            return taxiOrder;
        }

        public void UpdateDestination(TaxiOrder order, string street, string building)
            => order.UpdateDestination(new Address(street, building));

        public void AssignDriver(TaxiOrder order, int driverId)
        {
            driversRepo.FillDriverToOrder(driverId, order);
            order.AssignDriver(currentTime);
        }

        public void UnassignDriver(TaxiOrder order) => order.UnassignDriver();

        public string GetDriverFullInfo(TaxiOrder order)
        {
            if (order.Status == TaxiOrderStatus.WaitingForDriver) return null;
            return string.Join(" ",
                "Id: " + order.Driver.DriverId,
                "DriverName: " + FormatName(order.Driver.DriverName.FirstName, 
				order.Driver.DriverName.LastName),
                "Color: " + order.Driver.Car.CarColor,
                "CarModel: " + order.Driver.Car.CarModel,
                "PlateNumber: " + order.Driver.Car.CarPlateNumber);
        }

        public string GetShortOrderInfo(TaxiOrder order)
        {
            var toAddress = order.Destination == null ? "" :
                FormatAddress(order.Destination.Street, order.Destination.Building);
            var driver = order.Driver == null ? ""
                : FormatName(order.Driver.DriverName.FirstName, order.Driver.DriverName.LastName);
            return string.Join(" ",
                "OrderId: " + order.Id,
                "Status: " + order.Status,
                "Client: " + FormatName(order.ClientName.FirstName, order.ClientName.LastName),
                "Driver: " + driver,
                "From: " + FormatAddress(order.Start.Street, order.Start.Building),
                "To: " + toAddress,
                "LastProgressTime: " + GetLastProgressTime(order)
				.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        private DateTime GetLastProgressTime(TaxiOrder order)
        {
            if (order.Status == TaxiOrderStatus.WaitingForDriver) return order.CreationTime;
            if (order.Status == TaxiOrderStatus.WaitingCarArrival) return order.DriverAssignmentTime;
            if (order.Status == TaxiOrderStatus.InProgress) return order.StartRideTime;
            if (order.Status == TaxiOrderStatus.Finished) return order.FinishRideTime;
            if (order.Status == TaxiOrderStatus.Canceled) return order.CancelTime;
            throw new NotSupportedException(order.Status.ToString());
        }

        private string FormatName(string firstName, string lastName) => firstName == "" && lastName == "" ?
            "" : string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));

        private string FormatAddress(string street, string building)
            => string.Join(" ", new[] { street, building }.Where(n => n != null));

        public void Cancel(TaxiOrder order) => order.Cancel(currentTime);

        public void StartRide(TaxiOrder order) => order.StartRide(currentTime);

        public void FinishRide(TaxiOrder order) => order.FinishRide(currentTime);
    }

    public class TaxiOrder : Entity<int>
    {
        public TaxiOrder(int idNumber) : base(idNumber) => id = idNumber;

        private bool isAssignDriver = false;

        private readonly int id;
        public PersonName ClientName { get; private set; }
        public Address Start { get; private set; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }
        public TaxiOrderStatus Status { get; private set; }
        public void AssignDriver(Func<DateTime> currentTime)
        {
            if (!isAssignDriver)
            {
                DriverAssignmentTime = currentTime();
                AddTaxiOrderStatus(TaxiOrderStatus.WaitingCarArrival);
                isAssignDriver = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
		
        public void UnassignDriver()
        {
            if (Status == TaxiOrderStatus.InProgress) throw new InvalidOperationException();
            if (Driver == null) throw new InvalidOperationException("WaitingForDriver");
            AddDriver(
                new Driver(Driver.DriverId,
                new PersonName("", ""),
                Driver.Car.CarColor,
                Driver.Car.CarModel,
                Driver.Car.CarPlateNumber));
            AddTaxiOrderStatus(TaxiOrderStatus.WaitingForDriver);
        }
		
        public void Cancel(Func<DateTime> currentTime)
        {
            if (Status == TaxiOrderStatus.InProgress)
            {
                throw new InvalidOperationException();
            }
            AddTaxiOrderStatus(TaxiOrderStatus.Canceled);
            CancelTime = currentTime();
        }

        public void UpdateDestination(Address address)
            => AddDestinationPoint(new Address(address.Street, address.Building));
        public void StartRide(Func<DateTime> currentTime)
        {
            if (!isAssignDriver)
                throw new InvalidOperationException();
            AddTaxiOrderStatus(TaxiOrderStatus.InProgress);
            StartRideTime = currentTime();
        }

        public void FinishRide(Func<DateTime> currentTime)
        {
            if (!isAssignDriver)
                throw new InvalidOperationException();
            if (Status == TaxiOrderStatus.InProgress)
            {
                Status = TaxiOrderStatus.Finished;
                FinishRideTime = currentTime();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void AddDriver(Driver driver) => Driver = driver;

        public void AddClient(PersonName client) => ClientName = new PersonName(client.FirstName,
            client.LastName);
        public void AddStartPoint(Address address) => Start = new Address(address.Street,
            address.Building);
        public void AddDestinationPoint(Address address) => Destination = new Address(address.Street,
            address.Building);
        public void AddTaxiOrderStatus(TaxiOrderStatus status) => Status = status;
        public void AddCreationTime(DateTime creationTime) => CreationTime = creationTime;


        public DateTime CreationTime { get; private set; }
        public DateTime DriverAssignmentTime { get; private set; }
        public DateTime CancelTime { get; private set; }
        public DateTime StartRideTime { get; private set; }
        public DateTime FinishRideTime { get; private set; }
    }
	
    public class Driver : Entity<int>
    {
        public int DriverId { get; private set; }
        public PersonName DriverName { get; private set; }
        public Car Car { get; private set; }
        public Driver(int driverId, PersonName name, string carColor, string carModel, string carPlateNumber)
            : base(driverId)
        {
            DriverId = driverId;
            DriverName = name;
            Car = new Car(carColor, carModel, carPlateNumber);
        }
    }

    public class Car : ValueType<Car>
    {
        public string CarColor { get; private set; }
        public string CarModel { get; private set; }
        public string CarPlateNumber { get; private set; }
        public Car(string color, string model, string number)
        {
            CarColor = color;
            CarModel = model;
            CarPlateNumber = number;
        }
    }
}
