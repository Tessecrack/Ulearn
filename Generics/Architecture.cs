using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IRobotAI<TypeCommand> { TypeCommand GetCommand(); }

    public interface IDevice<in T> { string ExecuteCommand(T command); }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        int counter = 1;
        public ShooterCommand GetCommand() => ShooterCommand.ForCounter(counter++);
    }

    public class BuilderAI : IRobotAI<BuilderCommand>
    {
        int counter = 1;
        public BuilderCommand GetCommand() => BuilderCommand.ForCounter(counter++);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand command) => command == null ?
            throw new ArgumentException() : $"MOV {command.Destination.X}, {command.Destination.Y}";
    }

    public class Robot<Type>
    {
        readonly IRobotAI<Type> ai;
        readonly IDevice<Type> device;

        public Robot(IRobotAI<Type> ai, IDevice<Type> executor)
        {
            this.ai = ai;
            device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }

        }
    }

    public class Robot
    {
        public static Robot<T> Create<T>(IRobotAI<T> ai, IDevice<T> device) => new Robot<T>(ai, device);
    }
}
