using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    // In real aplication it whould be the place where database is used to find driver by its Id.
    // But in this exercise it is just a mock to simulate database
    public class Client
    {
        public PersonName ClientName;
        public Client(PersonName nameClient)
        {
            ClientName = new PersonName(nameClient.FirstName, nameClient.LastName);
        }
    }
    public class Driver : Entity<int>
    {
        public int? DriverId { get; }
        public PersonName DriverName { get; }
        public Car Car { get; set; }
        public Driver(int idDriver, PersonName nameDriver) : base(idDriver)
        {
            this.DriverId = idDriver;
            if (nameDriver != null) DriverName = new PersonName(nameDriver.FirstName, nameDriver.LastName);
            else DriverName = new PersonName(null, null);
        }
    }

    public class Car : ValueType<Car>
    {
        public string carColor { get; }
        public string carModel { get; }
        public string carPlateNumber { get; }
        public Car(string color, string model, string number)
        {
            carColor = color;
            carModel = model;
            carPlateNumber = number;
        }
    }
    public class DriversRepository
    {
        public void FillDriverToOrder(int driverId, TaxiOrder order) { }
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

        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building)
        {
            var nameClient = new PersonName(firstName, lastName);
            var address = new Address(street, building);
            return new TaxiOrder(idCounter++, nameClient, address, null, null, null, currentTime());
        }
        public void UpdateDestination(TaxiOrder order, string street, string building) => order.UpdateDestination(new Address(street, building));
        public void AssignDriver(TaxiOrder order, int driverId) => order.AssignDriver(currentTime(), driverId);
        public void UnassignDriver(TaxiOrder order) => order.UnassignDriver();
        public string GetDriverFullInfo(TaxiOrder order) => order.GetDriverFullInfo();
        public string GetShortOrderInfo(TaxiOrder order) => order.GetShortOrderInfo();
        private DateTime GetLastProgressTime(TaxiOrder order) => order.GetLastProgressTime();
        public void Cancel(TaxiOrder order) => order.Cancel(currentTime());
        public void StartRide(TaxiOrder order) => order.StartRide(currentTime());
        public void FinishRide(TaxiOrder order) => order.FinishRide(currentTime());
    }

    public class TaxiOrder : Entity<int>
    {
        private bool readyDriver = false;
        private readonly int id;
        public PersonName ClientName { get; private set; }
        public Address Start { get; private set; }
        public Address Destination { get; private set; }
        public Driver Driver { get; private set; }

        private TaxiOrderStatus status;

        private DateTime creationTime;
        private DateTime driverAssignmentTime;
        private DateTime cancelTime;
        private DateTime startRideTime;
        private DateTime finishRideTime;

        public TaxiOrder(int idNumber, PersonName client, Address start, Address destination, Driver driver, Car car, DateTime time)
            : base(idNumber)
        {
            id = idNumber;
            this.ClientName = client;
            Start = start;
            Destination = destination;
            this.Driver = driver;
            creationTime = time;
        }

        public void UpdateDestination(Address address)
        {
            Destination = new Address(address.Street, address.Building);
        }

        private Car GetReposCar()
        {
            var carModel = "Lada sedan";
            var carColor = "Baklazhan";
            var carPlateNumber = "A123BT 66";
            return new Car(carColor, carModel, carPlateNumber);
        }

        public void AssignDriver(DateTime time, int driverId)
        {
            if (Driver == null)
            {
                if (driverId == 15)
                {
                    var driverFirstName = "Drive";
                    var driverLastName = "Driverson";
                    var name = new PersonName(driverFirstName, driverLastName);
                    Driver = new Driver(driverId, name);
                    Driver.Car = GetReposCar();
                    readyDriver = true;
                }
                else throw new Exception("Unknown driver id " + driverId);
                driverAssignmentTime = time;
                status = TaxiOrderStatus.WaitingCarArrival;
            }
            else throw new InvalidOperationException();
        }

        public void UnassignDriver()
        {
            if (status == TaxiOrderStatus.InProgress || Driver == null || !readyDriver) 
                throw new InvalidOperationException(status.ToString());
            Driver = new Driver(default(int), null);
            status = TaxiOrderStatus.WaitingForDriver;
        }

        public string GetDriverFullInfo()
        {
            if (status == TaxiOrderStatus.WaitingForDriver) return null;
            return string.Join(" ",
                "Id: " + Driver.DriverId,
                "DriverName: " + FormatName(Driver.DriverName.FirstName, Driver.DriverName.LastName),
                "Color: " + Driver.Car.carColor,
                "CarModel: " + Driver.Car.carModel,
                "PlateNumber: " + Driver.Car.carPlateNumber);
        }

        public string GetShortOrderInfo()
        {
            var driver = "";
            var dest = "";
            if (this.Driver != null) driver = FormatName(this.Driver.DriverName.FirstName, this.Driver.DriverName.LastName);
            if (Destination != null) dest = FormatAddress(Destination.Street, Destination.Building);
            return string.Join(" ",
                "OrderId: " + Id,
                "Status: " + status,
                "Client: " + FormatName(ClientName.FirstName, ClientName.LastName),
                "Driver: " + driver,
                "From: " + FormatAddress(Start.Street, Start.Building),
                "To: " + dest,
                "LastProgressTime: " + GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        public DateTime GetLastProgressTime()
        {
            if (status == TaxiOrderStatus.WaitingForDriver) return creationTime;
            if (status == TaxiOrderStatus.WaitingCarArrival) return driverAssignmentTime;
            if (status == TaxiOrderStatus.InProgress) return startRideTime;
            if (status == TaxiOrderStatus.Finished) return finishRideTime;
            if (status == TaxiOrderStatus.Canceled) return cancelTime;
            throw new NotSupportedException(status.ToString());
        }
        public string FormatName(string firstName, string lastName)
            => string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));

        public string FormatAddress(string street, string building) =>
            string.Join(" ", new[] { street, building }.Where(n => n != null));

        public void Cancel(DateTime time)
        {
            if (status == TaxiOrderStatus.InProgress) throw new InvalidOperationException();
            status = TaxiOrderStatus.Canceled;
            cancelTime = time;
        }

        public void StartRide(DateTime time)
        {
            if (!readyDriver) throw new InvalidOperationException();
            status = TaxiOrderStatus.InProgress;
            startRideTime = time;
        }

        public void FinishRide(DateTime time)
        {
            if (status == TaxiOrderStatus.WaitingCarArrival || !readyDriver) 
                throw new InvalidOperationException();
            status = TaxiOrderStatus.Finished;
            finishRideTime = time;
        }
    }
}