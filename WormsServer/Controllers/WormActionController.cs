using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Worms.DTO;
using Worms.Utility;
using WormsServer.DTO;
using Action = Worms.Action;

namespace WormsServer.Controllers {
    [ApiController]
    public sealed class WormActionController : ControllerBase {
        private readonly BehaviourWrapper behaviours;
        public WormActionController(BehaviourWrapper behaviours_) => behaviours = behaviours_;

        [HttpPost]
        [HttpGet]
        [Route("/{name}/getAction")]
        public JsonResult GetAction(
            string name,
            [FromBody] World world
        ) {
            var step = int.Parse(Request.Query["step"]);
            var run = int.Parse(Request.Query["run"]);
            
            var simulation = world.ToSimulationState();
            var nextAction = behaviours[run].NextAction(
                simulation,
                simulation.Worms.First(it => it.Name == name),
                step
            );

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
                new Worms.DTO.Action {
                    Direction = direction,
                    Split = split
                }
            );
        }
    }
}