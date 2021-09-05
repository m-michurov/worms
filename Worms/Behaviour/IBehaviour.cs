namespace Worms.Behaviour {
    internal interface IBehaviour {
        Action NextAction(Simulation simulation, Worm worm);
    }
}