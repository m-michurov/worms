using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Worms.Behaviour;
using Worms.DTO;
using Worms.Utility;
using WormsServer.DTO;
using Action = Worms.Action;

namespace WormsServer.Controllers {
    [ApiController]
    public sealed class WormActionController : ControllerBase {
        private readonly IBehaviour behaviour = new HedonisticBehaviour();

        [HttpPost]
        [HttpGet]
        [Route("/{name}/getAction")]
        public JsonResult GetAction(
            string name,
            [FromBody] World world
        ) {
            var simulation = world.ToSimulationState();
            var nextAction = behaviour.NextAction(simulation, simulation.Worms.First(it => it.Name == name));

            var direction = Direction.Up.ToString();
            var split = false;

            switch (nextAction) {
                case Action.Move move:
                    direction = move.Direction.ToString();
                    break;
                case Action.Reproduce reproduce:
                    direction = reproduce.Direction.ToString();
                    split = true;
                    break;
            }

            return new JsonResult(
                new Response {
                    Action = new Worms.DTO.Action {
                        Direction = direction,
                        Split = split
                    }
                }
            );
        }
    }
}