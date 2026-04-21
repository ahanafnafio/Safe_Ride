namespace Saferide.Models
{
    public class Vehicle
    {
        public static int nextVehicleId = 1;
        private int vehicleId;
        private string make;
        private string model;
        private string color;
        private string plate;
        private string notes;
        // Constrcutor
        public Vehicle(string make, string model, string color, string plate, string notes)
        {
            vehicleId = nextVehicleId++;
            this.make = make;
            this.model = model;
            this.color = color;
            this.plate = plate;
            this.notes = notes;
        }
        // Methods
        public int GetVehicleId()
        {
            return vehicleId;
        }
        public string GetMake()
        {
            return make;
        }

        public string GetModel()
        {
            return model;
        }

        public string GetColor()
        {
            return color;
        }

        public string GetPlate()
        {
            return plate;
        }

        public string GetNotes()
        {
            return notes;
        }
    }
}