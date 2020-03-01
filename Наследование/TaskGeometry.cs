using System;
//ball:       4.0 * Math.PI * Math.Pow(((Ball)this).Radius, 3) / 3;
//cube:      Math.Pow(((Cube)this).Size, 3);
//cyclinder: Math.PI * Math.Pow(c.Radius, 2) * c.Height;
namespace Inheritance.Geometry
{
    public interface IVisitor
    {
        void CalculateArea(Body figure);
        void Dimensions(Body figure);
    }

	public abstract class Body
	{
        public abstract double GetVolume();
        public abstract void Accept(IVisitor visitor);
	}

	public class Ball : Body
	{
		public double Radius { get; set; }
        public override void Accept(IVisitor visitor) { visitor.CalculateArea(this); visitor.Dimensions(this); }
        public override double GetVolume() => 4.0 * Math.PI * (Radius * Radius * Radius) / 3;
    }

	public class Cube : Body
	{
		public double Size { get; set; }
        public override void Accept(IVisitor visitor) { visitor.CalculateArea(this); visitor.Dimensions(this); }
        public override double GetVolume() => Size * Size * Size;
    }

    public class Cylinder : Body
    {
        public double Height { get; set; }
        public double Radius { get; set; }
        public override void Accept(IVisitor visitor) { visitor.CalculateArea(this); visitor.Dimensions(this); }
        public override double GetVolume() => Math.PI * (Radius * Radius) * Height;
    }

	// Заготовка класса для задачи на Visitor
	public class SurfaceAreaVisitor : IVisitor
	{
		public double SurfaceArea { get; private set; }
        void IVisitor.CalculateArea(Body figure)
        {
            if (figure is Ball)
            {
                var fig = figure as Ball;
                SurfaceArea = 4 * Math.PI * fig.Radius * fig.Radius;
            }
            if (figure is Cube)
            {
                var fig = figure as Cube;
                SurfaceArea = fig.Size * fig.Size * 6;
            }
            if (figure is Cylinder)
            {
                var fig = figure as Cylinder;
                SurfaceArea = 2 * Math.PI * fig.Radius * fig.Height + 2 * Math.PI * fig.Radius * fig.Radius;
            }
        }

        public void Dimensions(Body figure) { }
    }

	public class DimensionsVisitor : IVisitor
	{
		public Dimensions Dimensions { get; private set; }
        void IVisitor.CalculateArea(Body figure) { }
        void IVisitor.Dimensions(Body figure)
        {
            if (figure is Ball)
            {
                var fig = figure as Ball;
                Dimensions = new Dimensions(fig.Radius * 2, fig.Radius * 2);
            }
            if (figure is Cylinder)
            {
                var fig = figure as Cylinder;
                Dimensions = new Dimensions(fig.Radius * fig.Radius, fig.Height);
            }
            if (figure is Cube)
            {
                var fig = figure as Cube;
                Dimensions = new Dimensions(fig.Size, fig.Size);
            }
        }
    }
}
